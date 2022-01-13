using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeHandler : MonoBehaviour
{
    public ShapeType shapeType;
    public ShapeData shapeData;
    public Color32 shapeColor;
    public MeshRenderer meshRenderer;

    private void Start()
    {
        if (string.IsNullOrEmpty(shapeData.id))
        {
            shapeData.id = System.DateTime.Now.ToLongDateString() + System.DateTime.Now.ToLongTimeString() + Random.Range(0, int.MaxValue).ToString();
            shapeData.shapeType = shapeType;
            SaveData.current.allShapes.Add(shapeData);
        }
    }

    private void Update()
    {
        shapeData.position = transform.position;
        shapeData.rotation = transform.rotation;
        shapeData.scale = transform.localScale;
        shapeColor = meshRenderer.materials[0].color;
        shapeData.r = shapeColor.r;
        shapeData.g = shapeColor.g;
        shapeData.b = shapeColor.b;
    }

    public void DestroyShape()
    {
        SaveData.current.allShapes.Remove(shapeData);
    }
}
