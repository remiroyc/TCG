using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using WellFired;

/// <summary>
/// Script qui permet de gérer la capacité Transformation Super Saiyan de Sangoku
/// </summary>
public class TransformationScript : SkillBase
{

    public ParticleSystem[] Effects;
    public GokuFX PlayerEffectScript;
    public SaiyanTransformationState CurrentState = SaiyanTransformationState.Normal;
    public SkinnedMeshRenderer HeadMeshRenderer;
    public Mesh NormalHeadMesh, SsjHeadMesh;
    public Material NormalMaterial, SS1Material;
    public USSequencer CutsceneCamera;

    /// <summary>
    /// Quantité de Ki utilisé par frame par la capcité Super Saiyan. 
    /// La variable "KiRequired" étant la quantité de Ki utilisé pour l'activation de la transformation.
    /// </summary>
    public float KiRequiredPerFrame;

    public bool Transforming = false;

    public AudioClip ssjFinishTransformSound;


    [HideInInspector]
    public bool IsSuperSaiyan
    {
        get { return CurrentState != SaiyanTransformationState.Normal; }
    }

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        CurrentSkillType = SkillType.Transformation;
    }

    // ReSharper disable once UnusedMember.Local
    private void Update()
    {
        if (IsSuperSaiyan)
        {
            if (MyCharacterController.Instance.Ki <= 0)
            {
                ActivateNormalMode();
            }
        }
        else if (Transforming && PlayerEffectScript.IsTransformationFinished)
        {
            FinishSS1Transformation();
            Transforming = false;
        }

    }

    // ReSharper disable once UnusedMember.Local
    private void LateUpdate()
    {
        if (IsSuperSaiyan)
        {
            MyCharacterController.Instance.Ki -= KiRequiredPerFrame * Time.deltaTime;
        }
    }

    //    CharAnimator.SetBool("transforming", false);
    //CharController.CanMove = true;
    //CharController.CanAttack = true;
    //if (CharController.FocusEnemy != null)
    //{
    //    CharController.FocusEnemy.GetComponent<Enemy>().CanAttack = true;
    //}

    // ReSharper disable once InconsistentNaming
    private void FinishSS1Transformation()
    {
        CurrentState = SaiyanTransformationState.SuperSaiyan1;
        HeadMeshRenderer.sharedMesh = SsjHeadMesh;
        HeadMeshRenderer.material = SS1Material;
        MyCharacterController.Instance.CanMove = true;
        MyCharacterController.Instance.CanAttack = true;
        MyCharacterController.Instance.CurrentSkill = null;
        ManageEnemiesActivation(true, true);
        this.GetComponent<AudioSource>().volume = 1f;
    }

    public void ManageSaiyanTransformation()
    {
        if (!IsSuperSaiyan)
        {
            if (CanActivateSkill && MyCharacterController.Instance.Grounded)
            {
                MyCharacterController.Instance.CurrentSkill = CurrentSkillType;
                SubstractKi();
                if (CutsceneCamera != null)
                {
                    CutsceneCamera.Play();
                }
                PlayerEffectScript.IsTransformationFinished = false;
                Transforming = true;
                ManageEnemiesActivation(false, false);
                DesactivateOtherSkill();
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                MyCharacterController.Instance.CharAnimator.SetTrigger("transforming");
                Effects[1].Play();
                MyCharacterController.Instance.CanMove = false;
                MyCharacterController.Instance.CanAttack = false;
            }
        }
        else
        {
            ActivateNormalMode();
        }
    }

    // Lorsque le saiyen n'est pas encore totalement transforme (cheveux dressés mais pas de jaune) 
    public void ManageMiTransformation()
    {
        HeadMeshRenderer.material = NormalMaterial;
        HeadMeshRenderer.sharedMesh = SsjHeadMesh;
    }

    private void ActivateNormalMode()
    {
        Transforming = false;
        CurrentState = SaiyanTransformationState.Normal;
        Effects[0].Play();
        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/descend"));
        HeadMeshRenderer.material = NormalMaterial;
        HeadMeshRenderer.sharedMesh = NormalHeadMesh;
    }

    private void ManageEnemiesActivation(bool canMove, bool canAttack)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var enemiesFiltered = enemies.Select(enemy => enemy.GetComponent<Enemy>()).Where(enemyScript => enemyScript != null);
        foreach (var enemyScript in enemiesFiltered)
        {
            enemyScript.CanMove = canMove;
            enemyScript.CanAttack = canAttack;
        }
        //Debug.Log(string.Format("ManageEnemiesActivation({0}, {1})", canMove, canAttack));
    }

    public void PlayTransformationEndSound()
    {
        this.GetComponent<AudioSource>().volume = 0.6f;
        this.GetComponent<AudioSource>().PlayOneShot(ssjFinishTransformSound);
    }
}

