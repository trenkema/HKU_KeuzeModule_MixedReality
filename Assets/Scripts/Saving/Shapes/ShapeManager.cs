using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
    [SerializeField] private GameObject[] shapeLibrary;
    [SerializeField] private List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        OnLoad(PersistentSaveName.saveName);
    }

    public void OnSave(string _saveName)
    {
        SerializationManager.Save(_saveName, SaveData.current);
    }

    public void OnLoad(string _saveName)
    {
        Debug.Log("SaveName: " + _saveName);

        DestroyCurrentObjects();

        SaveData.current = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/" + _saveName + ".save");

        for (int i = 0; i < SaveData.current.allShapes.Count; i++)
        {
            ShapeData currentShape = SaveData.current.allShapes[i];
            GameObject obj = Instantiate(shapeLibrary[(int)currentShape.shapeType]);

            spawnedObjects.Add(obj);
            ShapeHandler shapeHandler = obj.GetComponent<ShapeHandler>();
            shapeHandler.shapeData = currentShape;

            // Set Shape Transform
            shapeHandler.transform.position = currentShape.position;
            shapeHandler.transform.rotation = currentShape.rotation;
            shapeHandler.transform.localScale = currentShape.scale;

            // Set Shape Color
            Color32 shapeColor = new Color32();
            shapeColor.r = currentShape.r;
            shapeColor.g = currentShape.g;
            shapeColor.b = currentShape.b;
            obj.GetComponent<MeshRenderer>().materials[0].color = shapeColor;
        }
    }

    public void DestroyCurrentObjects()
    {
        foreach (var item in spawnedObjects)
        {
            Destroy(item);
        }

        spawnedObjects.Clear();
    }

    public void RemoveShape(GameObject _shapeObject)
    {
        spawnedObjects.Remove(_shapeObject);
    }

    public void AddShape(GameObject _shapeObject)
    {
        spawnedObjects.Add(_shapeObject);
    }

    public void ToggleCollisions(Collider _playerBodyCollider, Collider _grabBodyCollider, bool _areCollidersOn)
    {
        foreach (var item in spawnedObjects)
        {
            Physics.IgnoreCollision(_playerBodyCollider, item.GetComponent<Collider>(), !_areCollidersOn);
            Physics.IgnoreCollision(_grabBodyCollider, item.GetComponent<Collider>(), !_areCollidersOn);
        }
    }

    private void OnDisable()
    {
        OnSave(PersistentSaveName.saveName);
    }
}
