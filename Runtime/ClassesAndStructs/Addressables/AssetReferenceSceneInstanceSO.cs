using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace DeadWrongGames.ZCommon
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Addressables/ReferenceSceneInstance", fileName = "AssetReferenceSceneInstance")]
    public class AssetReferenceSceneInstanceSO : BaseAssetReferenceSO<UnityEngine.Object, AssetReference>
    {
        // AssetReference methods specifically for scenes
        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(LoadSceneMode loadMode, bool activateOnLoad) => Addressables.LoadSceneAsync(_key, loadMode, activateOnLoad);
        public AsyncOperationHandle<SceneInstance> UnLoadScene(AsyncOperationHandle<SceneInstance> handle) => Addressables.UnloadSceneAsync(handle);
        
        // "Disable" the not applicable methods
        public override AsyncOperationHandle<UnityEngine.Object> LoadAssetAsync()
        {
            throw new InvalidOperationException($"{name}: Can not load a SceneInstance reference.");
        }
        
        public override void ReleaseAsset(AsyncOperationHandle<UnityEngine.Object> handle)
        {
            throw new InvalidOperationException($"{name}: Can not release a SceneInstance reference.");
        }
    }
}