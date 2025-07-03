using UnityEngine;

namespace Knifest.DebugTools
{
    public class IntRangeTestField : TestField<int>
    {
        [SerializeField] private int _minValue, _maxValue;
        [SerializeField] private UnityEngine.UI.Slider _slider;
        [SerializeField] private string _valueFormat = "0.000";
        [SerializeField] private TMPro.TMP_Text _valueUI;

        protected override void OnValidate()
        {
            DefaultValue = Mathf.Clamp(DefaultValue, _minValue, _maxValue);
            base.OnValidate();
        }

        public override void Init()
        {
            base.Init();
            _slider.onValueChanged.AddListener(TransformValue);
        }

        protected override void OnValueChanged(int value)
        {
            base.OnValueChanged(value);
            SetValueToUI(Value);
        }

        protected override void SetValueToUI(int value)
        {
            _slider.value = Mathf.InverseLerp(_minValue, _maxValue, value);
            _valueUI.text = value.ToString(_valueFormat);
        }

        protected override int GetPrefsValue()
        {
            return PlayerPrefs.GetInt(Label);
        }

        protected override void SavePrefsValue()
        {
            PlayerPrefs.SetInt(Label, Value);
        }

        private void TransformValue(float t)
        {
            float value = Mathf.Lerp(_minValue, _maxValue, t);
            OnValueChanged((int)value);
        }

    }
}