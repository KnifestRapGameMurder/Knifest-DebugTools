using TriInspector;
using UnityEngine;

namespace Knifest.DebugTools
{
    public class BoolActionDebugField : ActionDebugField<bool>
    {
        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        private UnityEngine.UI.Toggle toggle;

        public override void OnButtonClick()
        {
            onClick?.Invoke(toggle.isOn);
        }

        protected override bool ParseInput(string value) => bool.Parse(value);
        protected override void SetValueToUI(bool value) => toggle.isOn = value;
    }
}