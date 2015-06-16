using UnityEngine;
using System.Collections;
using Assets.Scripts.SkillScript;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

namespace Assets.Scripts.SkillScript
{
    /// <summary>
    /// Script qui permet de gérer la capacité Kaioken de notre personnage
    /// </summary>
    public class KaiokenScript : SkillBase, ICombo
    {

        private float _timer = 0f;
        public ParticleSystem[] Effects;

        public AudioClip KaiokenChargeAudioSource;
        public AudioClip KaiokenDash;
        public AudioClip KaiokenDash2;
        public AudioClip KaiokenScream;
        public AudioClip KaiokenAura;



        public QTEManager QTE;
        private GameObject _ennemy;
        private bool _isMoving;
        public float dashSpeed = 7f;

        public Transform[] waypointsRight;
        public Transform[] waypointsTop;
        private bool _initTween;
        private int _hitCount;
        private int maxAttacksNb = 4;

        void Awake()
        {
            CurrentSkillType = SkillType.Kaioken;
        }

        void Start()
        {
            //QTE.GenerateTapSequence(4);
            _ennemy = GameObject.Find("EnnemyPrefab");
        }

        void FixedUpdate()
        {

            // On dash jusqu'à être assez proche de l'ennemi 
            if (_isMoving && (Vector3.Distance(this.transform.position, _ennemy.transform.position) > 2f))
            {
                if (!_initTween)
                {

                    //if (_hitCount == 0)
                    //    transform.DOPath(waypointsRight.Select(w => w.transform.position).ToArray(), 0.5f, PathType.CatmullRom, PathMode.Full3D, 5, Color.red)
                    //           .SetEase(Ease.Linear)
                    //           .SetLookAt(1);
                    //else if (_hitCount >= 1)
                    //    transform.DOPath(waypointsTop.Select(w => w.transform.position).ToArray(), 0.5f, PathType.CatmullRom, PathMode.Full3D, 5, Color.red)
                    //       .SetEase(Ease.Linear)
                    //       .SetLookAt(1);

                    //this.rigidbody.AddForce(new Vector3(20f, 0, 0), ForceMode.VelocityChange);



                    _initTween = true;
                }

                iTween.MoveUpdate(this.gameObject, _ennemy.transform.position, .5f);
                // Déplacement principal
                //this.transform.position = Vector3.Lerp(CharController.transform.position, _ennemy.transform.position, Time.deltaTime * dashSpeed);
            }
            // On a atteint l'ennemi
            else if (_isMoving && (Vector3.Distance(this.transform.position, _ennemy.transform.position) < 2.05f))
            {
                _isMoving = false;
                //CharController.CanMove = true;

                //On fout un fat coup 
                MyCharacterController.Instance.CharAnimator.Play("DashAttack");
                _ennemy.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 1f, 1f) * 10f, ForceMode.VelocityChange);
                _hitCount++;

            }
            else
            {
                _isMoving = false;
                _initTween = false;
                //CharController.CanMove = true;
            }

        }


        public void Charge()
        {
            //if (CanActivateSkill)
            //{
            //CharController.CanMove = false;
            //DesactivateOtherSkill();
            _initTween = false;
            MyCharacterController.Instance.CurrentSkill = CurrentSkillType;
            _timer = Time.time;
            MyCharacterController.Instance.CharAnimator.SetBool("loading_ki", true);

            // On play les animations de particules
            foreach (var particle in Effects)
            {
                particle.Play();
            }
            // On active l'aura
            foreach (Transform obj in GameObject.Find("aura2").transform)
            {
                obj.gameObject.SetActive(true);
            }
            //On joue le son
            //if (!KaiokenChargeAudioSource.isPlaying)
            //{
            //    //KaiokenChargeAudioSource.Play();
            //}
            //// On bouge la camera
            //Camera.main.GetComponent<OrbitCamera>().shake();

            //}
        }

        public void CancelCharge()
        {
            //CharController.CanMove = true;
            MyCharacterController.Instance.CharAnimator.SetBool("loading_ki", false);

            // On stop les animations de particules
            foreach (var particle in Effects)
            {
                particle.Stop();
            }
            // On désactive l'aura
            foreach (Transform obj in GameObject.Find("aura2").transform)
            {
                obj.gameObject.SetActive(false);
            }

            ComboStarted();
        }



        #region ICombo implementation

        public void ComboStarted()
        {
            MyCharacterController.Instance.CharAnimator.SetBool("flying", true);
            MyCharacterController.Instance.CanMove = false;


            if (_ennemy != null)
                _isMoving = true;


            MyCharacterController.Instance.GetComponent<AudioSource>().PlayOneShot(KaiokenDash);
        }

        public void ComboFinished()
        {

        }

        #endregion
    }
}