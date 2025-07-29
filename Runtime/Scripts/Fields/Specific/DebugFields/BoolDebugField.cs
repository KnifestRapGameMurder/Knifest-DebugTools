using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Knifest.DebugTools
{
    public class BoolDebugField : DebugField<bool>
    {
        [field: Group(Tabs_Events), Tab(Tab_User), SerializeField]
        protected UnityEngine.Events.UnityEvent<bool> OnValueInverted { get; private set; }

        [field: Group(Tabs_Events), Tab(Tab_User), SerializeField]
        protected UnityEngine.Events.UnityEvent OnTrue { get; private set; }

        [field: Group(Tabs_Events), Tab(Tab_User), SerializeField]
        protected UnityEngine.Events.UnityEvent OnFalse { get; private set; }

        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        private UnityEngine.UI.Toggle toggle;

        public override void Init()
        {
            base.Init();
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void OnValueChanged(bool value)
        {
            base.OnValueChanged(value);
            OnValueInverted.Invoke(!value);
            if (value) OnTrue.Invoke();
            else OnFalse.Invoke();
        }

        protected override void SetValueToUI(bool value)
        {
            toggle.isOn = value;
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