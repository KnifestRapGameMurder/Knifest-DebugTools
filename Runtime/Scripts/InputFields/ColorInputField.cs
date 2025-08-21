using System;
using Knifest.DebugTools.ColorPick;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Knifest.DebugTools.InputFields
{
    [RequireComponent(typeof(Graphic))]
    public class ColorInputField : UIBehaviour, IPointerClickHandler
    {
        public event Action<Color> ValueChanged;

        [SerializeField] private Graphic graphic;
        [SerializeField] private ColorPickerCanvas pickerCanvasPrefab;
        [SerializeField] private TMPro.TMP_Text colorName;

        private Graphic _graphic;
        private ColorPickerCanvas _canvas = null;

        public Color Color
        {
            get => graphic.color;
            set
            {
                graphic.color = value;
                colorName.text = ColorUtility.ToHtmlStringRGBA(value);
                colorName.color = value.grayscale > 0.5f && value.a > 0.5 ? Color.black : Color.white;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _canvas = Instantiate(pickerCanvasPrefab);
            _canvas.CloseButtonClicked.AddListener(OnCloseButtonClicked);
            _canvas.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _canvas.gameObject.SetActive(true);
            _canvas.SetOldColor(Color);
        }

        private void OnCloseButtonClicked()
        {
            Color = _canvas.Color;
            ValueChanged?.Invoke(_canvas.Color);
            _canvas.gameObject.SetActive(false);
        }
    }
}