using UnityEngine;


public class RoshiMaestroScript : MonoBehaviour
{

    private Animator _charAnimator;
    private bool _sleep = true, _talk = false, _activated = false, _shooted = false;
    private int _nbSentence;
    private int _nbPunchReceived;
    
    private Vector3 _initialRoshiCamPosition; // Position initiale de la caméra liée à Tortue Géniale

    private const string TravelingAnimationName = "RoshiSangokuTraveling";
    private bool _gettingHit = false;

    public Transform Target;
    public GameObject PnjIcon;
    public TutorialManager TutorialManager;
    public OrbitCamera CameraScript;
    public Camera PnjCamera;

    public AudioClip[] RoshiAudios;
    public MyCharacterController CharMouvementScript;

	public bool MissionFailed = false; 

    #region ROSHI SENTENCES

    public readonly string[] RoshiFirstSentences =
    {
        "Hey Sangoku, La journée commence bien... J'imagine que tu viens ici pour l'entrainement.\nCa tombe bien tu es tombé sur <b>un vrai champion</b>! Allez c'est parti...",
        "Commencons par les bases : le déplacement.",
        "Utilise le joystick gauche pour te déplacer ou réaliser des pas de côté.\nEn touchant ton écran avec ton pouce droit, tu peux manipuler la caméra et réaliser des rotations.",
        "Essaye de te déplacer sur les 4 sphères lumineuses !"
    };

    public readonly string[] RoshiSecondSentences =
    {
        "Bravo avec le temps tu apprendras à maitriser ta vitesse.", 
        "Avec ce nouveau bouton C tu vas pouvoir effectuer des sauts.",
        "Essaye sans attendre et tente de franchir les cibles avant la fin du compte à rebours."
    };

	public readonly string[] RoshiSecondeSentencesFail = {
		"Dommage, encore un peu trop lent!",
		"Visualise bien les sphères avant de t'élancer."
	};

    public readonly string[] RoshiThirdSentences =
    {
        "Maintenant testons ensemble les enchainements de combat. Tu diposes de deux boutons A et B qui te permettent d'activer tes techniques de combat.", 
        "Essaye de me donner un coup de poing avec A."
    };

    public readonly string[] RoshiFourthSentences =
    {
        "Tu apprends vite! En apprenant les combinaisons suivantes tu pourras terasser tes adverssaires...",
        "Tiens toi prêt Sangoku à me défier !"
    };

    #endregion

    #region MONO BEHAVIOUR METHODS

    private void Awake()
    {
        _charAnimator = GetComponent<Animator>();
        CharMouvementScript = Target.GetComponent<MyCharacterController>();
        _initialRoshiCamPosition = PnjCamera.transform.position;
    }

    private void Update()
    {
        _charAnimator.SetBool("Sleep", _sleep);

        if (!_activated)
        {
            var activate = (transform.position - Target.position).sqrMagnitude <= 5;
            if (activate)
            {
                var scream = Resources.Load<AudioClip>("Sounds/RoshiMaestro/scream");
                GetComponent<AudioSource>().PlayOneShot(scream);
                _activated = true;
                ActivateRoshiTalk();
            }
        }
    }

    #endregion

    #region TALK

    private void ActivateAnimationTalk()
    {
        var finalPosition = CharMouvementScript.transform.position - CharMouvementScript.transform.right - CharMouvementScript.transform.forward;
        iTween.MoveTo(PnjCamera.gameObject, iTween.Hash("position", finalPosition, "name", TravelingAnimationName,
           "delay", 2, "looktarget", CharMouvementScript.transform, "time", 3, "looptype", "pingPong"));
    }

    private void StopAnimationTalk()
    {
        iTween.StopByName(PnjCamera.gameObject, TravelingAnimationName);
        PnjCamera.transform.position = _initialRoshiCamPosition;
    }

    public void ActivateNextSentence()
    {
        var index = Random.Range(0, RoshiAudios.Length);
        AudioSource.PlayClipAtPoint(RoshiAudios[index], transform.position);

        var tab = GetRoshiSentences();
        if (++_nbSentence > tab.Length - 1)
        {
            EndTalk();
        }
        else
        {
            TutorialManager.DisplayMessage(tab[_nbSentence]);
        }
    }

