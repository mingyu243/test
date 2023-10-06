using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SubclassSelector))]
public class SubclassSelectorEditor : PropertyDrawer
{
    private static Dictionary<Type, List<Type>> selectableTypeMap = new Dictionary<Type, List<Type>>();

    string keyword = string.Empty;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect keywordFieldRect = new Rect(position.x, position.y, position.width, 18);
        Rect popupRect = new Rect(position.x, position.y + 20, position.width, 18);

        SubclassSelector selector = property.managedReferenceValue as SubclassSelector;
        if (selector == null) 
        {
            selector = new SubclassSelector();
            property.managedReferenceValue = selector;
        }

        List<Type> selectableTypes = null;

        InheritsAttribute inherits = fieldInfo.GetCustomAttribute<InheritsAttribute>();
        if (inherits != null)
        {
            if (!selectableTypeMap.ContainsKey(inherits.Type)) 
            {
                selectableTypes = new List<Type>();
                selectableTypes.AddRange(TypeCache.GetTypesDerivedFrom(inherits.Type)
                    .Where((Type type) => !(type.IsNotPublic || type.IsAbstract || type.IsInterface)));

                selectableTypeMap.Add(inherits.Type, selectableTypes);
            }
            else
            {
                selectableTypes = selectableTypeMap[inherits.Type];
            }

            selector.BaseType = inherits.Type;
        }
        else
        {
            if (selectableTypeMap.ContainsKey(typeof(object))) 
            {
                selectableTypes = selectableTypeMap[typeof(object)];
            }
            else
            {
                selectableTypes = new List<Type>();
                selectableTypes.AddRange(TypeCache.GetTypesDerivedFrom<object>()
                    .Where((Type type) => !(type.IsNotPublic || type.IsAbstract || type.IsInterface)));

                selectableTypeMap.Add(typeof(object), selectableTypes);
            }
        }
        
        keyword = EditorGUI.TextField(keywordFieldRect, "Search", keyword, EditorStyles.textField);

        List<GUIContent> contents = new List<GUIContent>();
        contents.Add(new GUIContent("(None)"));

        Dictionary<int, Type> popupIndexMap = new Dictionary<int, Type>()
        {
            { 0, null }
        };

        int selectedIdx = 0;
        for (int i = 0; i < selectableTypes.Count(); i++)
        {
            Type type = selectableTypes[i];
            if (!type.Name.Contains(keyword) && type != selector.SelectedType)
            {
                continue;
            }

            if (type == selector.SelectedType)
            {
                selectedIdx = popupIndexMap.Count;
            }

            contents.Add(new GUIContent(type.Name));
            popupIndexMap.Add(popupIndexMap.Count, type);
        }

        selectedIdx = EditorGUI.Popup(popupRect, selectedIdx, contents.ToArray(), EditorStyles.popup);

        Type selectedType = popupIndexMap[selectedIdx];
        if (selector.SelectedType != selectedType) 
        {
            selector.SelectedType = selectedType;
            keyword = string.Empty;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 40f;
    }
}