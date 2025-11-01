using Sirenix.OdinInspector;
using UnityEngine;

namespace DeadWrongGames.ZCommon.Actions
{
    public class ActionOnAwake : SerializedMonoBehaviour
    {
        [SerializeField] IInvokable _action;
        
        private void Awake()
        {
            _action?.Invoke();
        }
    }
}
