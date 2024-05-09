using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 _currentSpeed;
    private Vector3 _deltaSpeed;

    private int _walkSpeed = 100;


    public Rigidbody playerBody;
    public Transform rotationTransform;
   
    [SerializeField] private Camera playerCamera;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction _moveAction;
    private Vector2 _moveVector;


    private void Awake()
    {
        InputActionMap actionMap = inputActions.FindActionMap("XRI LeftHand Locomotion");
        _moveAction = actionMap.FindAction("Move");
        _moveAction.Enable();
        _moveAction.performed += OnMovementPerformed;
        _moveAction.canceled += OnMovementCanceled;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float currentRotation = rotationTransform.eulerAngles.y;
        float yawRotation = playerCamera.transform.eulerAngles.y;
        float newRotation = yawRotation - currentRotation;
        //rotationTransform.Rotate(0, newRotation, 0);

        float xRot = transform.eulerAngles.x;
        float yRot = transform.eulerAngles.y;
        float zRot = transform.eulerAngles.z;
        Quaternion newRotation2 = Quaternion.Euler(xRot, (yRot + yawRotation), zRot);
        rotationTransform.rotation = newRotation2;
        
        
   
        
        
        Vector3 right = playerCamera.transform.right;
        Vector3 direction = playerCamera.transform.forward;
        direction.y = 0;
        direction.Normalize();
        direction += rotationTransform.transform.forward;
        direction.Normalize();
        
        float forwards = _moveVector.y;
        float sideways = _moveVector.x;
        Vector3 movement = (direction * forwards + right * sideways);
        playerBody.AddForce((movement));
        
       
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        _moveVector = context.ReadValue<Vector2>();
    }
    
    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        _moveVector = Vector2.zero;
    }
    
}
