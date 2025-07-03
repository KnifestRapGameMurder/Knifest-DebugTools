using UnityEngine;

namespace Knifest.DebugTools
{
    public class IntTestField : TestField<int>
    {
        [SerializeField] private TMPro.TMP_InputField _input;

        public override void Init()
        {
            _input.contentType = TMPro.TMP_InputField.ContentType.IntegerNumber;
            base.Init();
            _input.onValueChanged.AddListener(ParseValue);
        }

        protected override void SetValueToUI(int value)
        {
            _input.text = value.ToString();
        }

        protected override int GetPrefsValue()
        {
            return PlayerPrefs.GetInt(Label);
        }

        protected override void SavePrefsValue()
        {
            PlayerPrefs.SetInt(Label, Value);
        }

        private void ParseValue(string str)
        {
            if (int.TryParse(str, out var value)) OnValueChanged(value);
            else print($"Can not parse '{str}' to float");
        }
    }
}