using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController instance;

    [SerializeField] private GameObject[] _playerLights;
    [SerializeField] private float _lightDamage;
    [SerializeField] private float _flashlightBattery;
    [SerializeField] private float _decreaseBatterySpeed;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private bool _flashlightOn = true;

    public CharacterController controller;
    private Camera _playerCamera;
    private float _cameraVerticalAngle;
    private float _characterVelocityY;
    private float _gravityDownForce = -1f;
    private bool _isDead;

    public float LightDamage { get { return _lightDamage; } }
    public float FlashlightBattery { get { return _flashlightBattery; } }
    public float FlashlightMaxBattery { get; set; }
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }

    void Awake() {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);

        controller = GetComponent<CharacterController>();
        _playerCamera = transform.Find("Main Camera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;

        FlashlightMaxBattery = FlashlightBattery;
    }

    void Update() {
        //Handle the movement for the player
        if (_isDead) { GameManager.instance.SetGameOver(); return; }
        if (GameManager.instance.didGameWin) return;
        Movement();
        HandleCharacterLook();
        HandlePageLook();
        HandleFlashlight();
    }

    private void HandleFlashlight() {

        // Turn off the flashlight
        if (Input.GetKeyDown(KeyCode.F) && _flashlightOn || _flashlightBattery <= 0) {
            _flashlightOn = false;
            // Turn on the flashlight
        } else if (Input.GetKeyDown(KeyCode.F) && !_flashlightOn && _flashlightBattery > 0) {
            _flashlightOn = true;
        }
        
        // Choose whether to have the flashlight on or off
        foreach (GameObject g in _playerLights) {
            g.SetActive(_flashlightOn);
        }

        if (_flashlightOn && _flashlightBattery >= 0) {
            _flashlightBattery -= _decreaseBatterySpeed * Time.deltaTime;
        }
    }

    private void Movement() {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); //Predefined axes in Unity linked to WASD controllers
        move = transform.TransformDirection(move);

        // Add gravity
        _characterVelocityY += _gravityDownForce * Time.deltaTime;
        move.y = _characterVelocityY;

        // Move the character controller
        controller.Move(move * Time.deltaTime * _moveSpeed); //Moves character in the given direction from our move vector3
    }

    private void HandleCharacterLook() {
        float lookX = Input.GetAxisRaw("Mouse X");
        float lookY = Input.GetAxisRaw("Mouse Y");

        // Rotate the transform with the input speed around its local Y axis
        transform.Rotate(new Vector3(0f, lookX * _mouseSensitivity * Time.deltaTime * 100, 0f), Space.Self);

        // Add vertical inputs to the camera's vertical angle
        _cameraVerticalAngle -= lookY * _mouseSensitivity * Time.deltaTime * 100;

        // Limit the camera's vertical angle to min/max
        _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);

        // Apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
        _playerCamera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
    }

    private void HandlePageLook() {
        if (Input.GetKeyUp(KeyCode.E)) {
            Ray ray = _playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform.name.Contains("Note")) {
                GameManager.instance.currentNotesFound++;
                GameManager.instance.noteCanvas.SetActive(true);
                Destroy(GameObject.Find(hit.transform.name));
            }
        }
    }

    public void AddBattery(float _batteryToAdd) {
        _flashlightBattery += _batteryToAdd;
        _flashlightBattery = _flashlightBattery < FlashlightMaxBattery ? _flashlightBattery : FlashlightMaxBattery;
    }
}