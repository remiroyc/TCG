using System.Collections;
using UnityEngine;

/// <summary>
/// Script qui intervient lorsque le personnage se fait éjecter et est en mode "ko".
/// </summary>
public class KoRecoverScript : MonoBehaviour
{

    public Player Character;
    public float GroundKoTime = 4; // Nb de secondes que le personnage passera au sol avant de pouvoir se relever.
    private bool _koGrounded = false; // Booléen qui permet de savoir (dans le cas ou notre personnage est "ko") si il a touché le sol.
    private float _koTime;
    private bool _ko;
    private bool _inCoroutine = false;
    private const string RecoverAnimName = "ko_recover";

    private const float MinGroundDetectionTime = 0.5f;
    private GameObject groundWave;
    private int _groundWavesCounter;

    public bool Ko
    {
        get { return _ko; }
        set
        {
            _ko = value;
            _koTime = value ? Time.time : 0;

            Character.CanMove = !value;
            Character.CanAttack = !value;

            if (!value)
            {
                _koGrounded = false;
            }
        }
    }

    private void Update()
    {
        Character.CharAnimator.SetBool("ko", Ko);
        if (Ko && Character.Grounded && Time.time - _koTime >= MinGroundDetectionTime)
        {
            if (!_koGrounded)
            {
                // On tape le sol pour la première fois. 
                // C'est à partir de ce moment on enclenche le décompte qui va permettre au personnage de se relever.
                _koGrounded = true;
                _koTime = Time.time;
                //this.transform.rigidbody.velocity = Vector3.zero
                LaunchGroundWave();
                MyCharacterController.Instance.PlayRandomPainAudioClip();
            }
            else
            {
                if ((Time.time - _koTime) >= GroundKoTime && !_inCoroutine)
                {
                    _inCoroutine = true;
                    StartCoroutine(LaunchKoRecoverAnim());
                }
            }
        }

        if (_groundWavesCounter > 4)
        {
            CancelInvoke("InstantiateSeveralGroundWaves");
        }
    }

    public void LaunchGroundWave()
    {
        groundWave = Resources.Load<GameObject>("Prefabs/DustStorm");
        if (groundWave != null)
        {
            //InvokeRepeating("InstantiateSeveralGroundWaves", 0f, 0.3f);
            InstantiateSeveralGroundWaves();
            //var ps = groundWaveObj.GetComponent<ParticleSystem>();
            //if (!ps.isPlaying)
            //{
            //    ps.Play();
            //}


        }
    }

    private void InstantiateSeveralGroundWaves()
    {
        var groundWaveObj = (GameObject)Instantiate(groundWave, transform.position + Vector3.down * 0.6f, Quaternion.identity);

        var particleAudio = groundWaveObj.GetComponent<AudioSource>();
        var clip = Resources.Load<AudioClip>("Sounds/Explosions/explosion2");
        particleAudio.PlayOneShot(clip, 1.5f);

    }

    private IEnumerator LaunchKoRecoverAnim()
    {
        Character.CharAnimator.SetTrigger(RecoverAnimName);
        yield return new WaitForSeconds(1.2f);

        if (Character.IsMyPlayer)
        {
            ((MyCharacterController)Character).AutoFocus.enabled = true;
        }

        Ko = false;
        _inCoroutine = false;
    }

}
