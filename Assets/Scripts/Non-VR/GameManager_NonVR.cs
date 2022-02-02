using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_NonVR : MonoBehaviour
{
    public static GameManager_NonVR Instance { get; private set; }

    public Material[] wallShapeMaterialLibrary;
    public Material defaultMaterial;

    public GameObject pickupAttacher;

    public ShapeManager shapeManager;

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
            wallShapeMaterialLibrary[i].color = defaultMaterial.color;
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
