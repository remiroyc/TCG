using UnityEngine;

public class PunchEffectScript : MonoBehaviour
{
    public ParticleSystem LeftPunchEffect;
    public ParticleSystem RightPunchEffect;


    public void RightPunch()
    {
        RightPunchEffect.Play();
        //transform.parent.audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/mediumunch"));
    }

    public void LeftPunch()
    {
        LeftPunchEffect.Play();
        //transform.parent.audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/mediumkick"));
    }

    public void RightKick()
    {
        LeftPunchEffect.Play();
        //transform.parent.audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/strongkick"));
    }

    public void RightStrongPunch()
    {
        LeftPunchEffect.Play();
        //transform.parent.audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/strongpunch"));
    }

}