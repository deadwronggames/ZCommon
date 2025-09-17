using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DeadWrongGames.ZCommon
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Addressables/ReferenceGameObject", fileName = "AssetReferenceGameObject")]
    public class AssetReferenceGameObjectSO : BaseAssetReferenceSO<GameObject, AssetReferenceGameObject>
    {
        public AsyncOperationHandle<GameObject> InstantiateAsync() => Addressables.InstantiateAsync(_key);
        public void ReleaseInstance(GameObject instance) => Addressables.ReleaseInstance(instance);
    }
}