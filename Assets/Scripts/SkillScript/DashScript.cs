using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class DashScript : SkillBase
{
    [HideInInspector]
    public bool IsDashing = false;
    public AudioSource DashAudioSource;
    public ParticleSystem FxAura;
    public GameObject Aura, TrailNormal, TrailSuperSaiyan;
    private float _initialMagnitudeSpeed, _initialAccelerationSpeed, _initialJumpHeight, _initialAirSpeed;

    public GameObject DashObject;
    private GameObject _dashInstance;

    private void Awake()
    {
        CurrentSkillType = SkillType.Dash;
    }

    private void Start()
    {
        _initialMagnitudeSpeed = MyCharacterController.Instance.MaxMagnitudeSpeed;
        _initialAccelerationSpeed = MyCharacterController.Instance.AccelerationSpeed;
        _initialJumpHeight = MyCharacterController.Instance.JumpHeight;
        _initialAirSpeed = MyCharacterController.Instance.AirSpeed;
    }

    private void Update()
    {
        MyCharacterController.Instance.CharAnimator.SetBool("isDashing", IsDashing);
        if (IsDashing)
        {
            UpdateDashObj();
        }
    }

    private void UpdateDashObj()
    {
        if (Math.Abs(MyCharacterController.Instance.InputMovement.y) >= 0.3f)
        {
            if (_dashInstance == null)
            {
                _dashInstance = (GameObject)Instantiate(DashObject, transform.position, Quaternion.identity);
                _dashInstance.transform.parent = transform;
                _dashInstance.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else
            {
                var sign = (int)Mathf.Sign(MyCharacterController.Instance.InputMovement.y);
                _dashInstance.transform.localRotation = Quaternion.Euler(sign == -1 ? Vector3.zero : new Vector3(0, 180, 0));
            }
        }
        else if (_dashInstance != null)
        {
            Destroy(_dashInstance);
        }
    }

    private void LateUpdate()
    {
        if (IsDashing)
        {
            if (MyCharacterController.Instance.Ki < 1)
            {
                DesactivateDash();
            }
            else
            {
                MyCharacterController.Instance.Ki -= (KiRequired * Time.deltaTime);
            }
        }
    }

    private void FixedUpdate()
    {
        // Si on est en dash au corps à corps avec une vitesse suffisante alors on fait un expulsion
        if (IsDashing && MyCharacterController.Instance.CanAttack && !MyCharacterController.Instance.Attacking && MyCharacterController.Instance.CloseCombat)
        {
            // var magn = (CharController.MaxMagnitudeSpeed * 0.8f);
            if (MyCharacterController.Instance.InputMovement.y > 0) // rigidbody.velocity.sqrMagnitude > magn
            {
                StartCoroutine(DashAttack());
            }
        }
    }

    public void ActivateOrDesactivateDash()
    {
        if (!IsDashing)
        {
            ActivateDash();
        }
        else
        {
            DesactivateDash();
        }
    }

    public void ActivateDash()
    {
        if (!MyCharacterController.Instance.Attacking && !MyCharacterController.Instance.LoadingSkill && MyCharacterController.Instance.Ki > 0)
        {
            IsDashing = true;
            MyCharacterController.Instance.MaxMagnitudeSpeed = _initialMagnitudeSpeed * 2.5f;
            MyCharacterController.Instance.AccelerationSpeed = _initialAccelerationSpeed * 2.5f;
            MyCharacterController.Instance.JumpHeight = _initialJumpHeight * 1.2f;
            MyCharacterController.Instance.AirSpeed = _initialAirSpeed * 5;

            //if (Aura != null)
            //{
            //    Aura.SetActive(true);
            //}

            if (!DashAudioSource.isPlaying)
            {
                DashAudioSource.Play();
            }

            // FxAura.Play();

            if (!MyCharacterController.Instance.TransformationManager.IsSuperSaiyan)
            {
                TrailNormal.SetActive(true);
                TrailSuperSaiyan.SetActive(false);
            }
            else
            {
                TrailNormal.SetActive(false);
                TrailSuperSaiyan.SetActive(true);
            }
        }
        else
        {
            IsDashing = false;
        }
    }

    public void DesactivateDash()
    {

        MyCharacterController.Instance.MaxMagnitudeSpeed = _initialMagnitudeSpeed;
        MyCharacterController.Instance.AccelerationSpeed = _initialAccelerationSpeed;
        MyCharacterController.Instance.JumpHeight = _initialJumpHeight;
        MyCharacterController.Instance.AirSpeed = _initialAirSpeed;

        if (DashAudioSource.isPlaying)
        {
            DashAudioSource.Stop();
        }

        if (_dashInstance != null)
        {
            Destroy(_dashInstance);
        }

        if (Aura != null)
        {
            Aura.SetActive(false);
        }

        if (FxAura != null && FxAura.isPlaying)
        {
            FxAura.Stop();
        }

        TrailNormal.SetActive(false);
        TrailSuperSaiyan.SetActive(false);

        IsDashing = false;
    }

    private IEnumerator DashAttack()
    {
        MyCharacterController.Instance.Attacking = true;

        var dashAttackSound = Resources.Load("Sounds/pain2") as AudioClip;
        AudioSource.PlayClipAtPoint(dashAttackSound, transform.position);
        GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);

        MyCharacterController.Instance.CharAnimator.SetTrigger("dash_attack");

        if (MyCharacterController.Instance.RightAttack)
        {
            yield return new WaitForSeconds(0.3f);
            //Time.timeScale = 0.1f;
        }

        var attackSound = Resources.Load("Sounds/strongpunch") as AudioClip;
        AudioSource.PlayClipAtPoint(attackSound, MyCharacterController.Instance.FocusEnemy.position);

        var slashEffect = Resources.Load("BlueSlash") as GameObject;
        Instantiate(slashEffect, MyCharacterController.Instance.FocusEnemy.position, Quaternion.identity);

        var enemyScript = MyCharacterController.Instance.FocusEnemy.GetComponent<Enemy>();
        enemyScript.Refocus();

        MyCharacterController.Instance.FocusEnemy.GetComponent<Rigidbody>().AddForce((Vector3.up + transform.forward) * 400);
        enemyScript.ApplyDamage(new TakeDamageModel()
        {
            SkillName = "dash_attack",
            Default = true,
            CanDodge = false
        });

        if (MyCharacterController.Instance.RightAttack)
        {
            yield return new WaitForSeconds(0.05f);
            Time.timeScale = 1f;
        }

        MyCharacterController.Instance.RightAttack = !MyCharacterController.Instance.RightAttack;

        yield return new WaitForSeconds(0.7f);

        // DesactivateDash();
        MyCharacterController.Instance.Attacking = false;
    }

}

