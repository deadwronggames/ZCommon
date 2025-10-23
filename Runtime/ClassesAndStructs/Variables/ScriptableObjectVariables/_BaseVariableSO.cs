using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeadWrongGames.ZCommon.Variables
{
    /// <summary>
    /// Non-generic base class for variable ScriptableObjects.
    /// 
    /// Exists so that derived typed variable assets (e.g. FloatVariableSO, IntVariableSO)
    /// can all be assigned to a single <see cref="BaseVariableSO"/> reference in the Inspector.
    /// Unity cannot serialize generic base classes directly, so this untyped layer enables
    /// polymorphic assignment in serialized fields.
    ///
    /// Only exposes an object-based API (<see cref="ValueAsObject"/>) which usually requires somewhat awkward casting. Therefore, if possible, the generic class (see below) should be used.
    /// </summary>
    public abstract class BaseVariableSO : ScriptableObject
    {
        /// <summary>
        /// Fired whenever the stored value changes.
        /// </summary>
        public event Action ValueChanged;
        protected void InvokeValueChangedEvent() => ValueChanged?.Invoke(); // Useful for derived classes that want to trigger change notifications

        /// <summary>
        /// Untyped accessor for the stored value.
        /// This bypasses compile-time type safety and may require casting.
        /// Invokes the <see cref="ValueChanged"/> event when the value changes.
        /// </summary>
        public object ValueAsObject
        {
            get => _valueAsObjectBacking;
            set {
                if (_valueAsObjectBacking != value)
                {
                    _valueAsObjectBacking = value;
                    ValueChanged?.Invoke();
                }
            }
        }

        // Abstract backing property implemented by typed subclass (see below).
        protected abstract object _valueAsObjectBacking { get; set; }
    }
    

    /// <summary>
    /// Generic base class for typed variable ScriptableObjects.
    /// Stores and manages a single value of type T, automatically firing change events
    /// when the value is modified.
    ///
    /// Inherit from this class to create type-safe variable assets while still
    /// maintaining compatibility with the non-generic <see cref="BaseVariableSO"/> layer.
    /// </summary>
    public abstract class BaseVariableSO<T> : BaseVariableSO
    {
        [SerializeField] protected T _value;

        /// <summary>
        /// Typed accessor for the stored value.
        /// Invokes event when the value changes.
        /// </summary>
        public T Value
        {
            get => _value;
            set {
                if (!IsSameValue(value))
                {
                    _value = value;
                    InvokeValueChangedEvent();
                }
            }
        }

        // Bridges the typed and untyped systems by exposing the value as an object.
        // Used by the non-generic base for dynamic access.
        protected override object _valueAsObjectBacking
        {
            get => _value;
            set => _value = (T)value;
        }

        // Determines whether two values should be considered equal.
        // Can be overridden for special comparisons (e.g. approximate float equality).
        protected virtual bool IsSameValue(T newValue)
            => EqualityComparer<T>.Default.Equals(_value, newValue);

        /// <summary>
        /// Allows implicit conversion from a variable asset to its stored value.
        /// Example:
        /// <code>float f = myFloatVariable;</code>
        /// </summary>
        public static implicit operator T(BaseVariableSO<T> reference)
            => reference.Value;
    }
}

