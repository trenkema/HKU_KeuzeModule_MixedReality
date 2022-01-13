using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ShapeType
{
    Cube,
    Sphere,
    Cylinder
}

[System.Serializable]
public class ShapeData
{
    public string id;
    public ShapeType shapeType;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public byte r, g, b;
}
