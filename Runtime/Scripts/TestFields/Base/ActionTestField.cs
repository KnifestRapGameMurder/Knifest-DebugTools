using UnityEngine;

namespace Knifest.DebugTools
{
    public abstract class ActionTestField<T> : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_InputField _input;
        [SerializeField] private UnityEngine.Events.UnityEvent<T> _onClick;
        [SerializeField] private T _defaulValue;

        public void OnButtonClick()
        {
            _onClick?.Invoke(ParseInput(_input.text));
            // _input.text = _defaulValue.ToString();
        }

        protected abstract T ParseInput(string input);
    }
}