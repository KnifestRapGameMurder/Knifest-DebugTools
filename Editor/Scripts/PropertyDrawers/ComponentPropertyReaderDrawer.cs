// This script MUST be in an 'Editor' folder (e.g., Assets/Editor/)

using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

// Target any ComponentPropertyReader<T>
[CustomPropertyDrawer(typeof(ComponentPropertyReader<>))]
public class ComponentPropertyReaderDrawer : PropertyDrawer
{
    // Key: property.propertyPath
    // Value: (lastObjectPath, cachedCompTypes, cachedCompNames, selectedCompIdx,
    //         cachedMembers, cachedMemberNames, selectedMemberIdx)
    // The MemberType enum type will be accessed dynamically.
    private static Dictionary<string, (string, Type[], string[], int, (MemberInfo, object)[], string[], int)>
        _drawerStates =
            new Dictionary<string, (string, Type[], string[], int, (MemberInfo, object)[], string[], int)>();

    private static GUIStyle _customBoxStyle;
    private Type _memberTypeEnum; // To cache the actual Type for MemberType enum

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 +
               EditorGUIUtility.standardVerticalSpacing * 3; // Increased for box padding
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        string propertyPath = property.propertyPath;
        // Initialize state with default "No Object" and "No Member" values
        if (!_drawerStates.ContainsKey(propertyPath))
        {
            // The 'object' placeholder for MemberType.None will be cast later
            _drawerStates.Add(propertyPath,
                (null, null, new string[] { "No Object" }, 0, null, new string[] { "No Member" }, 0));
        }

        var state = _drawerStates.GetValueOrDefault(propertyPath);
        string lastObjectPath = state.Item1;
        Type[] _componentTypes = state.Item2;
        string[] _componentNames = state.Item3;
        int _selectedComponentIndex = state.Item4;
        // The tuple type for members now uses 'object' for the MemberType enum value
        (MemberInfo, object)[] _members = state.Item5;
        string[] _memberNames = state.Item6;
        int _selectedMemberIndex = state.Item7;

        SerializedProperty targetObjectProp = property.FindPropertyRelative("targetObject");
        SerializedProperty componentTypeNameProp = property.FindPropertyRelative("componentTypeName");
        SerializedProperty memberNameProp = property.FindPropertyRelative("memberName");
        SerializedProperty memberTypeProp = property.FindPropertyRelative("memberType");

        // Get the generic type argument of the current ComponentPropertyReader<T> instance
        Type genericPropertyType = fieldInfo.FieldType.GetGenericArguments()[0];

        // Dynamically get the MemberType enum Type
        if (_memberTypeEnum == null)
        {
            _memberTypeEnum = fieldInfo.FieldType.GetGenericTypeDefinition().GetNestedType("MemberType");
            if (_memberTypeEnum == null)
            {
                Debug.LogError("Could not find nested enum 'MemberType' in ComponentPropertyReader.");
                return; // Exit if the enum type isn't found
            }
        }

        // Get the 'None' enum value dynamically
        object memberTypeNone = Enum.Parse(_memberTypeEnum, "None");


        if (_customBoxStyle == null)
        {
            _customBoxStyle = new GUIStyle(GUI.skin.box);
            Color boxColor = new Color(0.18f, 0.18f, 0.18f, 1.0f); // Darker gray for outline
            Texture2D coloredTexture = new Texture2D(1, 1);
            coloredTexture.SetPixel(0, 0, boxColor);
            coloredTexture.Apply();
            _customBoxStyle.normal.background = coloredTexture;
            _customBoxStyle.hover.background = coloredTexture;
            _customBoxStyle.active.background = coloredTexture;
            _customBoxStyle.border = new RectOffset(2, 2, 2, 2); // Add a small border
            _customBoxStyle.padding = new RectOffset(5, 5, 5, 5); // Add internal padding
        }

        Rect innerPosition = new Rect(position.x + 2, position.y + 2, position.width - 4, position.height - 4);
        GUI.Box(position, GUIContent.none, _customBoxStyle);

        Rect labelRect = new Rect(innerPosition.x, innerPosition.y, EditorGUIUtility.labelWidth,
            EditorGUIUtility.singleLineHeight);
        EditorGUI.PrefixLabel(labelRect, label);

        Rect contentRect = new Rect(labelRect.xMax, innerPosition.y, innerPosition.width - labelRect.width,
            EditorGUIUtility.singleLineHeight);

        EditorGUI.BeginChangeCheck();
        UnityEngine.Object currentSelectedObject = EditorGUI.ObjectField(contentRect, GUIContent.none,
            targetObjectProp.objectReferenceValue, typeof(UnityEngine.Object), true);
        if (EditorGUI.EndChangeCheck())
        {
            targetObjectProp.objectReferenceValue = currentSelectedObject;
            componentTypeNameProp.stringValue = "";
            memberNameProp.stringValue = ""; // Clear member name
            memberTypeProp.enumValueIndex = (int)memberTypeNone; // Reset member type dynamically
            _selectedComponentIndex = 0;
            _selectedMemberIndex = 0;
            _componentTypes = null;
            _members = null;
        }

