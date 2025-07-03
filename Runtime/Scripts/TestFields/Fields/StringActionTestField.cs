using UnityEngine;

namespace Knifest.DebugTools
{
    public class StringActionTestField : ActionTestField<string>
    {
        protected override string ParseInput(string input) => input;
    }
}