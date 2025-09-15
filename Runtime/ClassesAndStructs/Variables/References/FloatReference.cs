using UnityEngine;

namespace DeadWrongGames.ZCommon.Variables
{
    public class FloatReference : BaseReference
    {
        [SerializeField] FloatVariable _variable;
        [SerializeField] float _constantValue;
        [SerializeField] bool _doAlwaysUseConstValue;
        
        public float Value => (_doAlwaysUseConstValue) ? _constantValue : (_variable != null) ? _variable.Value : _constantValue;
        public override object ValueAsObject => Value;

        public static implicit operator float(FloatReference reference)
            => reference.Value;
    }
}