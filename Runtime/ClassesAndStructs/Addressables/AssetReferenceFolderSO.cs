using System.Collections.Generic;
using System.Linq;
using DeadWrongGames.ZUtils;
using Sirenix.OdinInspector;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace DeadWrongGames.ZCommon
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Addressables/ReferenceFolder", fileName = "AssetReferenceFolder")]
    public class AssetReferenceFolderSO : BaseAssetReferenceSO<Object, AssetReference>
    {
        [SerializeField, ReadOnly] List<string> _subKeys = new();
        
        // Load all assets in folder
        public AsyncOperationHandle<IList<TAsset>> LoadAssetsAsync<TAsset>() where TAsset : Object
        {
            AsyncOperationHandle<IList<TAsset>> handle = Addressables.LoadAssetsAsync<TAsset>(_subKeys, _ => { }, Addressables.MergeMode.Union);
            return handle;
        }
        
        // "Disable" the not applicable methods
        public override AsyncOperationHandle<Object> LoadAssetAsync()
        {
            $"{name}: Cannot load single asset from a folder. Use LoadAssetsAsync instead. Returning default.".Log(level: ZMethodsDebug.LogLevel.Error);
            return default;
        }
        
        public override void ReleaseAsset(AsyncOperationHandle<Object> handle)
        {
            "{name}: Cannot release a single asset. Use ReleaseAssets instead. Returning.".Log(level: ZMethodsDebug.LogLevel.Error);
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
                "Folder reference and key are null. Returning.".Log(level: ZMethodsDebug.LogLevel.Warning);
                return;
            }

            AddressableAssetEntry folderEntry = (_key != NOT_SET_STRING) ? GetReferenceEntry(_key) : GetReferenceEntry(_assetReference); // Sometimes _assetReference weirdly thinks that it is not null when it should be. But does not make much of a difference.
            if (folderEntry == null)  
            {
                "No Addressables entry found. Returning.".Log(level: ZMethodsDebug.LogLevel.Error);
                return;
            }

            if (!folderEntry.IsFolder)
            {
                $"{folderEntry.address} is not a folder entry. Returning.".Log(level: ZMethodsDebug.LogLevel.Error);
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
            
            $"{name}: Added {_subKeys.Count} sub keys from folder {folderEntry.address}".Log(level: ZMethodsDebug.LogLevel.Assertion);
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
                $"{name}: Sub assets are corrupted.".Log(level: ZMethodsDebug.LogLevel.Error);
                _isValid = false;
            }
            else foreach (AddressableAssetEntry child in folderEntry.SubAssets.Where(child => child == null || GetReferenceEntry(child.address, doSubAssetsOnly: true) == null))
            {
                $"{name}: Sub key {child?.address} is not valid".Log(level: ZMethodsDebug.LogLevel.Error);
                _isValid = false;
            }
            if (_isValid) $"{name}: All sub keys are valid".Log(level: ZMethodsDebug.LogLevel.Assertion);
        }
        #endregion
#endif
    }
}