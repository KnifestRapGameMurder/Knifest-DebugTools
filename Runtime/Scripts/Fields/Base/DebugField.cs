using System;
using TriInspector;
using UnityEngine;

namespace Knifest.DebugTools.DebugFields
{
    public abstract class DebugField<T> : BaseDebugField<T>, IDebugField
    {
        [field: Group(Tabs_Events), Tab(Tab_User), SerializeField]
        protected UnityEngine.Events.UnityEvent<T> OnValue { get; private set; }

        public T Value { get; protected set; }

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

        protected abstract T GetPrefsValue();
        protected abstract void SavePrefsValue();
    }
}