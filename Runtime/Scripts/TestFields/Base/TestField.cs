using System;
using Knifest.DebugTools.PropertyAttributes;
using UnityEngine;

namespace Knifest.DebugTools
{
    public abstract class TestField<T> : MonoBehaviour, ITestField
    {
        [SerializeField] [ReadOnly] private string saveKey;

        [SerializeField] private TMPro.TMP_Text _labelUI;
        [field: SerializeField] protected UnityEngine.Events.UnityEvent<T> OnInput { get; private set; }
        [field: SerializeField] protected string Label { get; private set; }
        [field: SerializeField] protected T DefaultValue { get; set; }

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

            OnInput?.Invoke(Value);
        }

        public void ResetToDefault()
        {
            OnValueChanged(DefaultValue);
            SetValueToUI(Value);
        }

        protected virtual void OnValueChanged(T value)
        {
            Value = value;
            OnInput.Invoke(Value);
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