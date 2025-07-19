using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Knifest.DebugTools
{
    public class StringDebugField : DebugField<string>
    {
        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        private TMPro.TMP_InputField input;

        public override void Init()
        {
            base.Init();
            input.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void SetValueToUI(string value)
        {
            input.text = value;
        }

        protected override string GetPrefsValue()
        {
            return LoadString();
        }

        protected override void SavePrefsValue()
        {
            SaveString(Value);
        }
    }
}