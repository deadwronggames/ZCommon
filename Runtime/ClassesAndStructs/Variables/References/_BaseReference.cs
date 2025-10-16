using UnityEngine;

namespace DeadWrongGames.ZCommon.Variables
{
    /// <summary>
    /// Base MonoBehaviour for reference wrappers that expose a value as an object.
    /// Enables polymorphic handling of value references (float, int, etc.).
    /// </summary>
    public abstract class BaseReference : MonoBehaviour
    {
        public abstract object ValueAsObject { get; }
    }
}