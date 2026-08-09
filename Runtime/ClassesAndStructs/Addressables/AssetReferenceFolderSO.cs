using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings;
#endif


namespace DeadWrongGames.ZCommon
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Addressables/ReferenceFolder", fileName = "AssetReferenceFolder")]
    public class AssetReferenceFolderSO : BaseAssetReferenceSO<UnityEngine.Object, AssetReference>
    {
        [SerializeField, ReadOnly] List<string> _subKeys = new();
        
        // Load all assets in folder
        public AsyncOperationHandle<IList<TAsset>> LoadAssetsAsync<TAsset>(Action<TAsset> assetCallback = null) where TAsset : UnityEngine.Object
        {
            AsyncOperationHandle<IList<TAsset>> handle = Addressables.LoadAssetsAsync(_subKeys, assetCallback ?? (_ => { }), Addressables.MergeMode.Union);
            return handle;
        }
        
        // "Disable" the not applicable methods
        public override AsyncOperationHandle<UnityEngine.Object> LoadAssetAsync()
        {
            throw new InvalidOperationException($"{name}: Cannot load a single asset from a folder. Use LoadAssetsAsync (plural) instead.");
        }
        
        public override void ReleaseAsset(AsyncOperationHandle<UnityEngine.Object> handle)
        {
            throw new InvalidOperationException($"{name}: Cannot release a single asset. Use ReleaseAssets (plural) instead..");
        }
        
#if UNITY_EDITOR
        #region InspectorFunctionalityOverrides
        
        /// <summary>
        /// Can also be used, to reapply the sub keys in case assets have changed (sometimes a bit quirky, if in doubt, just press a few times or else, just clear the reference and start over.)
        /// </summary>
        protected override void ApplyReference()
        {
            // Safety checks and get folder entry from either reference or key
            if (_assetReference == null && _key == NOT_SET_STRING)
            {
                Debug.LogWarning("Folder reference and key are null. Returning.");
                return;
            }

            AddressableAssetEntry folderEntry = (_key != NOT_SET_STRING) ? GetReferenceEntry(_key) : GetReferenceEntry(_assetReference); // Sometimes _assetReference weirdly thinks that it is not null when it should be. But does not make much of a difference.
            if (folderEntry == null)  
            {
                Debug.LogError("No Addressables entry found. Returning.");
                return;
            }

            if (!folderEntry.IsFolder)
            {
                Debug.LogError($"{folderEntry.address} is not a folder entry. Returning.");
                return;
            }
            
            // Clear and repopulate subKeys
            _subKeys.Clear();
            foreach (AddressableAssetEntry child in folderEntry.SubAssets)
                _subKeys.Add(child.address);

            // Cleanup
            _key = folderEntry.address;
            _assetReference = null;
            _isValid = _subKeys.Count > 0;
            
            Debug.Log($"{name}: Added {_subKeys.Count} sub-keys from folder {folderEntry.address}");
        }

        protected override void ClearReference()
        {
            _subKeys.Clear();
            base.ClearReference();
        }

        protected override void Validate()
        {
            base.Validate();
            AddressableAssetEntry folderEntry = GetReferenceEntry(_key);
            if (folderEntry.SubAssets == null)
            {
                Debug.LogError($"{name}: Sub-assets are corrupted.");
                _isValid = false;
            }
            else foreach (AddressableAssetEntry child in folderEntry.SubAssets.Where(child => child == null || GetReferenceEntry(child.address, doSubAssetsOnly: true) == null))
            {
                Debug.LogError($"{name}: Sub-key {child?.address} is not valid");
                _isValid = false;
            }
            if (_isValid) Debug.Log($"{name}: All sub-keys are valid");
        }
        #endregion
#endif
    }
}