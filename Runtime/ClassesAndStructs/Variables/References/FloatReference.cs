using UnityEngine;

namespace DeadWrongGames.ZCommon.Variables
{
    /// <summary>
    /// Reference wrapper for float values.
    /// Can use either a constant value or a ScriptableObject variable.
    /// </summary>
    public class FloatReference : BaseReference
    {
        [SerializeField] FloatVariableSO _variable;
        [SerializeField] float _constantValue;
        [SerializeField] bool _doAlwaysUseConstValue;
        
        public float Value => (_doAlwaysUseConstValue) ? _constantValue : (_variable != null) ? _variable.Value : _constantValue;
        public override object ValueAsObject => Value;

        public static implicit operator float(FloatReference reference)
            => reference.Value;
    }
}