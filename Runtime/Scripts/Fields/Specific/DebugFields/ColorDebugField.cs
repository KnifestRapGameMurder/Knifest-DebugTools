using Knifest.DebugTools.InputFields;
using TriInspector;
using UnityEngine;

namespace Knifest.DebugTools.DebugFields
{
    public class ColorDebugField : DebugField<Color>
    {
        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        private ColorInputField input;

        public override void Init()
        {
            base.Init();
            input.ValueChanged += OnValueChanged;
        }

        protected override void SetValueToUI(Color value)
        {
            input.Color = value;
        }

        protected override Color GetPrefsValue()
        {
            return LoadColor();
        }

        protected override void SavePrefsValue()
        {
            SaveColor(Value);
        }

        protected void SaveColor(Color value)
        {
            var htmlString = ColorUtility.ToHtmlStringRGBA(value);
            // Debug.Log(htmlString);
            SaveString(htmlString);
        }

        protected Color LoadColor()
        {
            string loadedString = LoadString();
            // Debug.Log(loadedString);
            bool parsed = ColorUtility.TryParseHtmlString('#' + loadedString, out var color);
            return parsed ? color : DefaultValue;
        }
    }
}