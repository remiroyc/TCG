using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KiaiScript : SkillBase
{
    public ParticleSystem fxKiai;
    private Enemy lastFocusedEnemy;
    public Button btnQTE;
    public GameObject tpSprite; 

    public void DoKiai()
    {
        if (MyCharacterController.Instance.FocusEnemy != null)
        {
            lastFocusedEnemy = MyCharacterController.Instance.FocusEnemy.GetComponent<Enemy>();

            MyCharacterController.Instance.CanMove = false;
            MyCharacterController.Instance.CanAttack = false;
            MyCharacterController.Instance.FocusEnemy.GetComponent<Enemy>().CanMove = false;
            MyCharacterController.Instance.FocusEnemy.GetComponent<Enemy>().CanAttack = false;

            //Effet
            fxKiai.Play();

            //Son
            GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/kiai"));

            //Animation
            MyCharacterController.Instance.CharAnimator.SetTrigger("do_kiai");

            //Propulsion Ennemi
            MyCharacterController.Instance.FocusEnemy.GetComponent<Rigidbody>().AddExplosionForce(600f, this.transform.position, 50f);
            lastFocusedEnemy.TakeDamage("kiai");
            MyCharacterController.Instance.CanMove = true;
            MyCharacterController.Instance.CanAttack = true;
            StartCoroutine(EnemyRecover());
        }
    }

    IEnumerator EnemyRecover()
    {
        // Mettre la visibilité d'un bouton de QTE 
        btnQTE.gameObject.SetActive(true);
        iTween.ShakeRotation(btnQTE.gameObject, Vector3.one * 20f, 2f);

        yield return new WaitForSeconds(2f);
        btnQTE.gameObject.SetActive(false);
        lastFocusedEnemy.CanMove = true;
        lastFocusedEnemy.CanAttack = true;
        lastFocusedEnemy.RandomAction();
    }

    public void QTEBtnPressed()
    {
        StartCoroutine(TeleportBehindEnemy());
    }
    public IEnumerator TeleportBehindEnemy()
    {
        // Son
        var rand = Random.value;
        if (rand <= .33f)
            GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/teleport"));
        else if (rand > .33f && rand < .77f)
            GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/teleport00"));
        else
            GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/teleport2"));

        //Effet
        tpSprite.SetActive(true);
        foreach (var mesh in GameObject.FindGameObjectsWithTag("PlayerMeshes"))
            mesh.GetComponent<SkinnedMeshRenderer>().enabled = false;

        yield return new WaitForSeconds(.3f);

        // Changement Position
        transform.position = lastFocusedEnemy.transform.position - lastFocusedEnemy.transform.forward;
        foreach (var mesh in GameObject.FindGameObjectsWithTag("PlayerMeshes"))
            mesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
        tpSprite.SetActive(false);

        //btnQTE.gameObject.SetActive(false);
    }
}
