using UnityEngine;
using System.Collections;

public class EveryplayManager : MonoBehaviour
{

    void Start()
    {
        Everyplay.FaceCamRecordingPermission += CheckForRecordingPermission;
        Everyplay.FaceCamRequestRecordingPermission();
    }

    private void CheckForRecordingPermission(bool granted)
    {
        if (granted)
        {
            Debug.Log("Microphone access was granted");
            // * HERE YOU CAN START YOUR FACECAM SAFELY * //
        }
        else
        {
            Debug.Log("Microphone access was DENIED");
        }
    }


}
