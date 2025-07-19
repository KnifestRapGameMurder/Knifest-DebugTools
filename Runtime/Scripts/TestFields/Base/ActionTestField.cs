using UnityEngine;

namespace Knifest.DebugTools
{
    public abstract class ActionTestField<T> : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_InputField input;
        [SerializeField] private UnityEngine.Events.UnityEvent<T> onClick;
        [SerializeField] private T defaultValue;

        public void OnButtonClick()
        {
            onClick?.Invoke(ParseInput(input.text));
            // _input.text = _defaulValue.ToString();
        }

        protected abstract T ParseInput(string input);
    }
}