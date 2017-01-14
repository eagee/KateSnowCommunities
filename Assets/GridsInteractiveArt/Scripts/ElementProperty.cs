using UnityEngine;

[System.Serializable]
public struct ElementProperty
{
    public string ID;
    public short shapeFrame;
    public short shapePaletteIndex;
    public ElementProperty(string id, short frame)
    {
        ID = id;
        shapeFrame = frame;
        shapePaletteIndex = 1;
    }
}