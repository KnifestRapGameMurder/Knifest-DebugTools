using System.Collections.Generic;
using UnityEngine;

namespace Knifest.DebugTools
{
    public class TestManager : MonoBehaviour
    {
        [SerializeField] private GameObject _settings;
        [SerializeField] private Transform _fieldsContainer;

        private List<ITestField> _fields = new List<ITestField>();

        private void Awake()
        {
            foreach (Transform child in _fieldsContainer)
                if (child.TryGetComponent<ITestField>(out var field))
                    _fields.Add(field);

            _fields.ForEach(f => f.Init());
            CloseSettings();
        }

        public void OpenSettings()
        {
            Time.timeScale = 0;
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
            Time.timeScale = 1;
        }
    }
}