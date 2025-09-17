using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DeadWrongGames.ZCommon
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Addressables/ReferenceAudioClip", fileName = "AssetReferenceAudioClip")]
    public class AssetReferenceAudioClipSO : BaseAssetReferenceSO<AudioClip, AssetReferenceT<AudioClip>> { }
}
