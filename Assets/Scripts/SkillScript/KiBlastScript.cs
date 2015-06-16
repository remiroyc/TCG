using System.Collections;
using UnityEngine;

public class KiBlastScript : SkillBase
{

    public AudioClip KiBlastSound;
    public GameObject KiBlastPrefab;
    private const string KiBlastTriggerName = "kiblast_drop";

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        CurrentSkillType = SkillType.KiBlast;
        if (KiBlastPrefab == null)
        {
            KiBlastPrefab = Resources.Load<GameObject>("Prefab/Fireball");
        }
    }

    public void LaunchKiBlast()
    {
        if (!MyCharacterController.Instance.CanAttack && !MyCharacterController.Instance.GettingHit)
            return;

        DesactivateOtherSkill();
        StartCoroutine(DropKiBlast());
    }

    private IEnumerator DropKiBlast()
    {
        MyCharacterController.Instance.CurrentSkill = CurrentSkillType;
        SubstractKi();

        MyCharacterController.Instance.CharAnimator.SetTrigger(KiBlastTriggerName);
        MyCharacterController.Instance.GetComponent<AudioSource>().PlayOneShot(KiBlastSound);

        // var angle = transform.rotation;
        var posF = transform.position + (transform.forward * 0.2f);
        var instPos = !MyCharacterController.Instance.RightAttack ? posF - (transform.right * 0.2f) : posF + (transform.right * 0.2f);
        instPos.y += 0.1f;

        var angle = MyCharacterController.Instance.FocusEnemy != null
            ? Quaternion.LookRotation(transform.position - MyCharacterController.Instance.FocusEnemy.position)
            : Quaternion.LookRotation(transform.position - CharacterSight.Instance.GetImpactPoint());

        Instantiate(KiBlastPrefab, instPos, angle);

        MyCharacterController.Instance.RightAttack = !MyCharacterController.Instance.RightAttack;
        yield return new WaitForSeconds(0.2f);
        MyCharacterController.Instance.CurrentSkill = null;
        MyCharacterController.Instance.Attacking = false;
    }

}
