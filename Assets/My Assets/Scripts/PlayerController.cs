using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Input
    private PlayerInputActions _inputActions;

    //Movement
    [Header("Movement")]
    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _walkSpeed, _walkBackSpeed;
    [SerializeField] private float _runSpeed, _runBackSpeed;
    [SerializeField] private float _crouchSpeed, _crouchBackSpeed;   
    private CharacterController _characterController;  
    private Vector3 _moveDirection;
    private bool _isWalking;
    private bool _isSprinting;
    private bool _isCrouching;

    //Gravity
    [Header("Gravity")]
    [SerializeField] private float _gravityMultiplier;
    [SerializeField] private float _velocityY;
    private float _gravity = -9.81f;

    //Jump
    [Header("Jump")]
    [SerializeField] private float _jumpForce;
    
    //Look
    [Header("Look")]
    [SerializeField] private Transform _centerSpinePos;
    [Range(0.0f, 100.0f)] [SerializeField] private float _lookSensitivity;
    private float _xRot;
    private float _yRot;

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleInputAndMove();
        HandleLook();
        HandleGravity();
        HandleJump();
        HandleSpeed();
    }

    private void HandleSpeed()
    {
        //get inputVector
        Vector2  inputVector = _inputActions.Player.Move.ReadValue<Vector2>();

        //set bools
        if (_inputActions.Player.Sprint.IsPressed())
        {
            _isSprinting = true;
            _isCrouching = false;
            _isWalking = false;
        } else if (_inputActions.Player.Crouch.IsPressed())
        {
            _isCrouching = true;
            _isSprinting = false;
            _isWalking = false;
        } else
        {
            _isWalking = true;
            _isCrouching = false;
            _isSprinting = false;
        }

        //handle speeds
        if (_isSprinting && inputVector.y > 0) _currentSpeed = _runSpeed;
        else if (_isSprinting && inputVector.y < 0) _currentSpeed = _runBackSpeed;
        if (_isWalking && inputVector.y > 0) _currentSpeed = _walkSpeed;
        else if (_isWalking && inputVector.y < 0) _currentSpeed = _walkBackSpeed;
        if (_isCrouching && inputVector.y > 0) _currentSpeed = _crouchSpeed;
        else if (_isCrouching && inputVector.y < 0) _currentSpeed = _crouchBackSpeed;
    }

    private void HandleGravity()
    {
        if(_characterController.isGrounded && _velocityY < 0)
        {
            _velocityY = -1f;
        }else
        {
            _velocityY += _gravity * Time.deltaTime * _gravityMultiplier;
        }
    }

    private void HandleJump()
    {
        if (_inputActions.Player.Jump.IsPressed() && _characterController.isGrounded)
        {
            _velocityY = _jumpForce;
        }
    }

    private void HandleLook()
    {
        //get the look vector and set it up
        Vector2 lookVector = _inputActions.Player.Look.ReadValue<Vector2>();
        float mouseX = lookVector.x * _lookSensitivity * Time.deltaTime; 
        float mouseY = lookVector.y * _lookSensitivity * Time.deltaTime; 
        _yRot += mouseX;
        _xRot -= mouseY;
        _xRot = Mathf.Clamp(_xRot, -60f, 60f);

        //set the values to the objects
        transform.rotation = Quaternion.Euler(0, _yRot, 0);
        _centerSpinePos.rotation = Quaternion.Euler(_xRot, _yRot, 0);
    }

    private void HandleInputAndMove()
    {
        Vector2 inputVector = _inputActions.Player.Move.ReadValue<Vector2>();
        _moveDirection = inputVector.y * transform.forward + inputVector.x * transform.right;
        _characterController.Move(_moveDirection * _currentSpeed * Time.deltaTime);
        _characterController.Move(transform.up * _velocityY * Time.deltaTime);
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }
}
