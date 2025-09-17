using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DeadWrongGames.ZCommon
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Addressables/ReferenceSprite", fileName = "AssetReferenceSprite")]
    public class AssetReferenceSpriteSO : BaseAssetReferenceSO<Sprite, AssetReferenceSprite> { }
}