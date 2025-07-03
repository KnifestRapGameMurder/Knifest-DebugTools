using UnityEngine;

namespace Knifest.DebugTools
{
    public abstract class TestField<T> : MonoBehaviour, ITestField
    {
        [SerializeField] private TMPro.TMP_Text _labelUI;
        [field: SerializeField] protected UnityEngine.Events.UnityEvent<T> OnInput { get; private set; }
        [field: SerializeField] protected string Label { get; private set; }
        [field: SerializeField] protected T DefaultValue { get; set; }

        protected T Value { get; set; }

        protected virtual void OnValidate()
        {
            if (_labelUI != null) _labelUI.text = Label;
            SetValueToUI(DefaultValue);
        }

        public virtual void Init()
        {
            Value = DefaultValue;

            if (PlayerPrefs.HasKey(Label))
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
    }
}