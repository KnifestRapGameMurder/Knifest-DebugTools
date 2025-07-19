namespace Knifest.DebugTools
{
    public class BoolActionTestField : ActionTestField<bool>
    {
        protected override bool ParseInput(string input) => bool.Parse(input);
    }
}