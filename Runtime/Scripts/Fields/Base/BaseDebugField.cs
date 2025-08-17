using System;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Knifest.DebugTools.DebugFields
{
    [DeclareTabGroup(Tabs)]
    [DeclareBoxGroup(Tabs_Events, Title = "Events")]
    public abstract class BaseDebugField<T> : MonoBehaviour
    {
        protected const string Tabs = "Tabs";
        protected const string Tab_User = "User";
        protected const string Tab_Dev = "Dev";
        protected const string Tabs_Events = Tabs + "/Events";

        [field: Group(Tabs), Tab(Tab_User), SerializeField]
        protected string Label { get; private set; }

        [field: Group(Tabs), Tab(Tab_User), SerializeField]
        protected T DefaultValue { get; set; }

        [FormerlySerializedAs("_labelUI")] [Group(Tabs), Tab(Tab_Dev)] [SerializeField]
        protected TMPro.TMP_Text labelUI;

        [Group(Tabs), Tab(Tab_Dev), SerializeField, ReadOnly]
        private string saveKey;

        public string SaveKey => saveKey;

        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(saveKey)) saveKey = Guid.NewGuid().ToString();

            name = Label;
            if (labelUI != null) labelUI.text = Label;
            SetValueToUI(DefaultValue);
        }
        
        protected abstract void SetValueToUI(T value);
    }
}