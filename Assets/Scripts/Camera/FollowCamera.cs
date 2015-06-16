using UnityEngine;

/// <summary>
/// Ce script permet au personnage de suivre la rotation de la caméra (quand la caméra est en orbitale).
/// </summary>
public class FollowCamera : MonoBehaviour
{
    public Transform Target;
    public AutoFocusScript AutoFocusScript;

    void Awake()
    {
        if (AutoFocusScript == null)
        {
            AutoFocusScript = FindObjectOfType<AutoFocusScript>();
        }
    }

    private void Update()
    {
        if (!AutoFocusScript.AutoFocus && !MyCharacterController.Instance.KoManager.Ko)
        {
            var lookRotation = Quaternion.LookRotation(Camera.main.transform.position - Target.position);
            lookRotation *= Quaternion.AngleAxis(180, transform.up);
            transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }
}
