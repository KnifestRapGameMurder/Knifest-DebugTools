using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Knifest.DebugTools.DebugFields
{
    public class FloatDebugField : DebugField<float>
    {
        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        private TMPro.TMP_InputField input;

        public override void Init()
        {
            input.contentType = TMPro.TMP_InputField.ContentType.DecimalNumber;
            base.Init();
            input.onValueChanged.AddListener(ParseValue);
        }

        protected override void SetValueToUI(float value)
        {
            input.text = value.ToString();
        }

        protected override float GetPrefsValue()
        {
            return LoadFloat();
        }

        protected override void SavePrefsValue()
        {
            SaveFloat(Value);
        }

        private void ParseValue(string str)
        {
            if (float.TryParse(str, out var value)) OnValueChanged(value);
            else print($"Can not parse '{str}' to float");
        }
    }
}