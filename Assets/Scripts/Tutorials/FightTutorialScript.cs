using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script qui gère le tutoriel qui concerne l'utilisation des techniques de combat
/// </summary>
public class FightTutorialScript : BaseTutorial
{
    public MyCharacterController CharController;
    public Button AButton;
    public RoshiMaestroScript RoshiMaestro;

    public override void Retry()
    {
    }

    public override void StartTutorial()
    {
        AButton.gameObject.SetActive(true);
        CharController.FocusEnemy = RoshiMaestro.transform;
        RoshiMaestro.ActivateRoshiTalk();

        CharController.FocusEnemy = null;
        CharController.AutoFocus.enabled = true;
    }

    public void PunchReceived()
    {
        AButton.gameObject.SetActive(false);
        RoshiMaestro.ActivateRoshiTalk();

    }

}
