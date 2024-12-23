using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementDatabase", menuName = "Inventory/ElementDatabase")]
public class ElementDatabase : ScriptableObject
{
    [System.Serializable]
    public class Element
    {
        public string elementName;
        public GameObject element3D;
        public GameObject elementUI;
    }

    public List<Element> elements;

    public Element GetElementByName(string name)
    {
        return elements.Find(e => e.elementName == name);
    }
}
