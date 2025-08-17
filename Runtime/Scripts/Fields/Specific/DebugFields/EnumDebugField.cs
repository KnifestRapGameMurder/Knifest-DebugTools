using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace Knifest.DebugTools.DebugFields
{
    public class EnumDebugField : DebugField<int>
    {
        [Group(Tabs), Tab(Tab_Dev), SerializeField]
        private TMPro.TMP_Dropdown input;

        [Group(Tabs), Tab(Tab_User), SerializeReference]
        private IEnumDebugFieldInstruction instruction;

        public override void Init()
        {
            if (instruction != null)
            {
                input.ClearOptions();
                input.AddOptions(instruction.GetOptions());
            }

            base.Init();
            input.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void SetValueToUI(int value)
        {
            input.value = value;
        }

        protected override int GetPrefsValue()
        {
            return LoadInt();
        }

        protected override void SavePrefsValue()
        {
            SaveInt(Value);
        }

        protected override void OnValueChanged(int value)
        {
            base.OnValueChanged(value);
            instruction?.Invoke(value);
        }
    }

    public interface IEnumDebugFieldInstruction
    {
        List<string> GetOptions();
        void Invoke(int index);
    }
}