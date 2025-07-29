using System;
using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace Knifest.DebugTools
{
    public abstract class ActionDebugField<T> : BaseDebugField
    {
        [Group(Tabs_Events), Tab(Tab_User), SerializeField]
        protected T defaultValue;

        [Group(Tabs_Events), Tab(Tab_User), SerializeField]
        protected UnityEngine.Events.UnityEvent<T> onClick;


        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        protected TMPro.TMP_InputField input;


        protected virtual void OnValidate()
        {
            name = Label;
            if (labelUI != null) labelUI.text = Label;
            SetValueToUI(defaultValue);
        }

        [UsedImplicitly]
        public virtual void OnButtonClick()
        {
            onClick?.Invoke(ParseInput(input.text));
        }

        protected abstract T ParseInput(string value);
        protected abstract void SetValueToUI(T value);
    }
}