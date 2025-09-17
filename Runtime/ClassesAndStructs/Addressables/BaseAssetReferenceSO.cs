using System;
using System.Collections.Generic;
using System.Linq;
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

    public abstract class BaseAssetReferenceSO<TAsset, TAssetReference> : ScriptableObject where TAsset : UnityEngine.Object where TAssetReference : AssetReference
    {
        protected const string NOT_SET_STRING = "X";
        
        [SerializeField] protected TAssetReference _assetReference; // odin throws two errors when assigning from dropdown but can be ignored
        [SerializeField, ReadOnly] protected string _key = NOT_SET_STRING;
   
        public virtual AsyncOperationHandle<TAsset> LoadAssetAsync() => Addressables.LoadAssetAsync<TAsset>(_key);
        public virtual void ReleaseAsset(AsyncOperationHandle<TAsset> handle) => Addressables.Release(handle);


#if UNITY_EDITOR
        #region InspectorFunctionality
        [SerializeField, ReadOnly] protected bool _keyIsValid;
        
        private void OnValidate()
        {
            // The reference does not have any permanent use and should only be assigned to get the key
            if (_key != NOT_SET_STRING) _assetReference = null;
        }

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
            _keyIsValid = true;
            
            $"{name}: New key {_key} has successfully been assigned.".Log(level: ZMethodsDebug.LogLevel.Assertion);
        }

        [Button]
        protected virtual void ClearReference()
        {
            _assetReference = null;
            _key = NOT_SET_STRING;
            _keyIsValid = false;
        }
        
        [Button]
        private void Validate()
        {
            _keyIsValid = false;
            if (string.IsNullOrEmpty(_key) || _key == NOT_SET_STRING)
                $"{name}: Key is not set".Log(level: ZMethodsDebug.LogLevel.Warning);
            else if (GetReferenceEntry(_key) == null)
                $"{name}: Key {_key} is not valid".Log(level: ZMethodsDebug.LogLevel.Warning);
            else
            {
                $"{name}: Key {_key} is valid".Log(level: ZMethodsDebug.LogLevel.Assertion);
                _keyIsValid = true;
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

                if (instance._keyIsValid) validCount++;
                else invalidCount++;
            }

            $"Validated {instances.Count} assets: {validCount} valid, {invalidCount} invalid".Log(level: ZMethodsDebug.LogLevel.Info);
        }

        protected static AddressableAssetEntry GetReferenceEntry(AssetReference assetReference) => GetReferenceEntry(entry => entry.guid == assetReference.AssetGUID);
        private static AddressableAssetEntry GetReferenceEntry(string key) => GetReferenceEntry(entry => entry.address == key);
        private static AddressableAssetEntry GetReferenceEntry(Func<AddressableAssetEntry, bool> compareFunc)
        {
            // Safety check
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                "Addressable asset settings are null. Returning null.".Log(level: ZMethodsDebug.LogLevel.Error);
                return null; 
            }
                
            // Find the key
            AddressableAssetEntry referenceEntry = settings.groups.Where(group => group != null).SelectMany(group => group.entries).FirstOrDefault(compareFunc);
            if (referenceEntry == null)
            {
                "No Addressables entry found. Returning null".Log(level: ZMethodsDebug.LogLevel.Error);
                return null;
            }
            
            // Return the key
            return referenceEntry;
        }
        #endregion
#endif
    }
}