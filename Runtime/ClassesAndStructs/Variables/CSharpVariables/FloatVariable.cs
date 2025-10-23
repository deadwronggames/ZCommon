
using DeadWrongGames.ZUtils;

namespace DeadWrongGames.ZCommon.Variables
{
    /// <summary>
    /// Runtime-friendly float container that supports data binding.
    /// Uses approximate float comparison to avoid unnecessary change events from precision noise.
    /// </summary>
    public class FloatVariable : BaseVariable<float>
    {
        public FloatVariable(float initialValue) : base(initialValue) { }

        protected override bool IsSameValue(float newValue) => ZMethods.IsSameFloatValue(_value, newValue);
    }
}