        UnityEngine.Object liveTargetObject = targetObjectProp.objectReferenceValue;
        string currentObjectUniquePath = liveTargetObject != null
            ? AssetDatabase.GetAssetPath(liveTargetObject) + (liveTargetObject is GameObject
                ? liveTargetObject.name
                : liveTargetObject.GetType().Name)
            : "";
        bool objectChanged = (lastObjectPath != currentObjectUniquePath);

        Rect nextLineRect = new Rect(innerPosition.x + EditorGUIUtility.labelWidth,
            innerPosition.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
            innerPosition.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        float halfWidth = nextLineRect.width / 2 - EditorGUIUtility.standardVerticalSpacing / 2;
        Rect componentRect = new Rect(nextLineRect.x, nextLineRect.y, halfWidth, nextLineRect.height);
        Rect memberRect = new Rect(nextLineRect.x + halfWidth + EditorGUIUtility.standardVerticalSpacing,
            nextLineRect.y, halfWidth, nextLineRect.height);

        EditorGUI.indentLevel++;
        if (liveTargetObject is GameObject currentGameObject)
        {
            if (objectChanged || _componentTypes == null || (_selectedComponentIndex <= 0 &&
                                                             !string.IsNullOrEmpty(componentTypeNameProp.stringValue)))
            {
                PopulateComponentDropdown(currentGameObject, componentTypeNameProp.stringValue, out _componentTypes,
                    out _componentNames, out _selectedComponentIndex);
            }

            EditorGUI.BeginChangeCheck();
            int newSelectedComponentIndex = EditorGUI.Popup(componentRect, _selectedComponentIndex, _componentNames);
            if (EditorGUI.EndChangeCheck())
            {
                _selectedComponentIndex = newSelectedComponentIndex;
                if (_selectedComponentIndex > 0 && _componentTypes != null)
                {
                    componentTypeNameProp.stringValue =
                        _componentTypes[_selectedComponentIndex - 1].AssemblyQualifiedName;
                    PopulateMembersDropdown(_componentTypes[_selectedComponentIndex - 1], memberNameProp.stringValue,
                        (object)memberTypeProp.enumValueIndex, // Pass current member type as object
                        genericPropertyType, out _members, out _memberNames, out _selectedMemberIndex);
                }
                else
                {
                    componentTypeNameProp.stringValue = "";
                    _members = null;
                    _memberNames = new string[] { "No Component" };
                    _selectedMemberIndex = 0;
                    memberNameProp.stringValue = "";
                    memberTypeProp.enumValueIndex = (int)memberTypeNone; // Reset dynamically
                }

                memberNameProp.stringValue = "";
                memberTypeProp.enumValueIndex = (int)memberTypeNone; // Reset dynamically
                _selectedMemberIndex = 0;
            }
        }
        else // It's a direct UnityEngine.Object (not a GameObject)
        {
            if (objectChanged || _componentTypes == null || _componentNames?[0] != liveTargetObject?.GetType()?.Name)
            {
                Type objType = liveTargetObject?.GetType();
                _componentTypes = new Type[] { objType };
                _componentNames = new string[] { objType?.Name ?? "No Object" };
                _selectedComponentIndex = 0;
                componentTypeNameProp.stringValue = objType?.AssemblyQualifiedName ?? "";
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.Popup(componentRect, _selectedComponentIndex, _componentNames);
            EditorGUI.EndDisabledGroup();
        }

        // --- Member Dropdown (common to both GameObject components and direct Objects) ---
        Type currentComponentOrObjectType = null;
        if (liveTargetObject is GameObject)
        {
            if (_componentTypes != null && _selectedComponentIndex > 0 &&
                _selectedComponentIndex - 1 < _componentTypes.Length)
            {
                currentComponentOrObjectType = _componentTypes[_selectedComponentIndex - 1];
            }
        }
        else // Direct object
        {
            currentComponentOrObjectType = liveTargetObject?.GetType();
        }

        // Repopulate members if object/component changed, or if members list is null, or if stored member is not in current list
        if (objectChanged || _members == null ||
            (_selectedMemberIndex <= 0 && !string.IsNullOrEmpty(memberNameProp.stringValue)) ||
            (currentComponentOrObjectType != null && _members?.FirstOrDefault(m =>
                m.Item1?.Name == memberNameProp.stringValue &&
                m.Item1?.DeclaringType == currentComponentOrObjectType) == null))
        {
            PopulateMembersDropdown(currentComponentOrObjectType, memberNameProp.stringValue,
                (object)memberTypeProp.enumValueIndex, // Pass current member type as object
                genericPropertyType, out _members, out _memberNames, out _selectedMemberIndex);
        }
        else if ((liveTargetObject == null || currentComponentOrObjectType == null) &&
                 _memberNames?[0] != "No Member")
        {
            _members = null;
            _memberNames = new string[] { "No Member" };
            _selectedMemberIndex = 0;
            memberNameProp.stringValue = "";
            memberTypeProp.enumValueIndex = (int)memberTypeNone; // Reset dynamically
        }

        EditorGUI.BeginChangeCheck();
        int newSelectedMemberIndex = EditorGUI.Popup(memberRect, _selectedMemberIndex, _memberNames);
        if (EditorGUI.EndChangeCheck())
        {
            _selectedMemberIndex = newSelectedMemberIndex;
            if (_selectedMemberIndex > 0 && _members != null && _selectedMemberIndex - 1 < _members.Length &&
                _members[_selectedMemberIndex - 1].Item1 != null)
            {
                memberNameProp.stringValue = _members[_selectedMemberIndex - 1].Item1.Name;
                memberTypeProp.enumValueIndex =
                    (int)Convert.ChangeType(_members[_selectedMemberIndex - 1].Item2,
                        typeof(int)); // Cast back to int for enumValueIndex
            }
            else
            {
                memberNameProp.stringValue = "";
                memberTypeProp.enumValueIndex = (int)memberTypeNone; // Reset dynamically
            }
        }

        EditorGUI.indentLevel--;

        // Store updated state
        _drawerStates[propertyPath] = (currentObjectUniquePath, _componentTypes, _componentNames,
            _selectedComponentIndex, _members, _memberNames, _selectedMemberIndex);

        EditorGUI.EndProperty();
    }

    private void PopulateComponentDropdown(GameObject go, string currentlySelectedTypeName, out Type[] componentTypes,
        out string[] componentNames, out int selectedComponentIndex)
    {
        List<Type> types = new List<Type>();
        List<string> names = new List<string>();

        names.Add("None");
        types.Add(null);

        int currentIndex = 0;

        if (go != null)
        {
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Type compType = components?[i]?.GetType();
                if (compType != null && compType.IsVisible && !compType.FullName.Contains("UnityEditor") &&
                    !compType.FullName.Contains("UnityEngine.Internal"))
                {
                    types.Add(compType);
                    names.Add(compType.Name);

                    if (compType.AssemblyQualifiedName == currentlySelectedTypeName)
                    {
                        currentIndex = types.Count;
                    }
                }
            }
        }

        componentTypes = types.ToArray();
        componentNames = names.ToArray();
        selectedComponentIndex = Mathf.Max(0, currentIndex - 1);
    }

