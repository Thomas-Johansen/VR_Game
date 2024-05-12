using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    private bool _isFlying = false;
    private bool _doJump = false;
    private int _walkSpeed = 500;
    private int _jumpStrength = 250;


    public Rigidbody playerBody;
   
    [SerializeField] private Camera playerCamera;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction _leftMoveAction;
    private Vector2 _leftMoveVector;
    private InputAction _leftPrimaryAction;
    private bool _leftPrimary;
    private InputAction _leftSecondaryAction;
    private bool _leftSecondary;
    private InputAction _rightMoveAction;
    private Vector2 _rightMoveVector;


    private void Awake()
    {
        // Bindings
        // Headset
        
        // Right Hand
        InputActionMap rightHandLocomotion = inputActions.FindActionMap("XRI RightHand Locomotion");
        _rightMoveAction = rightHandLocomotion.FindAction("Move");
        _rightMoveAction.Enable();
        _rightMoveAction.performed += OnRightMovementPerformed;
        _rightMoveAction.canceled += OnRightMovementCanceled;
        
        // Left Hand
        InputActionMap leftHandLocomotion = inputActions.FindActionMap("XRI LeftHand Locomotion");
        _leftMoveAction = leftHandLocomotion.FindAction("Move");
        _leftMoveAction.Enable();
        _leftMoveAction.performed += OnLeftMovementPerformed;
        _leftMoveAction.canceled += OnLeftMovementCanceled;
        _leftPrimaryAction = leftHandLocomotion.FindAction("X Button");
        _leftPrimaryAction.Enable();
        _leftPrimaryAction.performed += OnLeftPrimaryPerformed;
        _leftPrimaryAction.canceled += OnLeftPrimaryCanceled;
        _leftSecondaryAction = leftHandLocomotion.FindAction("Y Button");
        _leftSecondaryAction.Enable();
        _leftSecondaryAction.performed += OnLeftSecondaryPerformed;
        _leftSecondaryAction.canceled += OnLeftSecondaryCanceled;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void FixedUpdate()
    {

        if (!_isFlying) // Walking Code
        {
            if (_leftMoveVector != Vector2.zero)
            {
                Vector3 playerUp = transform.up;
                Vector3 direction = playerCamera.transform.forward;
                Vector3 right = Vector3.Cross(playerUp, direction).normalized;
                Vector3 correctedDirection = Vector3.Cross(right, playerUp).normalized;
        
                float forwards = _leftMoveVector.y;
                float sideways = _leftMoveVector.x;
                Vector3 movement = (correctedDirection * forwards + right * sideways).normalized;
                playerBody.AddForce((movement * (Time.fixedDeltaTime * _walkSpeed))); 
            }

            if (_doJump)
            {
                _doJump = false;
                Debug.Log("Jumping");
                playerBody.AddForce(transform.up * _jumpStrength);
            }
            
        }
        else // Flying Code
        {
            
        }
        
        //Universal Controls
        
    }

    //Right Hand
    private void OnRightMovementPerformed(InputAction.CallbackContext context)
    {
        _rightMoveVector = context.ReadValue<Vector2>();
    }
    
    private void OnRightMovementCanceled(InputAction.CallbackContext context)
    {
        _rightMoveVector = Vector2.zero;
    }
    
    //LeftHand
    private void OnLeftMovementPerformed(InputAction.CallbackContext context)
    {
        _leftMoveVector = context.ReadValue<Vector2>();
    }
    
    private void OnLeftMovementCanceled(InputAction.CallbackContext context)
    {
        _leftMoveVector = Vector2.zero;
    }
    
    private void OnLeftPrimaryPerformed(InputAction.CallbackContext context)
    {
        _leftPrimary = true;
        _doJump = true;
    }
    
    private void OnLeftPrimaryCanceled(InputAction.CallbackContext context)
    {
        _leftPrimary = false;
        _doJump = false; //Edge case
    }
    
    private void OnLeftSecondaryPerformed(InputAction.CallbackContext context)
    {
        _leftSecondary = true;
        
    }
    
    private void OnLeftSecondaryCanceled(InputAction.CallbackContext context)
    {
        _leftSecondary = false;
    }
    
}
