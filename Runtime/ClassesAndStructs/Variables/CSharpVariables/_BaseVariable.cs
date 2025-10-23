using System;
using System.Collections.Generic;

namespace DeadWrongGames.ZCommon.Variables
{
    /// <summary>
    /// Base C# class for data-binding-friendly variable containers.
    /// Stores a single value and exposes a change event.
    /// </summary>
    public abstract class BaseVariable<T>
    {
        /// <summary>
        /// Event fired whenever the value changes.
        /// </summary>
        public event Action ValueChanged;

        /// <summary>
        /// Typed accessor for the stored value.
        /// Invokes the <see cref="ValueChanged"/> event when the value changes.
        /// </summary>
        public T Value
        {
            get => _value;
            set {
                if (!IsSameValue(value))
                {
                    _value = value;
                    ValueChanged?.Invoke();
                }
            }
        }
        protected T _value;
        
        // Constructor
        protected BaseVariable(T initialValue)
        {
            _value = initialValue;
        }

        // Determines whether two values should be considered equal.
        // Can be overridden for special comparisons (e.g. approximate float equality).
        protected virtual bool IsSameValue(T newValue) => EqualityComparer<T>.Default.Equals(_value, newValue);
        
        // Implicit conversion for convenience
        public static implicit operator T(BaseVariable<T> f) 
            => f._value;
    }
}
