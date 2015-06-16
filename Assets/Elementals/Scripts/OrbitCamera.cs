using System.Linq;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    #region Variables

    // Target to look at
    public Transform TargetLookAt;

    // Camera distance variables
    public float Distance = 10.0f;
    public float DistanceMin = 5.0f;
    public float DistanceMax = 15.0f;
    private float _startingDistance = 0.0f;
    private float _desiredDistance = 0.0f;

    public bool IsRotating = false;
    public float RotationDelta = 0;

    // Mouse variables
    private float _mouseX = 0.0f;
    private float _mouseY = 0.0f;
    public float X_MouseSensitivity = 5.0f;
    public float Y_MouseSensitivity = 5.0f;
    public float MouseWheelSensitivity = 5.0f;

    // Axis limit variables
    public float Y_MinLimit = 15.0f;
    public float Y_MaxLimit = 70.0f;
    public float DistanceSmooth = 0.025f;
    private float velocityDistance = 0.0f;
    private Vector3 desiredPosition = Vector3.zero;
    public float X_Smooth = 0.05f;
    public float Y_Smooth = 0.1f;

    // Velocity variables
    private float _velX = 0.0f;
    private float _velY = 0.0f;
    private float _velZ = 0.0f;
    private Vector3 _position = Vector3.zero;
    private Rect _joystickRect = new Rect();
    private Vector3 _originPosition;
    private Quaternion _originRotation;
    private float _initialYRotation;

    // public float shake_decay;
    // public float shake_intensity;


    #endregion

    // ######################################################################
    // MonoBehaviour Functions
    // ######################################################################

    #region Component Segments

    public void InitCamera()
    {
        //Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
        //Distance = Vector3.Distance(TargetLookAt.transform.position, gameObject.transform.position);
        //if (Distance > DistanceMax)
        //{
        //    DistanceMax = Distance;
        //}
        _initialYRotation = TargetLookAt.rotation.eulerAngles.y;
        Distance = DistanceMax;
        _startingDistance = DistanceMax;
        Reset();
    }

    void Start()
    {
        var rectSize = Screen.height * 0.6f;
        _joystickRect = new Rect(0, 0, rectSize, rectSize);
        InitCamera();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // LateUpdate is called after all Update functions have been called.
    void LateUpdate()
    {
        if (TargetLookAt == null)
            return;

        HandlePlayerInput();

        CalculateDesiredPosition();

        UpdatePosition();

    }

    #endregion

    // ######################################################################
    // Player Input Functions
    // ######################################################################

    #region Component Segments

    void HandlePlayerInput()
    {
        // mousewheel deadZone
        const float deadZone = 0.01f;



#if UNITY_EDITOR

        if (Input.GetButtonDown("Fire1"))
        {
            IsRotating = true;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            IsRotating = false;
        }

        if (IsRotating)
        {
            RotationDelta = Input.GetAxis("Mouse X") * X_MouseSensitivity;
            _mouseX += RotationDelta;
            _mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
            _mouseY = ClampAngle(_mouseY, Y_MinLimit, Y_MaxLimit);
        }
        else
        {
            RotationDelta = 0;
        }

        // get Mouse Wheel Input
        if (Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
        {
            _desiredDistance = Mathf.Clamp(Distance - (Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity),
                                                      DistanceMin, DistanceMax);
        }

#elif UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8

        if (Input.touchCount > 0)
        {
            //var touch = Input.touches.FirstOrDefault(t => !_joystickRect.Contains(t.position));

            Touch? touchSelected = null;
            foreach (var t in Input.touches)
            {
                bool isContainedInJoystick = _joystickRect.Contains(t.position);

                if (!isContainedInJoystick)
                {
                    touchSelected = t;
                    break;
                }
            }

            if (touchSelected == null)
            {
                return;
            }

            var touch = (Touch)touchSelected;


            switch (touch.phase)
            {
                case TouchPhase.Moved:
                    IsRotating = (touch.deltaPosition != Vector2.zero);

                    if (IsRotating)
                    {
                        RotationDelta = touch.deltaPosition.x * X_MouseSensitivity;
                        _mouseX += RotationDelta;
                        _mouseY -= touch.deltaPosition.y * Y_MouseSensitivity;
                        _mouseY = ClampAngle(_mouseY, Y_MinLimit, Y_MaxLimit);
                    }
                    else
                    {
                        RotationDelta = 0;
                    }

                    break;

                default:
                    IsRotating = false;
                    break;
            }

        }

#endif


    }

    #endregion

    // ######################################################################
    // Calculation Functions
    // ######################################################################

    #region Component Segments

    void CalculateDesiredPosition()
    {
        // Evaluate distance
        //Distance = Mathf.SmoothDamp(Distance, _desiredDistance, ref velocityDistance, DistanceSmooth);

        // Calculate desired position -> Note : mouse inputs reversed to align to WorldSpace Axis
        desiredPosition = CalculatePosition(_mouseY, _initialYRotation + _mouseX, Distance);
    }

    Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {
        var direction = new Vector3(0, 0, -distance);
        var rotation = Quaternion.Euler(rotationX, rotationY, 1);
        return TargetLookAt.position + (rotation * direction);
    }

    #endregion

    // ######################################################################
    // Utilities Functions
    // ######################################################################

    #region Component Segments

    // update camera position
    void UpdatePosition()
    {

        float posX = Mathf.SmoothDamp(_position.x, desiredPosition.x, ref _velX, X_Smooth);
        float posY = Mathf.SmoothDamp(_position.y, desiredPosition.y, ref _velY, Y_Smooth);
        float posZ = Mathf.SmoothDamp(_position.z, desiredPosition.z, ref _velZ, X_Smooth);
        _position = new Vector3(posX, posY, posZ);


        transform.position = desiredPosition;
        transform.LookAt(TargetLookAt);

        //if (shake_intensity > 0)
        //{
        //    transform.position = desiredPosition + Random.insideUnitSphere * shake_intensity;
        //    transform.rotation = new Quaternion(
        //        originRotation.x + Random.Range(-shake_intensity, shake_intensity) * .2f,
        //        originRotation.y + Random.Range(-shake_intensity, shake_intensity) * .2f,
        //        originRotation.z + Random.Range(-shake_intensity, shake_intensity) * .2f,
        //        originRotation.w + Random.Range(-shake_intensity, shake_intensity) * .2f
        //        );
        //    shake_intensity -= shake_decay;
        //}
    }

    // Reset Mouse variables
    public void Reset()
    {
        _mouseX = 0;
        _mouseY = 0;
        Distance = _startingDistance;
        _desiredDistance = Distance;
    }

    // Clamps angle between a minimum float and maximum float value
    float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360.0f || angle > 360.0f)
        {
            if (angle < -360.0f)
                angle += 360.0f;
            if (angle > 360.0f)
                angle -= 360.0f;
        }

        return Mathf.Clamp(angle, min, max);
    }

    //public void Shake()
    //{
    //    originPosition = transform.position;
    //    originRotation = transform.rotation;
    //    //shake_intensity = .3f;
    //    //shake_decay = 0.002f;
    //    shake_intensity = .1f;
    //    shake_decay = 0.002f;
    //}

    #endregion


}

