using UnityEngine;

[System.Serializable]
public struct ElementProperty
{
    public string ID;
    public int shapeFrame;
    public Color shapeColor;
    public ElementProperty(string id, int frame)
    {
        ID = id;
        shapeFrame = frame;
        shapeColor = new Color(1.0f, 1.0f, 1.0f);
    }
}