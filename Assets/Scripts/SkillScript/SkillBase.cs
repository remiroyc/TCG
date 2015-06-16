using UnityEngine;

public class SkillBase : MonoBehaviour
{
    public float KiRequired;
    public SkillType CurrentSkillType;

    /// <summary>
    /// Permet de savoir si on peut lancer la technique (vérification du ki, des attaques, etc.)
    /// </summary>
    public bool CanActivateSkill
    {
        get
        {
            return MyCharacterController.Instance.CanAttack && !MyCharacterController.Instance.LoadingSkill
                && !MyCharacterController.Instance.Attacking && MyCharacterController.Instance.Ki >= KiRequired;
        }
    }

    protected void SubstractKi()
    {
        MyCharacterController.Instance.Ki -= KiRequired;
    }

    protected void DesactivateOtherSkill()
    {
        if (MyCharacterController.Instance.DashManager.IsDashing)
        {
            MyCharacterController.Instance.DashManager.DesactivateDash();
        }
    }

}
