using UnityEngine;

namespace Knifest.DebugTools
{
    public class BoolTestField : TestField<bool>
    {
        [SerializeField] private UnityEngine.UI.Toggle _toggle;

        public override void Init()
        {
            base.Init();
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void SetValueToUI(bool value)
        {
            _toggle.isOn = value;
        }

        protected override bool GetPrefsValue()
        {
            return PlayerPrefs.GetInt(Label) != 0;
        }

        protected override void SavePrefsValue()
        {
            PlayerPrefs.SetInt(Label, Value ? 1 : 0);
        }
    }
}