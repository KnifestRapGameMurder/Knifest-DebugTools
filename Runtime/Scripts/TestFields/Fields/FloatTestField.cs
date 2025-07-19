using UnityEngine;

namespace Knifest.DebugTools
{
    public class FloatTestField : TestField<float>
    {
        [SerializeField] private TMPro.TMP_InputField _input;

        public override void Init()
        {
            _input.contentType = TMPro.TMP_InputField.ContentType.DecimalNumber;
            base.Init();
            _input.onValueChanged.AddListener(ParseValue);
        }

        protected override void SetValueToUI(float value)
        {
            _input.text = value.ToString();
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