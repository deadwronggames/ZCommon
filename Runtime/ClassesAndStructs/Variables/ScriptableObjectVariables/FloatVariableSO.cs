using UnityEngine;

namespace DeadWrongGames.ZCommon.Variables
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Variables/Float", fileName = "FloatVariable")]
    public class FloatVariableSO : BaseVariableSO
    {
        [SerializeField] float _value;
        
        public float Value => _value;
        protected override object _valueBackingField {
            get => _value; 
            set => _value = (float)value;
        }
        
        public static implicit operator float(FloatVariableSO reference)
            => reference.Value;
    }
}