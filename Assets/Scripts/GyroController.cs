using TMPro;
using UnityEngine;

public class GyroController : MonoBehaviour
{
    [SerializeField] private Transform _controlled;
    [SerializeField] private Rigidbody _controlledRB;
    [SerializeField] private Vector3 _forceVector;
    [SerializeField] private TextMeshProUGUI _debugText;
    [SerializeField] private bool _reverseRotationAxis = true;

    private bool _isGyroControlled = false;
    private Quaternion _originalControlledRotation;
    private bool _gameEnabled;

    public static GyroController Instance { get; private set; }
    
    // Start is called before the first frame update
    private void Awake()
    {
        _originalControlledRotation = _controlled.localRotation;
        EnableGyro();
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void EnableGyro()
    {
        // enables and also reset gyro orientation
        Input.gyro.enabled = false;
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_gameEnabled)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            if (!_isGyroControlled && SystemInfo.supportsGyroscope)
            {
                _isGyroControlled = true;
            }
        }

        if (_isGyroControlled)
        {
            HandleGyroMovement();
        }
        else
        {
            HandleKeyboardMovement();
        }
    }

    private void HandleGyroMovement()
    {
        var fixedQuaternion = new Quaternion(Input.gyro.attitude.x, Input.gyro.attitude.y, -Input.gyro.attitude.z,
            -Input.gyro.attitude.w);
        
        //values run from 0 to 360; the ranges we'd like to enable are 0 to 30 or 330 to 360.
        var pitch = TranslateGyroValue(fixedQuaternion.eulerAngles.x);
        var yaw = TranslateGyroValue(fixedQuaternion.eulerAngles.y);

        // _controlled.rotation = Quaternion.Euler(pitch, 0, yaw);
        _controlledRB.MoveRotation(Quaternion.Euler(pitch, 0, yaw));

        PrintGyroDebugText();
    }
    
    private void HandleKeyboardMovement()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var addedForces = new Vector3(
            -1 * horizontal * _forceVector.x * Time.fixedDeltaTime, 0,
            -1 * vertical * _forceVector.z * Time.fixedDeltaTime);
        var newRotation = _controlledRB.rotation.eulerAngles + addedForces;

        // some ugly coding is required here to avoid 'teleporting' I'm afraid
        var pitch = (-2f < newRotation.x && newRotation.x < 22f) || (338f < newRotation.x && newRotation.x < 362f)
            ? newRotation.x
            : _controlledRB.rotation.eulerAngles.x;
        var yaw = (-2f < newRotation.z && newRotation.z < 22f) || (338f < newRotation.z && newRotation.z < 362f)
            ? newRotation.z
            : _controlledRB.rotation.eulerAngles.z;

        _controlledRB.MoveRotation(Quaternion.Euler(pitch, 0, yaw));

        PrintKeyboardDebugText(addedForces, newRotation);
    }

    private float TranslateGyroValue(float gyroAngle)
    {
        //values run from 0 to 360; the ranges we'd like to enable are 0 to 20 or 340 to 360;
        //but to prevent physics jitter we translate from 340 - 360 to -20 to 0.
        
        var translatedAngle = (gyroAngle < 180f) ? 
            Mathf.Clamp(gyroAngle, 0, 20f) : 
            -1 * (360 - Mathf.Clamp(gyroAngle, 340f, 360f));

        return translatedAngle;
    }
    private void PrintGyroDebugText()
    {
        var attitudeEuler = Input.gyro.attitude.eulerAngles;

        _debugText.text = $"Input.gyro.attitude: X:{attitudeEuler.x:N2}, Y:{attitudeEuler.y:N2}, Z:{attitudeEuler.z:N2}\nInput.gyro.gravity: X:{Input.gyro.gravity.x:N2}, Y:{Input.gyro.gravity.y:N2}, Z:{Input.gyro.gravity.z:N2}";
    }
    
    private void PrintKeyboardDebugText(Vector3 addedForces, Vector3 calculatedRotation)
    {
        _debugText.text = $"Added Forces: X:{addedForces.x:N2}, Y:{addedForces.y:N2}, Z:{addedForces.z:N2}\nCalculated Rotation: X:{calculatedRotation.x:N2}, Y:{calculatedRotation.y:N2}, Z:{calculatedRotation.z:N2}";
    }

    public void ResetOrientation()
    {
        _controlled.rotation = _originalControlledRotation; 
        EnableGyro();
    }

    public void SetGameEnabled(bool gameEnabled)
    {
        _gameEnabled = gameEnabled;
    }
}
