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
            return LoadBool();
        }

        protected override void SavePrefsValue()
        {
            SaveBool(Value);
        }
    }
}