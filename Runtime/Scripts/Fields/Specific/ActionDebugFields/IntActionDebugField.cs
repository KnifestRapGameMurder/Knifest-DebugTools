using UnityEngine;

namespace Knifest.DebugTools
{
    public class IntActionDebugField : ActionDebugField<int>
    {
        protected override int ParseInput(string value) => int.Parse(value);
        protected override void SetValueToUI(int value) => input.text = value.ToString();
    }
}