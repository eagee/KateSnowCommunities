using UnityEngine;

[System.Serializable]
public struct ElementProperty
{
    public byte ID;
    public byte shapeFrame;
    public byte shapePaletteIndex;
    public ElementProperty(byte id, byte frame)
    {
        ID = id;
        shapeFrame = frame;
        shapePaletteIndex = 1;
    }
}