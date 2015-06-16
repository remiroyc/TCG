using UnityEngine;

/// <summary>
/// Gestion des entrées / sorties des périphériques d'un PC
/// </summary>
public class ComputerInputController : MonoBehaviour
{

    public MyCharacterController CharController;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {
            CharController.GetComponent<ThrowScript>().StartSwing();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            CharController.GetComponent<TeleportationComboScript>().ManageProjection();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            CharController.GenkidamaManager.StartGenkidama();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            CharController.ManageKeysPressed('C');
        }

        if (Input.GetKey(KeyCode.U))
        {
            CharController.TeleportationManager.StartFirstCombo();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            CharController.KamehamehaManager.ChargeKame();
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            CharController.KamehamehaManager.LaunchKame();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            CharController.ChargeKi();
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            CharController.CancelCharge();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            CharController.KaiokenManager.Charge();
        }
        else if (Input.GetKeyUp(KeyCode.V))
        {
            CharController.KaiokenManager.CancelCharge();
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            CharController.ManageKeysPressed('A');
        }

        if (Input.GetKeyUp(KeyCode.Keypad3))
        {
            CharController.ReleaseAButton();
        }

        if (Input.GetKey(KeyCode.G))
        {
            CharController.TransformationManager.ManageSaiyanTransformation();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            CharController.ChargeGenericBeam();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            CharController.GetComponent<KiBlastScript>().LaunchKiBlast();
        }

        if (Input.GetKeyUp(KeyCode.N))
        {
            CharController.GetComponent<DashScript>().ActivateOrDesactivateDash();
        }
    }
}
