using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingManagerInput_NonVR : MonoBehaviour
{
    [SerializeField] private float rayDistance = 5f;

    [SerializeField] private LayerMask layersToInteract;

    [SerializeField] private Transform holdDestination;

    [SerializeField] private GameObject currentShape = null;
    [SerializeField] private GameObject[] shapeLibrary;
    [SerializeField] private Transform shapeSpawnPoint;

    [SerializeField] private float scaleSpeedController = 0.025f;
    [SerializeField] private float scaleSpeedMouse = 0.25f;

    [SerializeField] private float rotateSpeedController = 0.025f;
    [SerializeField] private float rotateSpeedMouse = 0.25f;

    [SerializeField] private Collider grabBodyCollider;
    [SerializeField] private Collider playerBodyCollider;
    
    private GameObject pickupAttacher;

    private GameObject holdingShape = null;

    private ShapeManager shapeManager;

    public Color currentColor;

    private bool areCollidersOn = true;
    private bool isUsingColors = false;
    private bool canBuild = true;
    private bool isHolding = false;

    private bool canScale = false;
    private bool isScaling = false;

    private bool canRotate = false;
    private bool isRotating = false;

    private float curScaleInput = 0f;
    private Vector2 curRotateInput;

    private int currentShapeIndex = 0;

    private Camera cam;

    RaycastHit hit;

    private void Start()
    {
        canScale = false;
        isScaling = false;
        canRotate = false;
        isRotating = false;

        shapeManager = GameManager_NonVR.Instance.shapeManager;
        pickupAttacher = GameManager_NonVR.Instance.pickupAttacher;

        areCollidersOn = true;
        currentColor = Color.white;

        currentShape = shapeLibrary[currentShapeIndex];
        GameManager_NonVR.Instance.SetShapeColor(currentShapeIndex, currentColor);

        cam = Camera.main;
        EventSystem<bool>.Subscribe(EventType.IS_SCALING, ToggleScalingController);
        EventSystem<bool>.Subscribe(EventType.IS_ROTATING, ToggleRotatingController);
    }

    private void Update()
    {
        // Scaling
        if (canScale && isScaling)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayDistance, layersToInteract, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag == "Buildable")
                {
                    ShapeInformation shapeInformation = hit.collider.GetComponent<ShapeInformation>();

                    Vector3 newScale = new Vector3();
                    Transform test = hit.collider.transform;
                    float newScaleSpeed = curScaleInput * scaleSpeedController;

                    if (curScaleInput > 0)
                    {
                        newScale.x = Mathf.Clamp(test.localScale.x + newScaleSpeed, shapeInformation.minScale.x, shapeInformation.maxScale.x);
                        newScale.y = Mathf.Clamp(test.localScale.y + newScaleSpeed, shapeInformation.minScale.y, shapeInformation.maxScale.y);
                        newScale.z = Mathf.Clamp(test.localScale.z + newScaleSpeed, shapeInformation.minScale.z, shapeInformation.maxScale.z);
                        test.localScale = newScale;
                    }

                    if (curScaleInput < 0)
                    {
                        newScale.x = Mathf.Clamp(test.localScale.x - -newScaleSpeed, shapeInformation.minScale.x, shapeInformation.maxScale.x);
                        newScale.y = Mathf.Clamp(test.localScale.y - -newScaleSpeed, shapeInformation.minScale.y, shapeInformation.maxScale.y);
                        newScale.z = Mathf.Clamp(test.localScale.z - -newScaleSpeed, shapeInformation.minScale.z, shapeInformation.maxScale.z);
                        test.localScale = newScale;
                    }
                }
            }
        }

        // Rotating
        if (canRotate && isRotating)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayDistance, layersToInteract, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag == "Buildable")
                {
                    Transform test = hit.collider.transform;
                    Vector2 newRotateSpeed = curRotateInput * rotateSpeedController;

                    test.Rotate(newRotateSpeed.y, 0, -newRotateSpeed.x, Space.Self);
                }
            }
        }
    }

    public void SelectColor(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayDistance, layersToInteract, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag == "Color")
                {
                    isUsingColors = true;
                    currentColor = hit.collider.gameObject.GetComponent<MeshRenderer>().materials[0].color;
                    GameManager_NonVR.Instance.SetShapeColor(currentShapeIndex, currentColor);
                }

                if (hit.collider.tag == "NoColor")
                {
                    if (isUsingColors)
                    {
                        isUsingColors = false;
                    }
                }

                if (hit.collider.tag == "Buildable" && isUsingColors)
                {
                    hit.collider.gameObject.GetComponent<MeshRenderer>().materials[0].color = currentColor;
                }
            }
        }
    }

    public void SelectShape(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayDistance, layersToInteract, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag == "Shape")
                {
                    currentShapeIndex = hit.collider.gameObject.GetComponent<ShapeInformation>().index;
                    currentShape = shapeLibrary[currentShapeIndex];
                    GameManager_NonVR.Instance.SetShapeColor(currentShapeIndex, currentColor);
                }
            }
        }
    }

    public void SpawnShape(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
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
    }

    public void ScaleShapeMouse(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayDistance, layersToInteract, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag == "Buildable")
                {
                    ShapeInformation shapeInformation = hit.collider.GetComponent<ShapeInformation>();
                    float input = context.ReadValue<float>();

                    Vector3 newScale = new Vector3();
                    Transform test = hit.collider.transform;

                    if (input > 0)
                    {
                        newScale.x = Mathf.Clamp(test.localScale.x + scaleSpeedMouse, shapeInformation.minScale.x, shapeInformation.maxScale.x);
                        newScale.y = Mathf.Clamp(test.localScale.y + scaleSpeedMouse, shapeInformation.minScale.y, shapeInformation.maxScale.y);
                        newScale.z = Mathf.Clamp(test.localScale.z + scaleSpeedMouse, shapeInformation.minScale.z, shapeInformation.maxScale.z);
                        test.localScale = newScale;
                    }

                    if (input < 0)
                    {
                        newScale.x = Mathf.Clamp(test.localScale.x - scaleSpeedMouse, shapeInformation.minScale.x, shapeInformation.maxScale.x);
                        newScale.y = Mathf.Clamp(test.localScale.y - scaleSpeedMouse, shapeInformation.minScale.y, shapeInformation.maxScale.y);
                        newScale.z = Mathf.Clamp(test.localScale.z - scaleSpeedMouse, shapeInformation.minScale.z, shapeInformation.maxScale.z);
                        test.localScale = newScale;
                    }
                }
            }
        }
    }

    public void ScaleShapeController(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && canScale)
        {
            isScaling = true;
            curScaleInput = context.ReadValue<float>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isScaling = false;
            curScaleInput = 0f;
        }
    }

    public void RotateShape(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && canRotate)
        {
            isRotating = true;
            curRotateInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isRotating = false;
            curRotateInput = Vector2.zero;
        }
    }

    public void PickupShape(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (!isHolding)
            {
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayDistance, layersToInteract, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.tag == "Buildable")
                    {
                        if (!hit.collider.GetComponent<ShapeInformation>().isBeingHeld)
                        {
                            isHolding = true;
                            holdingShape = hit.collider.gameObject;
                            pickupAttacher.transform.position = hit.point;
                            holdingShape.transform.parent = pickupAttacher.transform;

                            holdingShape.GetComponent<ShapeInformation>().isBeingHeld = true;
                            pickupAttacher.transform.position = holdDestination.position;
                            pickupAttacher.transform.parent = holdDestination;
                        }
                    }
                }
            }
            else if (isHolding)
            {
                isHolding = false;
                holdingShape.GetComponent<ShapeInformation>().isBeingHeld = false;
                holdingShape.transform.parent = null;
            }
        }
    }

    public void DestroyShape(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayDistance, layersToInteract, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag == "Buildable")
                {
                    hit.collider.gameObject.GetComponent<ShapeHandler>().DestroyShape();
                    shapeManager.RemoveShape(hit.collider.gameObject);
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    public void ToggleCollisions(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            areCollidersOn = !areCollidersOn;
            EventSystem<bool>.RaiseEvent(EventType.SHAPE_COLLIDING, areCollidersOn);

            shapeManager.ToggleCollisions(playerBodyCollider, grabBodyCollider, areCollidersOn);
        }
    }

    public void ToggleScalingController(bool _isScaling)
    {
        canScale = _isScaling;
        isScaling = false;
        curScaleInput = 0f;
    }

    public void ToggleRotatingController(bool _isRotating)
    {
        canRotate = _isRotating;
        isRotating = false;
        curRotateInput = Vector2.zero;
    }

    // Save & Load
    public void SaveGame(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            shapeManager.OnSave(PersistentSaveName.saveName);
            Debug.Log("Saved Game");
        }
    }

    public void LoadGame(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            shapeManager.OnLoad(PersistentSaveName.saveName);
            Debug.Log("Loaded Game");
        }
    }

    // Collisions & Triggers
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
