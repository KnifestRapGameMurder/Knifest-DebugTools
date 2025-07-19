using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Knifest.DebugTools.Editor
{
    public static class MenuItems
    {
        private const string MenuPath = Names.Root + "/";
        private const string GO_MenuPath = "GameObject/" + MenuPath;
        private const int MenuPriority = int.MaxValue;
        private const string PrefabPath = "Packages/com.knifest.debugtools/Runtime/Prefabs/DEBUG TOOLS.prefab";
        private const string FieldPrefabsPath = "Packages/com.knifest.debugtools/Runtime/Prefabs/Fields";

        private const string _AddDebugTools = "Add DebugTools";

        [MenuItem(GO_MenuPath + _AddDebugTools, false, MenuPriority)]
        [MenuItem(MenuPath + _AddDebugTools, false, 0)]
        static void SpawnDebugTools(MenuCommand command)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);

            if (!prefab)
            {
                Debug.LogError($"Could not find prefab at '{PrefabPath}'");
                return;
            }

#if UNITY_2018_3_OR_NEWER
            // Must supply the Scene so it appears in the right scene
            var scene = (command.context as GameObject)?.scene ?? SceneManager.GetActiveScene();
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
#else
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
#endif

            if (command.context is GameObject parentGO)
            {
                instance.transform.SetParent(parentGO.transform, false);
            }

            Undo.RegisterCreatedObjectUndo(instance, _AddDebugTools);
            Selection.activeObject = instance;
        }

        [MenuItem(MenuPath + "Open Container", false, 1)]
        private static void ShowContainer()
        {
            var manager = Object.FindObjectOfType<DebugManager>();

            if (manager == null)
            {
                Debug.LogError("No DebugManager found in the scene.");
                return;
            }

            GameObject containerGO = manager.Container.gameObject;
            GameObject settingsGO = manager.Settings;

            Selection.activeGameObject = containerGO;
            EditorGUIUtility.PingObject(containerGO);

            ExpandOrCollapseSelectedGameObject(true);

            settingsGO.SetActive(true);
        }

        [MenuItem(MenuPath + "Close Container", false, 2)]
        private static void CloseContainer()
        {
            var manager = Object.FindObjectOfType<DebugManager>();

            if (manager == null)
            {
                Debug.LogError("No DebugManager found in the scene.");
                return;
            }

            GameObject containerGO = manager.Container.gameObject;
            GameObject settingsGO = manager.Settings;

            Selection.activeGameObject = manager.gameObject;
            // EditorGUIUtility.PingObject(containerGO);

            ExpandOrCollapseSelectedGameObject(false);

            settingsGO.SetActive(false);
        }

        [MenuItem(MenuPath + "Open Prefab Folder", false, 3)]
        private static void OpenPrefabFolder()
        {
            // 1) Load the folder asset
        var folderAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(FieldPrefabsPath);
        if (folderAsset == null)
        {
            Debug.LogError($"[DebugTools] Could not load folder at '{FieldPrefabsPath}'.");
            return;
        }
        int folderID = folderAsset.GetInstanceID();

        // 2) Grab the ProjectBrowser type & window instance
        var pbType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
        if (pbType != null)
        {
            // Get the open window (or open one if none exists)
            var pbWindow = EditorWindow.GetWindow(pbType);
            if (pbWindow != null)
            {
                // 3) Find the instance method ShowFolderContents(...)
                var methods = pbType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo showFolder = null;
                foreach (var m in methods)
                {
                    if (m.Name == "ShowFolderContents")
                    {
                        var p = m.GetParameters();
                        if ((p.Length == 2 && p[0].ParameterType == typeof(int) && p[1].ParameterType == typeof(bool))
                            || (p.Length == 1 && p[0].ParameterType == typeof(int)))
                        {
                            showFolder = m;
                            break;
                        }
                    }
                }

                if (showFolder != null)
                {
                    // 4) Invoke it with either (id, true) or (id) depending on signature
                    var parms = showFolder.GetParameters().Length == 2
                        ? new object[] { folderID, true }
                        : new object[] { folderID };
                    showFolder.Invoke(pbWindow, parms);

                    // 5) Ensure it repaint/focus so you see the change
                    pbWindow.Repaint();
                    return;
                }
            }
        }

        // Fallback: just ping/select
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = folderAsset;
        EditorGUIUtility.PingObject(folderAsset);
        }

        static void ExpandOrCollapseSelectedGameObject(bool expand)
        {
            if (Selection.activeGameObject == null)
            {
                Debug.LogWarning("No GameObject selected.");
                return;
            }

            var hierarchyWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            if (hierarchyWindowType == null)
            {
                Debug.LogError("Could not find SceneHierarchyWindow type.");
                return;
            }

            var window = EditorWindow.GetWindow(hierarchyWindowType);
            if (window == null)
            {
                Debug.LogError("Could not get SceneHierarchyWindow instance.");
                return;
            }

            var setExpandedRecursiveMethod = hierarchyWindowType.GetMethod("SetExpandedRecursive",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (setExpandedRecursiveMethod == null)
            {
                Debug.LogError("Could not find SetExpandedRecursive method.");
                return;
            }

            setExpandedRecursiveMethod.Invoke(window,
                new object[] { Selection.activeGameObject.GetInstanceID(), expand });

            if (expand)
            {
                var go = Selection.activeGameObject;
                foreach (Transform child in go.transform)
                {
                    Selection.activeGameObject = child.gameObject;
                    ExpandOrCollapseSelectedGameObject(false);
                }

                Selection.activeGameObject = go;
            }
        }
    }
}