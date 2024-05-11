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
        
        Vector3 playerUp = transform.up;
        Vector3 direction = playerCamera.transform.forward;
        Vector3 right = Vector3.Cross(playerUp, direction).normalized;
        Vector3 correctedDirection = Vector3.Cross(right, playerUp).normalized;
        
        float forwards = _moveVector.y;
        float sideways = _moveVector.x;
        Vector3 movement = (correctedDirection * forwards + right * sideways);
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
