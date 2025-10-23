using DeadWrongGames.ZUtils;
using UnityEngine;

namespace DeadWrongGames.ZCommon.Variables
{
    /// <summary>
    /// Float-specific variable SO
    /// Uses approximate float comparison to avoid unnecessary change events from precision noise.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Variables/Float", fileName = "FloatVariable")]
    public class FloatVariableSO : BaseVariableSO<float>
    {
        protected override bool IsSameValue(float newValue)
            => ZMethods.IsSameFloatValue(_value, newValue);
    }
}