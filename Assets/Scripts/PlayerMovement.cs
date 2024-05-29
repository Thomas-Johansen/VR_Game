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
    private float _maxSpeed = 10;
    public float brakeStrength = 5.0f;
    private float _gravityForce = 490.5f;
    private double _hoverNumber = 0;
    private float _pitchSpeed = 30f;
    private float _rollSpeed = 30f;


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
    private InputAction _rightPrimaryAction;
    private bool _rightPrimary;
    private InputAction _rightSecondaryAction;
    private bool _rightSecondary;


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
        _rightPrimaryAction = rightHandLocomotion.FindAction("A Button");
        _rightPrimaryAction.Enable();
        _rightPrimaryAction.performed += OnRightPrimaryPerformed;
        _rightPrimaryAction.canceled += OnRightPrimaryCanceled;
        _rightSecondaryAction = rightHandLocomotion.FindAction("B Button");
        _rightSecondaryAction.Enable();
        _rightSecondaryAction.performed += OnRightSecondaryPerformed;
        _rightSecondaryAction.canceled += OnRightSecondaryCanceled;
        
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
            //Multiple use varables
            
            
            
            //Gravity
            Vector3 planetCentre = new Vector3(0, 0, 0);

            // Calculate the direction from the player to the planet
            Vector3 gravityDirection = (planetCentre - transform.position).normalized;

            // Calculate the target up vector (opposite of gravity direction)
            Vector3 targetUp = -gravityDirection;

            // Rotate the player to align its up vector with the target up vector
            UnityEngine.Quaternion targetRotation = UnityEngine.Quaternion.FromToRotation(transform.up, targetUp) * transform.rotation;

            // Apply the rotation smoothly (optional)
            //transform.rotation = UnityEngine.Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
            transform.rotation = targetRotation;
            
            playerBody.AddForce(gravityDirection * (Time.fixedDeltaTime * _gravityForce));
            
            //Movement
            if (_leftMoveVector != Vector2.zero && playerBody.velocity.magnitude < _maxSpeed)
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
                playerBody.AddForce(transform.up * _jumpStrength);
            }
            
        }
        else // Flying Code
        {
            //Break
            if (playerBody.velocity.magnitude > 2)
            {
                playerBody.velocity =
                    Vector3.Lerp(playerBody.velocity, Vector3.zero, brakeStrength * Time.fixedDeltaTime);
            }
            //Hover
            else if (playerBody.velocity.magnitude < 2)
            {
                playerBody.velocity = (transform.up * ((float)Math.Sin(_hoverNumber) * 0.1f));
                _hoverNumber += 0.05;
                if (_hoverNumber >= 2 * Mathf.PI)
                {
                    _hoverNumber -= 2 * Mathf.PI;
                }
            }
            
            //Pitch and roll
            if (_rightMoveVector != Vector2.zero)
            {
                // Get the pitch and roll from the thumbstick input
                float pitch = _rightMoveVector.y * _pitchSpeed * Time.fixedDeltaTime;
                float roll = _rightMoveVector.x * _rollSpeed * Time.fixedDeltaTime;
                
                // Get the camera's forward and right vectors
                Vector3 cameraForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, playerBody.transform.up).normalized;
                Vector3 cameraRight = Vector3.ProjectOnPlane(playerCamera.transform.right, playerBody.transform.up).normalized;
                

                // Normalize the vectors
                cameraForward.Normalize();
                cameraRight.Normalize();
                
                // Calculate pitch and roll rotations
                UnityEngine.Quaternion pitchRotation = UnityEngine.Quaternion.AngleAxis(pitch, cameraRight);
                UnityEngine.Quaternion rollRotation = UnityEngine.Quaternion.AngleAxis(roll, -cameraForward);
                
                playerBody.MoveRotation(playerBody.rotation * pitchRotation * rollRotation);
            }
                
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
    
    private void OnRightPrimaryPerformed(InputAction.CallbackContext context)
    {
        _rightPrimary = true;
        _doJump = true;
    }

    private void OnRightPrimaryCanceled(InputAction.CallbackContext context)
    {
        _rightPrimary = false;
        _doJump = false; //Edge case
    }

    private void OnRightSecondaryPerformed(InputAction.CallbackContext context)
    {
        _rightSecondary = true;
        _isFlying = !_isFlying;
    }

    private void OnRightSecondaryCanceled(InputAction.CallbackContext context)
    {
        _rightSecondary = false;
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
    }
    
    private void OnLeftPrimaryCanceled(InputAction.CallbackContext context)
    {
        _leftPrimary = false;
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
