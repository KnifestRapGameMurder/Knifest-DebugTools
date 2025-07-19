using UnityEngine;
using System;
using System.Reflection; // For PropertyInfo, MethodInfo, FieldInfo, BindingFlags
using System.Linq; // For LINQ queries

[Serializable]
public class ComponentPropertyReader<T>
{
    public UnityEngine.Object targetObject;
    public string componentTypeName;

    // Use separate fields for property and method, and a string to indicate type of member
    public string memberName; // Stores the name of the selected property, field OR method
    public MemberType memberType; // Enum to indicate if it's a property, field or method

    public enum MemberType
    {
        None,
        Property,
        Field, // <--- ADDED FIELD ENUM VALUE
        Method
    }

    // Runtime cached fields
    private Component _cachedComponent;
    private UnityEngine.Object _cachedDirectObject;
    private PropertyInfo _cachedPropertyInfo;
    private FieldInfo _cachedFieldInfo; // <--- ADDED for fields
    private MethodInfo _cachedMethodInfo;

    public T Value
    {
        get
        {
            Initialize();

            object instanceToGetFrom = _cachedComponent != null ? (object)_cachedComponent : (object)_cachedDirectObject;

            if (instanceToGetFrom == null || string.IsNullOrEmpty(memberName) || memberType == MemberType.None)
            {
                //Debug.LogWarning($"ComponentPropertyReader: Cannot get value. Missing target, member, or type.");
                return default(T);
            }

            try
            {
                if (memberType == MemberType.Property && _cachedPropertyInfo != null)
                {
                    return (T)_cachedPropertyInfo.GetValue(instanceToGetFrom);
                }
                else if (memberType == MemberType.Field && _cachedFieldInfo != null) // <--- HANDLE FIELDS
                {
                    return (T)_cachedFieldInfo.GetValue(instanceToGetFrom);
                }
                else if (memberType == MemberType.Method && _cachedMethodInfo != null)
                {
                    // Assuming methods are getters that return T and take no params.
                    return (T)_cachedMethodInfo.Invoke(instanceToGetFrom, null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading member '{memberName}' of type {typeof(T).Name} from {instanceToGetFrom?.GetType().Name}: {e.Message}");
                return default(T);
            }
            return default(T); // Should not reach here if initialized correctly and member found
        }
    }

    public void Initialize()
    {
        if (targetObject == null || string.IsNullOrEmpty(memberName) || memberType == MemberType.None)
        {
            _cachedComponent = null;
            _cachedDirectObject = null;
            _cachedPropertyInfo = null;
            _cachedFieldInfo = null; // Clear field cache
            _cachedMethodInfo = null;
            return;
        }

        Type typeToReflectOn = null;
        if (targetObject is GameObject gameObject)
        {
            typeToReflectOn = Type.GetType(componentTypeName); // Try full name first, then Assembly-CSharp
            if (typeToReflectOn == null)
            {
                typeToReflectOn = Type.GetType(componentTypeName + ",UnityEngine");
            }
            if (typeToReflectOn == null) // If still null, try Assembly-CSharp
            {
                typeToReflectOn = Type.GetType(componentTypeName + ",Assembly-CSharp");
            }

            if (typeToReflectOn != null)
            {
                _cachedComponent = gameObject.GetComponent(typeToReflectOn);
                _cachedDirectObject = null;
            }
            else
            {
                Debug.LogError($"Component Type '{componentTypeName}' not found.");
                _cachedComponent = null;
                _cachedDirectObject = null;
                return;
            }

            if (_cachedComponent == null)
            {
                Debug.LogError($"Component '{componentTypeName}' not found on '{targetObject.name}'.");
                _cachedDirectObject = null;
                return;
            }
        }
        else // It's a direct UnityEngine.Object
        {
            typeToReflectOn = targetObject.GetType();
            _cachedDirectObject = targetObject;
            _cachedComponent = null;

            // Only warn if the stored componentTypeName is different and not null/empty
            if (!string.IsNullOrEmpty(componentTypeName) && typeToReflectOn.AssemblyQualifiedName != componentTypeName)
            {
                Debug.LogWarning($"ComponentPropertyReader: Target object type '{typeToReflectOn.Name}' does not match stored component type name '{componentTypeName}'. Resetting member selection.");
                _cachedPropertyInfo = null;
                _cachedFieldInfo = null;
                _cachedMethodInfo = null;
                return;
            }
        }

        if (typeToReflectOn == null)
        {
            _cachedPropertyInfo = null;
            _cachedFieldInfo = null;
            _cachedMethodInfo = null;
            return;
        }

        // Cache the specific member based on memberType
        // Clear other caches to ensure only the relevant one is populated
        _cachedPropertyInfo = null;
        _cachedFieldInfo = null;
        _cachedMethodInfo = null;

        const BindingFlags allInstanceBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        if (memberType == MemberType.Property)
        {
            bool propertyInfoMatches = (_cachedPropertyInfo != null && _cachedPropertyInfo.Name == memberName && _cachedPropertyInfo.DeclaringType == typeToReflectOn);
            if (propertyInfoMatches) return; // Already cached and matches

            _cachedPropertyInfo = typeToReflectOn.GetProperty(memberName, allInstanceBindingFlags);

            if (_cachedPropertyInfo == null)
            {
                Debug.LogError($"Property '{memberName}' not found on {typeToReflectOn.Name}.");
            }
            else if (!_cachedPropertyInfo.CanRead)
            {
                Debug.LogError($"Property '{memberName}' on {typeToReflectOn.Name} is not readable.");
                _cachedPropertyInfo = null;
            }
            else if (_cachedPropertyInfo.PropertyType != typeof(T))
            {
                Debug.LogError($"Property '{memberName}' of type {_cachedPropertyInfo.PropertyType.Name} does not match reader type {typeof(T).Name}.");
                _cachedPropertyInfo = null;
            }
        }
        else if (memberType == MemberType.Field) // <--- HANDLE FIELD INITIALIZATION
        {
            bool fieldInfoMatches = (_cachedFieldInfo != null && _cachedFieldInfo.Name == memberName && _cachedFieldInfo.DeclaringType == typeToReflectOn);
            if (fieldInfoMatches) return; // Already cached and matches

            _cachedFieldInfo = typeToReflectOn.GetField(memberName, allInstanceBindingFlags);

            if (_cachedFieldInfo == null)
            {
                Debug.LogError($"Field '{memberName}' not found on {typeToReflectOn.Name}.");
            }
            else if (_cachedFieldInfo.FieldType != typeof(T))
            {
                Debug.LogError($"Field '{memberName}' of type {_cachedFieldInfo.FieldType.Name} does not match reader type {typeof(T).Name}.");
                _cachedFieldInfo = null;
            }
        }
        else if (memberType == MemberType.Method)
        {
            bool methodInfoMatches = (_cachedMethodInfo != null && _cachedMethodInfo.Name == memberName && _cachedMethodInfo.DeclaringType == typeToReflectOn);
            if (methodInfoMatches) return; // Already cached and matches

            // Find method: public or non-public, instance, returns T, no parameters
            _cachedMethodInfo = typeToReflectOn.GetMethod(memberName,
                allInstanceBindingFlags,
                null, // Binder
                CallingConventions.Any,
                Type.EmptyTypes, // Methods with no parameters
                null); // Modifiers

            if (_cachedMethodInfo == null)
            {
                Debug.LogError($"Method '{memberName}' with no parameters not found on {typeToReflectOn.Name}.");
            }
            else if (_cachedMethodInfo.ReturnType != typeof(T))
            {
                Debug.LogError($"Method '{memberName}' returns {_cachedMethodInfo.ReturnType.Name} but reader expects {typeof(T).Name}.");
                _cachedMethodInfo = null;
            }
        }
    }
}