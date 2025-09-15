using UnityEngine;

namespace DeadWrongGames.ZCommon.Variables
{
    public abstract class BaseReference : MonoBehaviour
    {
        public abstract object ValueAsObject { get; }
    }
}