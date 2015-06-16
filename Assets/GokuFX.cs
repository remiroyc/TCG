using UnityEngine;

public class GokuFX : MonoBehaviour
{
    [HideInInspector]
    public bool IsTransformationFinished = false;
    public GameObject MyPlayer;
    public GameObject Aura;

    public Texture redAura, whiteAura, yellowAura;

    public ParticleSystem PowerUpDust, ShockWave, RockTransformation, LightningField;

    public AudioClip TransSound, AuraActivationSound, ScreamSuperSaiyanSound;
    public AudioClip ssjAuraLoopSound;

    /// <summary>
    /// Lancement du premier effet (foudre) pour la transformation
    /// </summary>
    public void StartThunder()
    {
        IsTransformationFinished = false;
        LightningField.Play();
        StartTransform();
    }

    public void StartTransform()
    {
        if (TransSound != null)
        {
            AudioSource.PlayClipAtPoint(TransSound, transform.position);
        }
    }

    public void FirstWave()
    {
        ShockWave.Play();
        if (PowerUpDust != null)
        {
            PowerUpDust.gameObject.SetActive(true);
        }
    }

    public void StartAura()
    {
        if (Aura != null)
        {
            Aura.GetComponent<SkinnedMeshRenderer>().material.mainTexture = yellowAura;
            Aura.SetActive(true);
        }

        MyCharacterController.Instance.TransformationManager.ManageMiTransformation();

        AudioSource.PlayClipAtPoint(AuraActivationSound, transform.position);
        this.GetComponent<AudioSource>().Play();
    }

    public void FinishTransform()
    {
        LightningField.Stop();

        if (Aura != null)
        {
            Aura.SetActive(false);
        }
        PowerUpDust.gameObject.SetActive(false);

        RockTransformation.gravityModifier = 1;
        RockTransformation.loop = false;
        if (PowerUpDust != null)
        {
            PowerUpDust.Stop();
        }
        this.GetComponent<AudioSource>().Stop();
        
        IsTransformationFinished = true;
    }

    public void StartFinishingTransform()
    {
        AudioSource.PlayClipAtPoint(ScreamSuperSaiyanSound, transform.position);
        MyCharacterController.Instance.TransformationManager.PlayTransformationEndSound();
    }

    public void ShootGenki()
    {
        GameObject.Find("genkidama_shoot_sound").GetComponent<AudioSource>().Play();
        GameObject.Find("genkidama_drop_sound").GetComponent<AudioSource>().Play();
    }
}