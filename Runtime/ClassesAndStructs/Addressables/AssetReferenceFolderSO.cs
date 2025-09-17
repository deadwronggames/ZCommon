using System.Collections.Generic;
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
        
        public AsyncOperationHandle<IList<TAsset>> LoadAssetsAsync<TAsset>() where TAsset : Object
        {
            AsyncOperationHandle<IList<TAsset>> handle = Addressables.LoadAssetsAsync<TAsset>(_subKeys, _ => { }, Addressables.MergeMode.Union);
            return handle;
        }

        public void ReleaseAssets(AsyncOperationHandle<IList<Object>> handle)
        {
            handle.Release(); // Is this correct?
        }
        
        
        // "Disable" the not applicable methods
        public override AsyncOperationHandle<Object> LoadAssetAsync()
        {
            $"{name}: Cannot load single asset from a folder. Use LoadAssetsAsync instead. Returning default.".Log(level: ZMethodsDebug.LogLevel.Warning);
            return default;
        }
        
        public override void ReleaseAsset(AsyncOperationHandle<Object> handle)
        {
            "{name}: Cannot release a single asset. Use ReleaseAssets instead. Returning.".Log(level: ZMethodsDebug.LogLevel.Warning);
        }
        
#if UNITY_EDITOR
        protected override void ApplyReference()
        {
            if (_assetReference == null)
            {
                "Folder reference is null. Returning.".Log(level: ZMethodsDebug.LogLevel.Warning);
                return;
            }

            AddressableAssetEntry folderEntry = GetReferenceEntry(_assetReference);
            if (folderEntry == null)
            {
                "No Addressables entry found for folder reference".Log(level: ZMethodsDebug.LogLevel.Warning);
                return;
            }

            // Make sure it really is a folder
            if (!folderEntry.IsFolder)
            {
                $"{folderEntry.address} is not a folder entry".Log(level: ZMethodsDebug.LogLevel.Warning);
                return;
            }

            // Clear and repopulate subKeys
            _subKeys.Clear();

            foreach (AddressableAssetEntry child in folderEntry.SubAssets) // might be empty
            {
                _subKeys.Add(child.address);
            }

            // foreach (AddressableAssetEntry child in folderEntry.children) // proper way to get children
            //     _subKeys.Add(child.address);

            $"{name}: Added {_subKeys.Count} sub keys from folder {folderEntry.address}".Log(level: ZMethodsDebug.LogLevel.Assertion);

            // clear the ref itself
            _assetReference = null;
            _key = NOT_SET_STRING; // folder doesn't have a single key
            _keyIsValid = _subKeys.Count > 0;
        }

        protected override void ClearReference()
        {
            _subKeys.Clear();
            base.ClearReference();
        }
#endif
    }
}