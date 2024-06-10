using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    private bool _isFlying = false;
    private bool _doJump = false;
    private double _hoverNumber = 0;
    private bool _attackActive = false;
    private float _chargeLevel = 0;
    private Vector3 _initialDirection;
    
    [SerializeField]private int _walkSpeed = 500;
    [SerializeField]private int _jumpStrength = 250;
    [SerializeField]private float _maxSpeed = 10;
    [SerializeField]private float _brakeStrength = 0.5f;
    [SerializeField]private float _hardbrakeStrength = 2.5f;
    [SerializeField]private float _gravityForce = 490.5f;
    [SerializeField]private float _pitchSpeed = 30f;
    [SerializeField]private float _rollSpeed = 30f;
    [SerializeField]private float _manoverSpeed = 100f;
    [SerializeField]private float _flightSpeed = 200f;
    [SerializeField]private float _projectileSpeed = 10f;

    public Rigidbody playerBody;
    public Transform leftController;
    public Transform rightController;
    public GameObject Energy1;
    public GameObject Energy2;
    public GameObject Energy3;
    
    
   
    [SerializeField] private Camera playerCamera;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction _leftMoveAction;
    private Vector2 _leftMoveVector;
    private InputAction _leftPrimaryAction;
    private bool _leftPrimary;
    private InputAction _leftSecondaryAction;
    private bool _leftSecondary;
    private InputAction _leftGrabAction;
    private bool _leftGrab;
    private InputAction _leftTriggerAction;
    private bool _leftTrigger;
    private InputAction _rightMoveAction;
    private Vector2 _rightMoveVector;
    private InputAction _rightPrimaryAction;
    private bool _rightPrimary;
    private InputAction _rightSecondaryAction;
    private bool _rightSecondary;
    private InputAction _rightGrabAction;
    private bool _rightGrab;
    private InputAction _rightTriggerAction;
    private bool _rightTrigger;
    
    


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
        _rightGrabAction = rightHandLocomotion.FindAction("Grab Move");
        _rightGrabAction.Enable();
        _rightGrabAction.performed += OnRightGrabPerformed;
        _rightGrabAction.canceled += OnRightGrabCanceled;
        _rightTriggerAction = rightHandLocomotion.FindAction("Trigger");
        _rightTriggerAction.Enable();
        _rightTriggerAction.performed += OnRightTriggerPerformed;
        _rightTriggerAction.canceled += OnRightTriggerCanceled;
        
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
        _leftGrabAction = leftHandLocomotion.FindAction("Grab Move");
        _leftGrabAction.Enable();
        _leftGrabAction.performed += OnLeftGrabPerformed;
        _leftGrabAction.canceled += OnLeftGrabCanceled;
        _leftTriggerAction = leftHandLocomotion.FindAction("Trigger");
        _leftTriggerAction.Enable();
        _leftTriggerAction.performed += OnLeftTriggerPerformed;
        _leftTriggerAction.canceled += OnLeftTriggerCanceled;

    }

    // Start is called before the first frame update
    void Start()
    { 
        Energy1.SetActive(false);
        Energy2.SetActive(false);
        Energy3.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_attackActive)
        {
            if (_leftTrigger)
            {
                if (_chargeLevel < 0.2)
                {
                    _chargeLevel += 0.0001f;
                    Energy1.SetActive(true);
                    Energy1.transform.localScale = new Vector3(_chargeLevel, _chargeLevel, _chargeLevel);   
                }
            }
            else if (_chargeLevel >= 0.2)
            {
                _attackActive = true;
                _initialDirection = playerCamera.transform.forward;
                Energy2.transform.position = playerBody.transform.position + (_initialDirection * 2);
                Energy2.SetActive(true);
                Energy3.SetActive(true);
            }
            else if(_chargeLevel > 0)
            {
                _chargeLevel -= 0.0001f;
                Energy1.transform.localScale = new Vector3(_chargeLevel, _chargeLevel, _chargeLevel);
                if (_chargeLevel <= 0)
                {
                    Energy1.SetActive(false);
                }
            }
        }
        else
        {
            float energySize =  (leftController.position - rightController.position).magnitude;
            
            Energy2.transform.position += _initialDirection * (Time.deltaTime * _projectileSpeed);
            
            //Moves the beam part
            Vector3 midpoint = (Energy1.transform.position + Energy2.transform.position) / 2;
            Vector3 direction = Energy2.transform.position - Energy1.transform.position;
            Energy3.transform.position = midpoint;
            Energy3.transform.up = direction;
            Energy3.transform.localScale = new Vector3(energySize, direction.magnitude * 0.5f, energySize);
            
            Energy1.transform.localScale = new Vector3(energySize, energySize, energySize);

            if (_leftTrigger)
            {
                _attackActive = false;
                Energy2.SetActive(false);
                Energy3.SetActive(false);
                _chargeLevel = 0.19f;
            }
        }
        
        
        
        //Ki blast
        Vector3 center = (leftController.transform.position + rightController.transform.position) / 2;
        Energy1.transform.position = center;
        
       


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
            transform.rotation = UnityEngine.Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
            //transform.rotation = targetRotation;
            
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
            bool forwardsBool = _rightGrab && _leftGrab;
            
            //Break
            if (playerBody.velocity.magnitude > 1 && _leftMoveVector == Vector2.zero && !forwardsBool)
            {
                playerBody.velocity =
                    Vector3.Lerp(playerBody.velocity, Vector3.zero, _brakeStrength * Time.fixedDeltaTime);
            }
            //Hover
            else if (playerBody.velocity.magnitude < 1 && _leftMoveVector == Vector2.zero && !forwardsBool)
            {
                playerBody.velocity = (transform.up * ((float)Math.Sin(_hoverNumber) * 0.1f));
                _hoverNumber += 0.05;
                if (_hoverNumber >= 2 * Mathf.PI)
                {
                    _hoverNumber -= 2 * Mathf.PI;
                }
            }
            //Hard Brake
            if (_rightPrimary)
            {
                playerBody.velocity =
                    Vector3.Lerp(playerBody.velocity, Vector3.zero, _hardbrakeStrength * Time.fixedDeltaTime);
            }
            
            //Pitch and roll
            if (_rightMoveVector != Vector2.zero)
            {
                // Get the pitch and roll from the thumbstick input
                float pitch = _rightMoveVector.y * _pitchSpeed * Time.fixedDeltaTime;
                float roll = _rightMoveVector.x * _rollSpeed * Time.fixedDeltaTime;
                
                transform.RotateAround(transform.position, playerCamera.transform.forward, -roll);
                transform.RotateAround(transform.position, playerCamera.transform.right, pitch);
            }
            //Up down left right
            if (_leftMoveVector != Vector2.zero)
            {
                float forwards = _leftMoveVector.y;
                float sideways = _leftMoveVector.x;
                
                Vector3 movement = (playerCamera.transform.up * forwards + playerCamera.transform.right * sideways).normalized;
                playerBody.AddForce((movement * (Time.fixedDeltaTime * _manoverSpeed))); 
            }
            //Forwards movement
            if (forwardsBool)
            {
                Vector3 midpoint = (leftController.position + rightController.position) / 2;
                Vector3 throttle = (midpoint - transform.position);
                float throttleFloat = throttle.magnitude;
                playerBody.AddForce((playerCamera.transform.forward * ((Time.fixedDeltaTime * _flightSpeed) * (throttleFloat * throttleFloat * throttleFloat)))); 
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

    private void OnRightGrabPerformed(InputAction.CallbackContext context)
    {
        _rightGrab = true;
    }
    
    private void OnRightGrabCanceled(InputAction.CallbackContext context)
    {
        _rightGrab = false;
    }
    
    private void OnRightTriggerPerformed(InputAction.CallbackContext context)
    {
        _rightTrigger = true;
    }
    
    private void OnRightTriggerCanceled(InputAction.CallbackContext context)
    {
        _rightTrigger = false;
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
    
    private void OnLeftGrabPerformed(InputAction.CallbackContext context)
    {
        _leftGrab = true;
    }
    
    private void OnLeftGrabCanceled(InputAction.CallbackContext context)
    {
        _leftGrab = false;
    }
    
    private void OnLeftTriggerPerformed(InputAction.CallbackContext context)
    {
        _leftTrigger = true;
    }
    
    private void OnLeftTriggerCanceled(InputAction.CallbackContext context)
    {
        _leftTrigger = false;
    }
    
}
