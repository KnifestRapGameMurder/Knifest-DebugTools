using UnityEngine;

namespace Knifest.DebugTools
{
    public class IntActionTestField : ActionTestField<int>
    {
        protected override int ParseInput(string input) => int.Parse(input);
    }
}