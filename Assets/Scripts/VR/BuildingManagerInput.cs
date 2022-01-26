using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingManagerInput : MonoBehaviour
{
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;

    [SerializeField] private InputActionReference triggerLeftReference;
    [SerializeField] private InputActionReference triggerRightReference;
    [SerializeField] private InputActionReference spawnButtonReference;
    [SerializeField] private InputActionReference scaleJoystickReference;
    [SerializeField] private InputActionReference destroyButtonReference;
    [SerializeField] private InputActionReference toggleCollisionsButtonReference;

    [SerializeField] private float rayDistance = 5f;

    [SerializeField] private Material handMaterial;
    [SerializeField] private Material defaultHandMaterial;

    [SerializeField] private LayerMask layersToInteract;

    [SerializeField] private GameObject currentShape = null;
    [SerializeField] private GameObject[] shapeLibrary;
    [SerializeField] private GameObject[] handShapeLibrary;
    [SerializeField] private Transform shapeSpawnPoint;

    [SerializeField] private Collider grabBodyCollider;
    [SerializeField] private Collider playerBodyCollider;

    private ShapeManager shapeManager;

    private Color currentColor;

    private bool areCollidersOn = true;
    private bool isUsingColors = false;
    private bool canBuild = true;

    private int currentShapeIndex = 0;

    RaycastHit hit;

    private void Start()
    {
        shapeManager = GameManager.Instance.shapeManager;

        areCollidersOn = true;
        currentColor = Color.white;

        handMaterial.color = defaultHandMaterial.color;

        currentShape = shapeLibrary[currentShapeIndex];
        GameManager.Instance.SetShapeColor(currentShapeIndex, currentColor);

        GameManager.Instance.grabBodyCollider = grabBodyCollider;

        triggerLeftReference.action.performed += LeftTriggerInput;
        triggerRightReference.action.performed += RightTriggerInput;
        spawnButtonReference.action.performed += SpawnShape;
        scaleJoystickReference.action.performed += ScaleShape;
        destroyButtonReference.action.performed += DestroyShape;
        toggleCollisionsButtonReference.action.performed += ToggleCollisions;
    }

    private void Update()
    {
        Debug.DrawRay(leftHand.transform.position, leftHand.transform.TransformDirection(Vector3.forward), Color.yellow);
    }

    private void LeftTriggerInput(InputAction.CallbackContext _context)
    {
        if (Physics.Raycast(leftHand.transform.position, leftHand.transform.TransformDirection(Vector3.forward), out hit, rayDistance, layersToInteract, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.tag == "Color")
            {
                isUsingColors = true;
                currentColor = hit.collider.gameObject.GetComponent<MeshRenderer>().materials[0].color;
                GameManager.Instance.SetShapeColor(currentShapeIndex, currentColor);
                handMaterial.color = currentColor;
            }

            if (hit.collider.tag == "NoColor")
            {
                if (isUsingColors)
                {
                    handMaterial.color = defaultHandMaterial.color;
                    isUsingColors = false;
                }
            }

            if (hit.collider.tag == "Buildable" && isUsingColors)
            {
                hit.collider.gameObject.GetComponent<MeshRenderer>().materials[0].color = currentColor;
            }
        }
    }

    private void RightTriggerInput(InputAction.CallbackContext _context)
    {
        if (Physics.Raycast(rightHand.transform.position, rightHand.transform.TransformDirection(Vector3.forward), out hit, rayDistance, layersToInteract, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.tag == "Shape")
            {
                currentShapeIndex = hit.collider.gameObject.GetComponent<ShapeInformation>().index;
                currentShape = shapeLibrary[currentShapeIndex];
                GameManager.Instance.SetShapeColor(currentShapeIndex, currentColor);

                foreach (var item in handShapeLibrary)
                {
                    item.SetActive(false);
                }

                handShapeLibrary[currentShapeIndex].SetActive(true);
            }
        }
    }

    private void SpawnShape(InputAction.CallbackContext _context)
    {
        if (currentShape != null && canBuild)
        {
            GameObject spawnedObject = Instantiate(currentShape, shapeSpawnPoint.position, Quaternion.identity);
            spawnedObject.GetComponent<MeshRenderer>().materials[0].color = currentColor;
            shapeManager.AddShape(spawnedObject);
            Physics.IgnoreCollision(playerBodyCollider, spawnedObject.GetComponent<Collider>(), !areCollidersOn);
            Physics.IgnoreCollision(grabBodyCollider, spawnedObject.GetComponent<Collider>(), !areCollidersOn);
        }
    }

    private void ScaleShape(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();

        if (Physics.Raycast(rightHand.transform.position, rightHand.transform.TransformDirection(Vector3.forward), out hit, rayDistance, layersToInteract, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.tag == "Buildable")
            {
                ShapeInformation shapeInformation = hit.collider.GetComponent<ShapeInformation>();
                Vector2 scaleSpeed = direction / 15;

                Vector3 newScale = new Vector3();
                Transform test = hit.collider.transform;
                newScale.x = Mathf.Clamp(test.localScale.x - scaleSpeed.y, shapeInformation.minScale.x, shapeInformation.maxScale.x);
                newScale.y = Mathf.Clamp(test.localScale.y - scaleSpeed.y, shapeInformation.minScale.y, shapeInformation.maxScale.y);
                newScale.z = Mathf.Clamp(test.localScale.z - scaleSpeed.y, shapeInformation.minScale.z, shapeInformation.maxScale.z);
                test.localScale = newScale;
            }
        }
    }

    private void DestroyShape(InputAction.CallbackContext context)
    {
        if (Physics.Raycast(leftHand.transform.position, leftHand.transform.TransformDirection(Vector3.forward), out hit, rayDistance, layersToInteract, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.tag == "Buildable")
            {
                hit.collider.gameObject.GetComponent<ShapeHandler>().DestroyShape();
                shapeManager.RemoveShape(hit.collider.gameObject);
                Destroy(hit.collider.gameObject);
            }
        }
    }

    private void ToggleCollisions(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            areCollidersOn = !areCollidersOn;
            EventSystem<bool>.RaiseEvent(EventType.SHAPE_COLLIDING, areCollidersOn);

            shapeManager.ToggleCollisions(playerBodyCollider, grabBodyCollider, areCollidersOn);

            Debug.Log("Toggled Collisions");
        }
    }

    public void SaveGame(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            shapeManager.OnSave("Test");
        }
    }

    public void LoadGame(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            shapeManager.OnLoad("Test");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "NoBuildZone")
        {
            canBuild = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "NoBuildZone")
        {
            canBuild = true;
        }
    }
}
