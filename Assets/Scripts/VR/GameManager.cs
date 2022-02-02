using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Material[] wallShapeMaterialLibrary;
    public Material defaultMaterial;

    public ShapeManager shapeManager;

    public Collider grabBodyCollider;

    private bool areCollidersOn = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        areCollidersOn = true;

        EventSystem<bool>.Subscribe(Event_Type.SHAPE_COLLIDING, SetShapeColliding);
    }

    private void SetShapeColliding(bool _areCollidersOn)
    {
        areCollidersOn = _areCollidersOn;
    }

    public bool GetShapeColliding()
    {
        return areCollidersOn;
    }

    public void SetShapeColor(int _index, Color _color)
    {
        for (int i = 0; i < wallShapeMaterialLibrary.Length; i++)
        {
            wallShapeMaterialLibrary[i] = defaultMaterial;
        }

        wallShapeMaterialLibrary[_index].color = _color;
    }

    private void OnDisable()
    {
        for (int i = 0; i < wallShapeMaterialLibrary.Length; i++)
        {
            wallShapeMaterialLibrary[i].color = defaultMaterial.color;
        }
    }
}
