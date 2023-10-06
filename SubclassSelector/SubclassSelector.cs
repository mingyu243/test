using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("SubclassSelector.Editor")]
[Serializable]
public class SubclassSelector
{
    public event Action onTypeSelected;

    [HideInInspector]
    [SerializeField]
    string selectedTypePath;

    [HideInInspector]
    [SerializeField]
    string baseTypePath;

    public Type SelectedType
    {
        get
        {
            if (string.IsNullOrEmpty(selectedTypePath))
            {
                return null;
            }

            return Type.GetType(selectedTypePath);
        }
        internal set
        {
            if (value == null)
            {
                selectedTypePath = null;
            }
            else
            {
                selectedTypePath = value.AssemblyQualifiedName;
            }

            onTypeSelected?.Invoke();
        }
    }

    public Type BaseType
    {
        get
        {
            return Type.GetType(baseTypePath);
        }
        internal set
        {
            baseTypePath = value.AssemblyQualifiedName;

            if (SelectedType != null && !SelectedType.IsSubclassOf(BaseType)) 
            {
                SelectedType = null;
            }
        }
    }
}