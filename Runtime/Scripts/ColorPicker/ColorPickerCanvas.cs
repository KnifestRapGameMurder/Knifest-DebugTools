using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Knifest.DebugTools.ColorPick
{
    public class ColorPickerCanvas : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private ColorPicker colorPicker;
        [SerializeField] private Image oldColorPreview;
        [SerializeField] private Image newColorPreview;
        [SerializeField] private TMPro.TMP_Text oldColorName;
        [SerializeField] private TMPro.TMP_Text newColorName;

        public Color Color => colorPicker.color;
        public UnityEvent CloseButtonClicked => closeButton.onClick;

        public void SetOldColor(Color color)
        {
            oldColorPreview.color = color;
            newColorPreview.color = color;
            oldColorName.text = ColorUtility.ToHtmlStringRGBA(color);
            newColorName.text = ColorUtility.ToHtmlStringRGBA(color);
            colorPicker.color = color;
        }

        private void LateUpdate()
        {
            newColorPreview.color = colorPicker.color;
            newColorName.text = ColorUtility.ToHtmlStringRGBA(colorPicker.color);
        }
    }
}