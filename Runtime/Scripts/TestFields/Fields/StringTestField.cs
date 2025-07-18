﻿using UnityEngine;

namespace Knifest.DebugTools
{
    public class StringTestField : TestField<string>
    {
        [SerializeField] private TMPro.TMP_InputField _input;

        public override void Init()
        {
            base.Init();
            _input.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void SetValueToUI(string value)
        {
            _input.text = value;
        }

        protected override string GetPrefsValue()
        {
            return PlayerPrefs.GetString(Label);
        }

        protected override void SavePrefsValue()
        {
            PlayerPrefs.SetString(Label, Value);
        }
    }
}