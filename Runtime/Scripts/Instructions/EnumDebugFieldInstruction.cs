using System;
using System.Collections.Generic;
using System.Linq;
using Knifest.DebugTools.DebugFields;
using UnityEngine.Events;

namespace Knifest.DebugTools
{
    public abstract class EnumDebugFieldInstruction<TEnum> : IEnumDebugFieldInstruction where TEnum : Enum
    {
        public UnityEvent<TEnum> onValueChanged;

        public List<string> GetOptions() => Enum.GetNames(typeof(TEnum)).ToList();

        public void Invoke(int index)
        {
            onValueChanged?.Invoke((TEnum)Enum.GetValues(typeof(TEnum)).GetValue(index));
        }
    }
}