using TriInspector;
using UnityEngine;

namespace Knifest.DebugTools.DebugFields
{
    public class FloatRangeDebugField : DebugField<float>
    {
        [Group(Tabs), Tab(Tab_User), SerializeField]
        private float _minValue, _maxValue;

        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        private UnityEngine.UI.Slider _slider;

        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        private string _valueFormat = "0.000";

        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        private TMPro.TMP_Text _valueUI;

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

        protected override void OnValueChanged(float value)
        {
            base.OnValueChanged(value);
            _valueUI.text = Value.ToString(_valueFormat);
        }

        protected override void SetValueToUI(float value)
        {
            _slider.value = Mathf.InverseLerp(_minValue, _maxValue, value);
            _valueUI.text = value.ToString(_valueFormat);
        }

        protected override float GetPrefsValue()
        {
            return LoadFloat();
        }

        protected override void SavePrefsValue()
        {
            SaveFloat(Value);
        }

        private void TransformValue(float t)
        {
            float value = Mathf.Lerp(_minValue, _maxValue, t);
            OnValueChanged(value);
        }
    }
}