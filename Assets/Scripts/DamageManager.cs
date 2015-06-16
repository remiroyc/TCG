using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public MyCharacterController Controller;
    public Transform LeftFoot, RightHand, LeftHand, RightFoot, LeftKnee;
    public bool DesactivateParticle;

    private ParticleSystem _instantiatedParticle;
    private GameObject _flashAttackObj;

    private void Awake()
    {
        _flashAttackObj = Resources.Load<GameObject>("CFXPrefabs_Mobile/Misc/FlashAttack");
    }

    private void Update()
    {
        if (DesactivateParticle && _instantiatedParticle != null)
        {
            _instantiatedParticle.Stop();
            _instantiatedParticle.Clear();
            DesactivateParticle = false;
        }
    }

    public void GiveDamage(string skill)
    {
        if (Controller != null)
        {
            Controller.GiveDamage(skill);
        }
        else
        {
            GameObject.Find("MyPlayer").GetComponent<MyCharacterController>().GiveDamage(skill);
        }
    }

    public void Recover()
    {
        var ring = Resources.Load("Prefabs/Ring") as GameObject;
        Instantiate(ring, transform.position + (Vector3.down * 0.3f), Quaternion.identity);
        Controller.CanMove = true;
        Controller.CanAttack = true;
    }

    public void CheckAttackCharge(string bodyPart)
    {
        Controller.CheckAttackCharge();
        if (Controller.ChargeActivated && !DesactivateParticle)
        {

            GameObject go = null;
            switch (bodyPart)
            {
                case "right_hand":
                    go = (GameObject)Instantiate(_flashAttackObj, RightHand.position, Quaternion.identity);
                    go.transform.parent = RightHand.transform;

                    break;

                case "left_knee":
                    go = (GameObject)Instantiate(_flashAttackObj, LeftKnee.position, Quaternion.identity);
                    go.transform.parent = LeftKnee.transform;
                    break;

                case "right_foot":
                    go = (GameObject)Instantiate(_flashAttackObj, RightFoot.position, Quaternion.identity);
                    go.transform.parent = RightFoot.transform;
                    break;

                case "left_hand":
                    go = (GameObject)Instantiate(_flashAttackObj, LeftHand.position, Quaternion.identity);
                    go.transform.parent = LeftHand.transform;
                    break;

                case "left_foot":
                    go = (GameObject)Instantiate(_flashAttackObj, LeftFoot.position, Quaternion.identity);
                    go.transform.parent = LeftFoot.transform;
                    break;
            }

            if (go != null)
            {
                _instantiatedParticle = go.GetComponent<ParticleSystem>();
                _instantiatedParticle.Play();
            }

            DesactivateParticle = false;
        }

    }

}