using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeadWrongGames.ZUtils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace DeadWrongGames.ZCommon
{
    /// <summary>
    /// Base ScriptableObject for type-safe Addressables references.
    /// Use this class to create ScriptableObject wrappers around Addressable assets
    /// so you can safely store and load assets by key, avoiding broken references
    /// when swapping assets in the Addressables system.
    /// </summary>
    /// <typeparam name="TAsset">The type of the asset (AudioClip, GameObject, etc.).</typeparam>
    /// <typeparam name="TAssetReference">The Addressables reference type (AssetReference, AssetReferenceSprite, AssetReferenceT, etc.).</typeparam>
    /// <example>
    /// How to use:
    /// 1. Create a ScriptableObject of type AssetReferenceAudioClipSO.
    /// 2. Assign an AudioClip Addressable to _assetReference in the Inspector.
    /// 3. Press "ApplyReference" to extract the key.
    /// 4. At runtime, load the asset like this:
    /// _myAudioClipSO.LoadAssetAsync().Completed += handle => _audioClip = handle.Result;
    /// </example>
    public abstract class BaseAssetReferenceSO<TAsset, TAssetReference> : ScriptableObject where TAsset : UnityEngine.Object where TAssetReference : AssetReference
    {
        protected const string NOT_SET_STRING = "X";
        
        [SerializeField] protected TAssetReference _assetReference; // odin sometimes throws two errors when assigning from dropdown but can be ignored
        [SerializeField, ReadOnly] protected string _key = NOT_SET_STRING;
   
        // Relay methods, to be used similarly to AssetReference methods  
        public virtual AsyncOperationHandle<TAsset> LoadAssetAsync() => Addressables.LoadAssetAsync<TAsset>(_key);
        public virtual void ReleaseAsset(AsyncOperationHandle<TAsset> handle) => Addressables.Release(handle);
        
        /// <summary>
        /// Loads the asset safely, catching exceptions and logging a warning if it fails.
        /// Since handle is not returned, can never be actively unloaded.
        /// </summary>
        public async Task<TAsset> LoadAssetSafeAsync()
        {
            try {
                AsyncOperationHandle<TAsset> handle = LoadAssetAsync();
                return await handle.Task;
            }
            catch (Exception e) {
                $"Failed to load asset {_key}: {e}".Log(level: ZMethodsDebug.LogLevel.Error);
                return null;
            }
        }


#if UNITY_EDITOR
        #region InspectorFunctionality
        [SerializeField, ReadOnly] protected bool _isValid;
        
        private void OnValidate()
        {
            // The reference does not have any permanent use and should only be assigned to get the key
            if (_key != NOT_SET_STRING) _assetReference = null;
        }

        /// <summary>
        /// First, assign _assetReference in the inspector.
        /// After calling this, the the Addressables key is extracted, stored and _assetReference is cleared.
        /// Use this in the editor to set up your ScriptableObject.
        /// </summary>
        [Button]
        protected virtual void ApplyReference()
        {
            // Safety checks
            if (_assetReference == null)
            {
                "Asset reference is null. Returning.".Log(level: ZMethodsDebug.LogLevel.Error);
                return;
            }
            if (_key != NOT_SET_STRING)
            {
                "Reference key is already set. If you want to reassign the key, clear the reference first. Returning.".Log(level: ZMethodsDebug.LogLevel.Error);
                return;   
            }
            
                
            // Get the key and clean up Inspector
            _key = GetReferenceEntry(_assetReference).address;
            _assetReference = null;
            _isValid = true;
            
            $"{name}: New key {_key} has successfully been assigned.".Log(level: ZMethodsDebug.LogLevel.Assertion);
        }

        /// <summary>
        /// Clears the current key and asset reference. Can be used to reset the ScriptableObject.
        /// </summary>
        [Button]
        protected virtual void ClearReference()
        {
            _assetReference = null;
            _key = NOT_SET_STRING;
            _isValid = false;
        }
        
        /// <summary>
        /// Validates that the stored key exists in the Addressables system.
        /// Updates the _isValid flag accordingly.
        /// </summary>
        [Button]
        protected virtual void Validate()
        {
            _isValid = false;
            if (string.IsNullOrEmpty(_key) || _key == NOT_SET_STRING)
                $"{name}: Key is not set".Log(level: ZMethodsDebug.LogLevel.Error);
            else if (GetReferenceEntry(_key) == null)
                $"{name}: Key {_key} is not valid".Log(level: ZMethodsDebug.LogLevel.Error);
            else
            {
                $"{name}: Key {_key} is valid".Log(level: ZMethodsDebug.LogLevel.Assertion);
                _isValid = true;
            }
        }
        
        [Button]
        private static void ValidateAllOfSameType()
        {
            // Find all assets of type BaseAssetReferenceSO<TAsset>
            List<BaseAssetReferenceSO<TAsset, TAssetReference>> instances = ZMethodsDebug.FindAllSOAssetsIncludingSubclasses<BaseAssetReferenceSO<TAsset, TAssetReference>>();
            
            // Check if they are valid
            int validCount = 0;
            int invalidCount = 0;
            foreach (BaseAssetReferenceSO<TAsset, TAssetReference> instance in instances.Where(instance => instance != null))
            {
                instance.Validate();

                if (instance._isValid) validCount++;
                else invalidCount++;
            }

            $"Validated {instances.Count} assets: {validCount} valid, {invalidCount} invalid".Log(level: ZMethodsDebug.LogLevel.Info);
        }

        /// <summary>
        /// Finds the Addressable entry corresponding to an AssetReference or a key string.
        /// Optional parameter <paramref name="doSubAssetsOnly"/> can be used e.g. for folder references to search only in sub-assets.
        /// </summary>
        /// <param name="assetReference">The AssetReference to look up.</param>
        /// <param name="doSubAssetsOnly">Whether to search only in sub-assets.</param>
        /// <returns>The AddressableAssetEntry if found; otherwise null.</returns>
        protected static AddressableAssetEntry GetReferenceEntry(AssetReference assetReference, bool doSubAssetsOnly = false) => GetReferenceEntry(entry => entry.guid == assetReference.AssetGUID, doSubAssetsOnly);
        protected static AddressableAssetEntry GetReferenceEntry(string key, bool doSubAssetsOnly = false) => GetReferenceEntry(entry => entry.address == key, doSubAssetsOnly);
        private static AddressableAssetEntry GetReferenceEntry(Func<AddressableAssetEntry, bool> compareFunc, bool doSubAssetsOnly)
        {
            // Safety check
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                "Addressable asset settings are null. Returning null.".Log(level: ZMethodsDebug.LogLevel.Error);
                return null; 
            }
                
            // Find the key
            foreach (AddressableAssetEntry entry in settings.groups.Where(group => group != null).SelectMany(group => group.entries))
            {
                if (doSubAssetsOnly)
                {
                    // Loop over all children
                    foreach (AddressableAssetEntry subEntry in entry.SubAssets.Where(compareFunc))
                        return subEntry;
                }
                else if (compareFunc(entry))
                    return entry;
            }
            
            // Otherwise log error
            "No Addressables entry found. Returning null".Log(level: ZMethodsDebug.LogLevel.Error);
            return null;
        }
        #endregion
#endif
    }
}