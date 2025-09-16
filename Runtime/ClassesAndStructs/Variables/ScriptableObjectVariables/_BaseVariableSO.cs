using System;
using UnityEngine;

namespace DeadWrongGames.ZCommon.Variables
{
    public abstract class BaseVariableSO : ScriptableObject
    {
        public event Action ValueChanged; // other classes that want to data-bind to the value can subscribe to this

        public object ValueAsObject
        {
            get => _valueBackingField;
            set {
                if (_valueBackingField != value)
                {
                    _valueBackingField = value;
                    ValueChanged?.Invoke();
                }
            }
        }

        protected abstract object _valueBackingField { get; set; }
    }
}