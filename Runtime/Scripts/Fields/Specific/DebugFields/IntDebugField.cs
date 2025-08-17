using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Knifest.DebugTools.DebugFields
{
    public class IntDebugField : DebugField<int>
    {
        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        private TMPro.TMP_InputField input;

        public override void Init()
        {
            input.contentType = TMPro.TMP_InputField.ContentType.IntegerNumber;
            base.Init();
            input.onValueChanged.AddListener(ParseValue);
        }

        protected override void SetValueToUI(int value)
        {
            input.text = value.ToString();
        }

        protected override int GetPrefsValue()
        {
            return LoadInt();
        }

        protected override void SavePrefsValue()
        {
            SaveInt(Value);
        }

        private void ParseValue(string str)
        {
            if (int.TryParse(str, out var value)) OnValueChanged(value);
            else print($"Can not parse '{str}' to float");
        }
    }
}