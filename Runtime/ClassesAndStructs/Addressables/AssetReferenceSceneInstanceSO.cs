using System.Linq;
using DeadWrongGames.ZUtils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace DeadWrongGames.ZCommon
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Addressables/ReferenceSceneInstance", fileName = "AssetReferenceSceneInstance")]
    public class AssetReferenceSceneInstanceSO : BaseAssetReferenceSO<Object, AssetReference>
    {
        // AssetReference methods specifically for scenes
        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(LoadSceneMode loadMode, bool activateOnLoad) => Addressables.LoadSceneAsync(_key, loadMode, activateOnLoad);
        public AsyncOperationHandle<SceneInstance> UnLoadScene(AsyncOperationHandle<SceneInstance> handle) => Addressables.UnloadSceneAsync(handle);
        
        // "Disable" the not applicable methods
        public override AsyncOperationHandle<Object> LoadAssetAsync()
        {
            $"{name}: Can not load a SceneInstance reference. Returning default.".Log(level: ZMethodsDebug.LogLevel.Warning);
            return default;
        }
        
        public override void ReleaseAsset(AsyncOperationHandle<Object> handle)
        {
            $"{name}: Can not release a SceneInstance reference. Returning.".Log(level: ZMethodsDebug.LogLevel.Warning);
        }
    }
}