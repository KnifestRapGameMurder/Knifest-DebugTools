using System;
using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace Knifest.DebugTools.DebugFields
{
    public abstract class ActionDebugField<T> : BaseDebugField<T>
    {
        [Group(Tabs_Events), Tab(Tab_User), SerializeField]
        protected UnityEngine.Events.UnityEvent<T> onClick;

        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        protected TMPro.TMP_InputField input;


        [UsedImplicitly]
        public virtual void OnButtonClick()
        {
            onClick?.Invoke(ParseInput(input.text));
        }

        protected abstract T ParseInput(string value);
    }
}