    // This method now populates both properties and methods
    private void PopulateMembersDropdown(Type typeToReflectOn, string currentlySelectedMemberName,
        object currentlySelectedMemberType, Type targetValueType, // currentlySelectedMemberType is now 'object'
        out (MemberInfo, object)[] members, out string[] memberNames, // Tuple also uses 'object'
        out int selectedMemberIndex)
    {
        List<(MemberInfo, object)> m_list = new List<(MemberInfo, object)>();
        List<string> n_list = new List<string>();

        // Dynamically get the enum values for comparison and storage
        object memberTypeNone = Enum.Parse(_memberTypeEnum, "None");
        object memberTypeProperty = Enum.Parse(_memberTypeEnum, "Property");
        object memberTypeField = Enum.Parse(_memberTypeEnum, "Field"); // <--- GET FIELD ENUM VALUE


        n_list.Add("None");
        m_list.Add((null, memberTypeNone));

        int currentIndex = 0;

        if (typeToReflectOn != null)
        {
            // --- Properties ---
            // Use Public | NonPublic | Instance to get both public and private instance properties
            PropertyInfo[] properties = typeToReflectOn
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.CanRead && p.PropertyType == targetValueType) // Filter by return type T
                .ToArray();

            if (properties.Length > 0)
            {
                n_list.Add("--- Properties ---"); // Separator
                m_list.Add((null, memberTypeNone)); // Placeholder for separator
            }

            foreach (PropertyInfo prop in properties)
            {
                m_list.Add((prop, memberTypeProperty));
                n_list.Add($"{prop.Name} (Property: {prop.PropertyType.Name})"); // More descriptive

                // Compare the 'object' enum values
                if (prop.Name == currentlySelectedMemberName && currentlySelectedMemberType.Equals(memberTypeProperty))
                {
                    currentIndex = m_list.Count;
                }
            }

            // --- Fields ---
            // Use Public | NonPublic | Instance to get both public and private instance fields
            FieldInfo[] fields = typeToReflectOn
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.FieldType == targetValueType) // Filter by type T
                .ToArray();

            if (fields.Length > 0)
            {
                n_list.Add("--- Fields ---"); // Separator
                m_list.Add((null, memberTypeNone)); // Placeholder for separator
            }

            foreach (FieldInfo field in fields)
            {
                m_list.Add((field, memberTypeField)); // <--- USE DEDICATED FIELD ENUM VALUE
                n_list.Add($"{field.Name} (Field: {field.FieldType.Name})"); // More descriptive

                if (field.Name == currentlySelectedMemberName &&
                    currentlySelectedMemberType.Equals(memberTypeField)) // <--- COMPARE WITH FIELD ENUM
                {
                    currentIndex = m_list.Count;
                }
            }
        }

        members = m_list.ToArray();
        memberNames = n_list.ToArray();
        selectedMemberIndex = Mathf.Max(0, currentIndex - 1);
    }
}