using System;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Knifest.DebugTools.DebugFields
{
    [DeclareTabGroup(Tabs)]
    [DeclareBoxGroup(Tabs_Events, Title = "Events")]
    public abstract class BaseDebugField<T> : MonoBehaviour
    {
        protected const string Tabs = "Tabs";
        protected const string Tab_User = "User";
        protected const string Tab_Dev = "Dev";
        protected const string Tabs_Events = Tabs + "/Events";

        [field: Group(Tabs), Tab(Tab_User), SerializeField]
        protected string Label { get; private set; }

        [field: Group(Tabs), Tab(Tab_User), SerializeField]
        protected T DefaultValue { get; set; }

        [Group(Tabs), Tab(Tab_Dev)] [SerializeField]
        protected TMPro.TMP_Text labelUI;

        // [Group(Tabs), Tab(Tab_Dev), SerializeField]
        // private string saveKey;

        // public string SaveKey => saveKey;
        public string SaveKey => Label;

        protected virtual void OnValidate()
        {
            // ManageSaveKey();
            name = Label;
            if (labelUI != null) labelUI.text = Label;
            SetValueToUI(DefaultValue);
        }

//         private void ManageSaveKey()
//         {
// #if UNITY_EDITOR
//             // Skip if this is the prefab asset itself, not a scene instance
//             if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
//                 return;
//
//             // Also skip if we're in Prefab Editing Mode (the isolated prefab stage)
//             if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
//                 return;
// #endif
//
//             if (string.IsNullOrEmpty(saveKey)) saveKey = Guid.NewGuid().ToString();
//         }

        protected abstract void SetValueToUI(T value);

        #region SaveLoad

        protected bool IsSaved() => PlayerPrefs.HasKey(SaveKey);
        protected void SaveBool(bool value) => PlayerPrefs.SetInt(SaveKey, value ? 1 : 0);
        protected bool LoadBool(bool defaultValue = false) => PlayerPrefs.GetInt(SaveKey, defaultValue ? 1 : 0) != 0;
        protected void SaveInt(int value) => PlayerPrefs.SetInt(SaveKey, value);
        protected int LoadInt(int defaultValue = 0) => PlayerPrefs.GetInt(SaveKey, defaultValue);
        protected void SaveFloat(float value) => PlayerPrefs.SetFloat(SaveKey, value);
        protected float LoadFloat(float defaultValue = 0) => PlayerPrefs.GetFloat(SaveKey, defaultValue);
        protected void SaveString(string value) => PlayerPrefs.SetString(SaveKey, value);
        protected string LoadString(string defaultValue = "") => PlayerPrefs.GetString(SaveKey, defaultValue);

        #endregion
    }
}