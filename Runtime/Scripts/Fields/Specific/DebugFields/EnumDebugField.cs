using System;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Knifest.DebugTools
{
    public class EnumDebugField : BaseDebugField
    {
        [Group(Tabs), Tab(Tab_User), PropertyOrder(0), SerializeReference]
        private Enum enumType;

        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        private TMPro.TMP_Dropdown input;

        [ReadOnly, SerializeReference] private object handler;

        private void OnValidate()
        {
            Debug.Log("OnValidate");
        }

        public void Test(RenderTextureFormat value)
        {
            Debug.Log(value);
        }

        [Serializable]
        private class Handler<T> where T : Enum
        {
            public T DefaultValue;
            public UnityEvent<T> OnValueChanged;

            public T Value { get; protected set; }
        }
    }
}