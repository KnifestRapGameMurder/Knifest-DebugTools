using UnityEngine;

namespace Knifest.DebugTools
{
    public class FloatActionTestField : ActionTestField<float>
    {
        protected override float ParseInput(string input) => float.Parse(input);
    }
}