using UnityEngine;
using System;
using System.Collections;
using System.Linq;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    //The name of the bool field that will be in control
    public string ConditionalSourceField = "";
    //TRUE = Hide in inspector / FALSE = Disable in inspector 
    public bool HideInInspector = false;

    public readonly string[] CompareValues = null;

    public ConditionalHideAttribute(string conditionalSourceField)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = false;
    }

    //public ConditionalHideAttribute(string fieldToCheck, bool hideInInspector)
    //{
    //    this.ConditionalSourceField = fieldToCheck;
    //    this.HideInInspector = hideInInspector;
    //}

    public ConditionalHideAttribute(string fieldToCheck, params object[] compareValues)
    {
        this.ConditionalSourceField = fieldToCheck;
        this.HideInInspector = false;
        this.CompareValues = compareValues.Select(c => c.ToString().ToUpper()).ToArray();
    }
}