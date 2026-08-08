using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DeadWrongGames.ZCommon
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Addressables/ReferenceFontAsset", fileName = "AssetReferenceFontAsset")]
    public class AssetReferenceFontAssetSO : BaseAssetReferenceSO<TMP_FontAsset, AssetReferenceT<TMP_FontAsset>> { }
}