using UnityEngine;

namespace Knifest.DebugTools.DebugFields
{
    public class FloatActionDebugField : ActionDebugField<float>
    {
        protected override float ParseInput(string value) => float.Parse(value);
        protected override void SetValueToUI(float value) => input.text = value.ToString();
    }
}