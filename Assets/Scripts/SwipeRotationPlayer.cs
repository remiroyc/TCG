using UnityEngine;
using System.Collections;
using System.Linq;

public class SwipeRotationPlayer : MonoBehaviour
{
    public float rotateSpeed = 0.5f;
    public float rotateSpeedMouse = 200.0f;
    public int invertPitch = 1;
    public Camera mainCam;
    public MyCharacterController CharController;

    private float _pitch = 0.0f, yaw = 0.0f;
    private float _mousePitch = 0.0f, _mouseYaw = 0.0f;
    private Vector2 _touchBeganPosition;
    private Rect _joystickRect;
    private bool _reinitDecalage = false;


    public void Start()
    {
        var rectSize = Screen.height * 0.6f;
        _joystickRect = new Rect(0, 0, rectSize, rectSize);
    }

    public void OnTouchMovedAnywhere()
    {

        // pitch -= Input.GetTouch(0).deltaPosition.y * rotateSpeed * invertPitch * Time.deltaTime;
        // yaw += Input.GetTouch(0).deltaPosition.x * rotateSpeed * invertPitch * Time.deltaTime;

        //limit so we dont do backflips
        // pitch = Mathf.Clamp(pitch, -80, 80);

        //do the rotations of our character
        /*
this.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
mainCam.transform.eulerAngles = new Vector3(pitch, 0, 0.0f);
*/

        //	yaw += Input.GetTouch (0).deltaPosition.x * rotateSpeed * invertPitch * Time.deltaTime;
        //	this.transform.eulerAngles = new Vector3 (pitch, yaw, 0.0f);

    }

    /*
    public void OnGUI ()
    {
	
            GUI.Label (new Rect (Screen.width / 2, Screen.height / 2, 550, 150), _selectedTouch.position.ToString () + " / " + _joystickRect.ToString ());
            GUI.Label (new Rect (Screen.width / 2, (Screen.height / 2) + 150, 550, 150), _joystickRect.Contains (_selectedTouch.position).ToString ());

    }
*/
    private void Update()
    {

        if (CharController.FocusEnemy != null)
        {
            return;
        }

        var cam = Camera.main.GetComponent<CameraController>();

        // if (_reinitDecalage) {
        if (Mathf.Abs(cam.decalage) >= 0.5f)
        {

            cam.decalage -= Mathf.Sign(cam.decalage) * Time.deltaTime * 10;

        }
        else
        {
            cam.decalage = 0;
            _reinitDecalage = false;
        }
        // }

        if (Input.touches.Length > 0)
        {

            // Permet de récupérer le premier touch qui n'est pas dans la zone du joystick.
            //Touch? touchSelected = Input.touches.FirstOrDefault(t => !_joystickRect.Contains(t.position));


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
                case TouchPhase.Began:
                    _reinitDecalage = false;
                    _touchBeganPosition = touch.deltaPosition;
                    break;
                case TouchPhase.Moved:

                    var deltaAngleX = touch.deltaPosition.x * rotateSpeed * Time.deltaTime;
                    var deltaAngleY = -touch.deltaPosition.y * rotateSpeed * Time.deltaTime;

                    if (deltaAngleX > 0 && cam.decalage <= 15)
                    {
                        cam.decalage += deltaAngleX;
                    }
                    else if (deltaAngleX < 0 && cam.decalage >= -15)
                    {
                        cam.decalage += deltaAngleX;
                    }

                    var x = transform.rotation.eulerAngles.x + deltaAngleY; // transform.rotation.eulerAngles.x + deltaAngleY;
                    var y = transform.rotation.eulerAngles.y + deltaAngleX;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(x, y, 0)), Time.deltaTime * 50f);

                    break;

                case TouchPhase.Ended:
                    _reinitDecalage = true;
                    break;

            }

        }

#if UNITY_EDITOR
        _mousePitch -= Input.GetAxis("Mouse Y") * rotateSpeed * invertPitch * Time.deltaTime;
        _mouseYaw += Input.GetAxis("Mouse X") * rotateSpeed * invertPitch * Time.deltaTime;
        transform.eulerAngles = new Vector3(_mousePitch, _mouseYaw, 0.0f);
#endif

    }
}