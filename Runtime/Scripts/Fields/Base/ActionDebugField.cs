using System;
using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace Knifest.DebugTools
{
    [DeclareTabGroup(Tabs)]
    [DeclareBoxGroup(Tabs_Events, Title = "Events")]
    public abstract class ActionDebugField<T> : MonoBehaviour
    {
        protected const string Tabs = "Tabs";
        protected const string Tab_User = "User";
        protected const string Tab_Dev = "Dev";
        protected const string Tabs_Events = Tabs + "/Events";


        [field: Group(Tabs), Tab(Tab_User), SerializeField]
        protected string Label { get; private set; }

        [Group(Tabs_Events), Tab(Tab_User), SerializeField]
        protected T defaultValue;

        [Group(Tabs_Events), Tab(Tab_User), SerializeField]
        protected UnityEngine.Events.UnityEvent<T> onClick;


        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        protected TMPro.TMP_InputField input;

        [Group(Tabs), Tab(Tab_Dev)] [SerializeField]
        private TMPro.TMP_Text _labelUI;


        protected virtual void OnValidate()
        {
            name = Label;
            if (_labelUI != null) _labelUI.text = Label;
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