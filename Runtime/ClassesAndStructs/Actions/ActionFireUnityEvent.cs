using System;
using UnityEngine;
using UnityEngine.Events;

namespace DeadWrongGames.ZCommon.Actions
{
    [Serializable]
    public struct ActionFireUnityEvent : IInvokable
    {
        [SerializeField] UnityEvent _event;
        
        public void Invoke()
        {
            _event?.Invoke();
        }
    }
}
