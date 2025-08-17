using System.Collections.Generic;
using Knifest.DebugTools.DebugFields;
using UnityEngine;

namespace Knifest.DebugTools
{
    public class DebugManager : MonoBehaviour
    {
        [SerializeField] private GameObject _settings;
        [SerializeField] private Transform _fieldsContainer;
        [SerializeField] private bool stopTime;

        private List<IDebugField> _fields = new();

        public Transform Container => _fieldsContainer;
        public GameObject Settings => _settings;

        private void Awake()
        {
            foreach (Transform child in _fieldsContainer)
                if (child.gameObject.activeSelf && child.TryGetComponent<IDebugField>(out var field))
                    _fields.Add(field);

            _fields.ForEach(f => f.Init());
            CloseSettings();
        }

        public void OpenSettings()
        {
            if (stopTime) Time.timeScale = 0;
            _settings.SetActive(true);
        }

        public void ResetSettings()
        {
            foreach (var field in _fields)
                field.ResetToDefault();
        }

        public void CloseSettings()
        {
            PlayerPrefs.Save();
            _settings.SetActive(false);
            if (stopTime) Time.timeScale = 1;
        }
    }
}