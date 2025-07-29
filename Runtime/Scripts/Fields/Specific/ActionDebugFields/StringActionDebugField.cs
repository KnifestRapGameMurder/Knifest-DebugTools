using UnityEngine;

namespace Knifest.DebugTools
{
    public class StringActionDebugField : ActionDebugField<string>
    {
        protected override string ParseInput(string value) => value;
        protected override void SetValueToUI(string value) => input.text = value;
    }
}