using UnityEngine;
using UnityEngine.InputSystem;

public class Player: MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float mass = 1f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private Transform cameraTransform;
    private CharacterController _controller;
    private Vector3 _velosity;
    private Vector2 _look;
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;

    void Awake ()
    {
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["move"];
        _lookAction = _playerInput.actions["look"];
        _jumpAction = _playerInput.actions["jump"];
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateGravity();
        UpdateMovement();
        UpdateLook();
    }

    private void UpdateGravity()
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        _velosity.y = _controller.isGrounded ? -1f : _velosity.y + gravity.y;
    }

    private Vector3 GetMovementInput()
    {
        var moveImput = _moveAction.ReadValue<Vector2>();
        var input = new Vector3();
        input += transform.forward * moveImput.y;
        input += transform.right * moveImput.x;
        input = Vector3.ClampMagnitude(input, 1f);
        input *= movementSpeed;
        return input;
    }

    private void UpdateMovement()
    {
        var input = GetMovementInput();
        var factor = acceleration * Time.deltaTime;
        _velosity.x = Mathf.Lerp(_velosity.x, input.x, factor);
        _velosity.z = Mathf.Lerp(_velosity.z, input.z, factor);
        var jampInput = _jumpAction.ReadValue<float>();
        if (jampInput > 0 && _controller.isGrounded)
            _velosity.y += jumpSpeed;
        _controller.Move(_velosity * Time.deltaTime);
    } 

    void UpdateLook()
    {
        var lookInput = _lookAction.ReadValue<Vector2>();
        _look.x += lookInput.x * mouseSensitivity;
        _look.y += lookInput.y * mouseSensitivity;
        _look.y = Mathf.Clamp(_look.y, -89f, 89f);
        cameraTransform.localRotation = Quaternion.Euler(-_look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, _look.x, 0);
    }
}
