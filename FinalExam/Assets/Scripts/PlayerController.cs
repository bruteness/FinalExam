using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController _instance;

    [SerializeField] private float _lightDamage;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _mouseSensitivity;

    private CharacterController _controller;
    private Camera _playerCamera;
    private float _cameraVerticalAngle;

    public float LightDamage { get { return _lightDamage; } }

    void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
            _instance = this;
        DontDestroyOnLoad(gameObject);

        _controller = GetComponent<CharacterController>();
        _playerCamera = transform.Find("Main Camera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //Handle the movement for the player
        Movement();
        HandleCharacterLook();
    }

    private void Movement()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); //Predefined axes in Unity linked to WASD controllers
        move = transform.TransformDirection(move);

        _controller.Move(move * Time.deltaTime * _moveSpeed); //Moves character in the given direction from our move vector3

    }

    private void HandleCharacterLook()
    {
        float lookX = Input.GetAxisRaw("Mouse X");
        float lookY = Input.GetAxisRaw("Mouse Y");

        // Rotate the transform with the input speed around its local Y axis
        transform.Rotate(new Vector3(0f, lookX * _mouseSensitivity, 0f), Space.Self);

        // Add vertical inputs to the camera's vertical angle
        _cameraVerticalAngle -= lookY * _mouseSensitivity;

        // Limit the camera's vertical angle to min/max
        _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);

        // Apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
        _playerCamera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
    }
}