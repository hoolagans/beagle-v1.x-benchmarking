using System;
using System.Collections;
using System.Collections.Generic;

//using LAShared;

namespace BrowserEmulator;

/// <summary>
/// The AttributeList class is used to store list of
/// Attribute classes.
/// This source code may be used freely under the
/// Limited GNU Public License(LGPL).
///
/// Written by Jeff Heaton (http://www.jeffheaton.com)
/// </summary>
///
public class AttributeList : Attribute
{
    /// <summary>
    /// An internally used Vector.  This vector contains
    /// the entire list of attributes.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    protected ArrayList m_list;
    /// <summary>
    /// Make an exact copy of this object using the cloneable
    /// interface.
    /// </summary>
    /// <returns>A new object that is a clone of the specified
    /// object.</returns>
    public override object Clone()
    {
        AttributeList rtn = new AttributeList();

        for (int i = 0; i < m_list.Count; i++)
            rtn.Add((Attribute)this[i].Clone());

        return rtn;
    }

    /// <summary>
    /// Create a new, empty, attribute list.
    /// </summary>
    public AttributeList(): base("", "")
    {
        m_list = new ArrayList();
    }

    /// <summary>
    /// Add the specified attribute to the list of attributes.
    /// </summary>
    /// <param name="a">An attribute to add to this
    /// AttributeList.
    /// </param>
    public void Add(Attribute a) {
        m_list.Add(a);
    }


    /// <summary>
    /// Clear all attributes from this AttributeList and return
    /// it to a empty state.
    /// </summary>
    public void Clear()
    {
        m_list.Clear();
    }

    /// <summary>
    /// Returns true of this AttributeList is empty, with no
    /// attributes.
    /// </summary>
    /// <returns>True if this AttributeList is empty, false
    /// otherwise.</returns>
    public bool IsEmpty()
    {
        return (m_list.Count <= 0);
    }

    /// <summary>
    /// If there is already an attribute with the specified name,
    /// it will have its value changed to match the specified
    /// value. If there is no Attribute with the specified name,
    /// one will be created. This method is case-insensitive.
    /// </summary>
    /// <param name="name">The name of the Attribute to edit or
    /// create.  Case-insensitive.</param>
    /// <param name="value">The value to be held in this
    /// attribute.</param>
    public void Set(string name, string value)
    {
        if (name == null) return;
        if (value == null) value = "";

        Attribute a = this[name];

        if (a == null)
        {
            a = new Attribute(name, value);
            Add(a);
        }
        else
        {
            a.Value = value;
        }
    }

    /// <summary>
    /// How many attributes are in this AttributeList?
    /// </summary>
    public int Count
    {
        get { return m_list.Count; }
    }

    /// <summary>
    /// A list of the attributes in this AttributeList
    /// </summary>
    public ArrayList List
    {
        get { return m_list; }
    }

    /// <summary>
    /// Access the individual attributes
    /// </summary>
    public Attribute this[int index]
    {
        get
        {
            if (index < m_list.Count) return (Attribute)m_list[index];
            else return null;
        }
    }

    /// <summary>
    /// Access the individual attributes by name.
    /// </summary>
    public Attribute this[string index]
    {
        get
        {
            int i = 0;

            while (this[i] != null)
            {
                if (this[i].Name.ToLower().Equals((index.ToLower()))) return this[i];
                i++;
            }
            return null;
        }
    }

    public string GetAttributeValue(string name) 
    {
        Attribute attr = this[name];
        if (attr == null) throw new Exception("Attribute '" + name + "' does not exist in tag <" + Name + ">");
        return attr.Value;
    }
    public bool Matches(string tagName, object attributesObj)
    {
        return Matches(tagName, AttributesHelper.AnonymousObjectToHtmlAttributes(attributesObj));
    }
    public bool Matches(string tagName, IDictionary<string, string> attributesDict)
    {
        if (tagName != null && Name.ToLower() != tagName.ToLower()) return false;
        if (attributesDict != null)
        {
            foreach (var attribute in attributesDict)
            {
                var myAttr = this[attribute.Key];
                if (myAttr == null) return false;
                if (myAttr.Value.Trim().ToLower() != attribute.Value.Trim().ToLower()) return false;
            }
        }
        return true;
    }
    public void EnsureMatches(string tagName, object attributesObj)
    {
        EnsureMatches(tagName, AttributesHelper.AnonymousObjectToHtmlAttributes(attributesObj));
    }
    public void EnsureMatches(string tagName, IDictionary<string, string> attributesDict)
    {
        if (tagName != null && Name.ToLower() != tagName.ToLower()) throw new BrowserEmulatorException("<" + Name + "> where <" + tagName + "> is expected");
        if (attributesDict != null)
        {
            foreach (var attribute in attributesDict)
            {
                var myAttr = this[attribute.Key];
                if (myAttr == null) throw new BrowserEmulatorException("<" + Name + "> with '" + attribute.Key + "' attribute is expected");
                if (myAttr.Value.Trim().ToLower() != attribute.Value.Trim().ToLower()) throw new BrowserEmulatorException("<" + Name + "> with '" + attribute.Key + "'='" + attribute.Value + "' attribute is expected");
            }
        }
    }
}