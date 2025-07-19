using System;
using TriInspector;
using UnityEngine;

namespace Knifest.DebugTools
{
    [DeclareTabGroup(Tabs)]
    [DeclareBoxGroup(Tabs_Events, Title = "Events")]
    public abstract class DebugField<T> : MonoBehaviour, IDebugField
    {
        protected const string Tabs = "Tabs";
        protected const string Tab_User = "User";
        protected const string Tab_Dev = "Dev";
        protected const string Tabs_Events = Tabs + "/Events";


        [field: Group(Tabs), Tab(Tab_User), SerializeField]
        protected string Label { get; private set; }

        [field: Group(Tabs), Tab(Tab_User), SerializeField]
        protected T DefaultValue { get; set; }

        [field: Group(Tabs_Events), Tab(Tab_User), SerializeField]
        protected UnityEngine.Events.UnityEvent<T> OnValue { get; private set; }


        [Group(Tabs), Tab(Tab_Dev), SerializeField, ReadOnly]
        private string saveKey;

        [Group(Tabs), Tab(Tab_Dev)] [SerializeField]
        private TMPro.TMP_Text _labelUI;


        public string SaveKey => saveKey;
        public T Value { get; protected set; }

        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(saveKey)) saveKey = Guid.NewGuid().ToString();

            name = Label;
            if (_labelUI != null) _labelUI.text = Label;
            SetValueToUI(DefaultValue);
        }

        public virtual void Init()
        {
            Value = DefaultValue;

            if (IsSaved())
            {
                Value = GetPrefsValue();
                SetValueToUI(Value);
            }

            OnValue?.Invoke(Value);
        }

        public void ResetToDefault()
        {
            OnValueChanged(DefaultValue);
            SetValueToUI(Value);
        }

        protected virtual void OnValueChanged(T value)
        {
            Value = value;
            OnValue.Invoke(Value);
            SavePrefsValue();
        }

        protected abstract void SetValueToUI(T value);
        protected abstract T GetPrefsValue();
        protected abstract void SavePrefsValue();

        #region SaveLoad

        protected bool IsSaved() => PlayerPrefs.HasKey(SaveKey);
        protected void SaveBool(bool value) => PlayerPrefs.SetInt(SaveKey, value ? 1 : 0);
        protected bool LoadBool(bool defaultValue = false) => PlayerPrefs.GetInt(SaveKey, defaultValue ? 1 : 0) != 0;
        protected void SaveInt(int value) => PlayerPrefs.SetInt(SaveKey, value);
        protected int LoadInt(int defaultValue = 0) => PlayerPrefs.GetInt(SaveKey, defaultValue);
        protected void SaveFloat(float value) => PlayerPrefs.SetFloat(SaveKey, value);
        protected float LoadFloat(float defaultValue = 0) => PlayerPrefs.GetFloat(SaveKey, defaultValue);
        protected void SaveString(string value) => PlayerPrefs.SetString(SaveKey, value);
        protected string LoadString(string defaultValue = "") => PlayerPrefs.GetString(SaveKey, defaultValue);

        #endregion
    }
}