    private string[] GetRoshiSentences()
    {
        var tab = new string[0];
        switch (TutorialManager.CurrentTutorial)
        {
            case 0:
                tab = RoshiFirstSentences;
                break;
            case 1:
				tab = MissionFailed ? RoshiSecondeSentencesFail : RoshiSecondSentences;
                break;
		case 2:
			tab = RoshiThirdSentences;
			break;
        }

		Debug.Log("TutorialManager.CurrentTutorial : " + TutorialManager.CurrentTutorial);

        return tab;
    }

    private void EndTalk()
    {

        TutorialManager.MessageBoxDisplayed = false;
        CharMouvementScript.CanMove = true;
        _talk = false;
        CameraScript.enabled = true;
        StopAnimationTalk();

        PnjCamera.gameObject.SetActive(false);

		if(MissionFailed){
			MissionFailed = false;
		}

        var tutorial = TutorialManager.Tutorials[TutorialManager.CurrentTutorial];
        tutorial.gameObject.SetActive(true);
        tutorial.StartTutorial();

    }

    /// <summary>
    /// Activation de l'explication du tutoriel par Tortue Géniale.
    /// </summary>
    public void ActivateRoshiTalk()
    {
        _talk = true;
        CameraScript.enabled = false;

        PnjCamera.transform.position = transform.position + (-transform.forward * 0.5f) + (transform.right * 0.5f);
        PnjCamera.transform.localRotation = Quaternion.Euler(0, 350, 0);
        PnjCamera.gameObject.SetActive(true);

		CharacterSight.Instance.SightImage.enabled = false;

		CharMouvementScript.CanMove = false;
        Target.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ActivatePnj(true);
        ActivateAnimationTalk();
        _nbSentence = 0;
		var sentences = GetRoshiSentences();

		if(sentences.Length > 0){
			TutorialManager.DisplayMessage(sentences[0]);
		}

    }

    #endregion

    private void ActivatePnj(bool activate)
    {
        PnjIcon.SetActive(!activate);
        _sleep = !activate;
        PnjIcon.transform.LookAt(Target);
        transform.LookAt(Target);
    }

    public void TakeDamage(string skill)
    {
        ApplyDamage(new TakeDamageModel()
        {
            SkillName = skill,
            Default = true,
            CanDodge = false
        });
    }

    public void ApplyDamage(TakeDamageModel model)
    {
        _shooted = true;
        // var hit = Resources.Load("CFXPrefabs_Mobile/Hits/CFXM_Hit_C White") as GameObject;
        // Instantiate(hit, transform.position, Quaternion.identity);

        if (TutorialManager.CurrentTutorial == 1)
        {
            if (++_nbPunchReceived == 2)
            {
                FindObjectOfType<FightTutorialScript>().PunchReceived();
            }
            _charAnimator.Play(CharMouvementScript.RightAttack ? "RightBlockAttack" : "LeftBlockAttack");
            return;
        }

        if (!model.Default)
        {
            //_animator.SetBool("hit_right_attack", model.RightAttack);
            switch (model.AttackTypes)
            {

                case AttackTypes.Weak:

                    //_animator.SetTrigger("hit_weak_attack");
                    //PlayRandomPainAudioClip();
                    //Instantiate(Resources.Load("CFXM_Hit_A Red"), transform.position, Quaternion.identity);

                    ////yield return new WaitForSeconds(0.5f);
                    ////_gettingHit = false;

                    //if (model.AttackMultiplicator >= 1.5f)
                    //{
                    //    rigidbody.AddForce(-transform.forward * 250);
                    //}

                    //Life -= (200 * model.AttackMultiplicator);
                    //StartCoroutine(WaitAndRandomAction(Random.Range(0.5f, 1.5f)));

                    break;

                case AttackTypes.Medium:

                    //Life -= 300;
                    //_animator.SetTrigger("hit_medium_attack");
                    //PlayRandomPainAudioClip();
                    //Instantiate(Resources.Load("CFXM_Hit_A Red"), transform.position, Quaternion.identity);

                    //StartCoroutine(WaitAndRandomAction(Random.Range(0.5f, 1.5f)));

                    break;

            }

        }
        //else
        //{
        //    switch (model.SkillName)
        //    {
        //        case "weak_attack":
        //            _charAnimator.Play(_charMouvementScript.RightAttack ? "RightBlockAttack" : "LeftBlockAttack");
        //            break;

        //        default:
        //            Debug.Log(model.SkillName);
        //            break;
        //    }
        //}

    }

}
