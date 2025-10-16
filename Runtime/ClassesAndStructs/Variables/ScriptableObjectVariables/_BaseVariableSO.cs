using System;
using UnityEngine;

namespace DeadWrongGames.ZCommon.Variables
{
    /// <summary>
    /// Base ScriptableObject for data-binding-friendly variable containers.
    /// Stores a single value and exposes a change event.
    /// </summary>
    public abstract class BaseVariableSO : ScriptableObject
    {
        public event Action ValueChanged; // other classes that want to data-bind to the value can subscribe to this

        /// <summary>
        /// Accessor for the stored value as an object.
        /// Invokes ValueChanged when modified.
        /// </summary>
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

        protected abstract object _valueBackingField { get; set; } // Implemented by derived classes to store the underlying typed value.
    }
}