using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JumpTutorialScript : BaseTutorial, IAreaCourse
{
    /// <summary>
    /// Tableaux des cibles où sangoku doit se rendre pour réussir l'épreuve
    /// </summary>
    public List<AreaTutorialScript> TargetAreas;
    public Transform Arrow;
    /// <summary>
    /// Temps maximum pour réaliser le tutorial
    /// </summary>
    public float MaxTimer;
    public Text TimerText;
    public Transform CButton;
    public Camera TravelingCamera;
    public int TravelingRate = 20;
    private int _currentIndex;
    private Transform _focusArea;
    private float _timer;
    private bool _travelingCameraEnabled = false;

    private void Start()
    {
        DOTween.Init().SetCapacity(30, 15);
        TravelingCamera.transform.position = (RoshiManager.CharMouvementScript.transform.position +
                                              RoshiManager.CharMouvementScript.transform.up);
		MyCharacterController.Instance.CanFly = false;
    }

    // ReSharper disable once UnusedMember.Local
    private void Update()
    {
        if (!_travelingCameraEnabled)
        {
            var nbSeconds = Time.time - _timer;
            if (nbSeconds >= MaxTimer)
            {
                Fail();
            }
            else
            {
                TimerText.text = ((int)(MaxTimer - nbSeconds)).ToString();
            }
            Arrow.LookAt(TargetAreas[_currentIndex].transform);
        }
    }


    public override void StartTutorial()
    {
        _currentIndex = 0;
        foreach (var area in TargetAreas)
        {
            area.gameObject.SetActive(true);
        }
        TravelingCamera.gameObject.SetActive(true);
        _travelingCameraEnabled = true;

        MoveTravelingCamera();

    }

    public void StartRace()
    {
        _currentIndex = 0;
        _timer = Time.time;
        Arrow.gameObject.SetActive(true);
        if (MusicAudioSource != null)
        {
            MusicAudioSource.Play();
        }
        _currentIndex = 0;
        _focusArea = TargetAreas[_currentIndex].transform;
        _focusArea.gameObject.SetActive(true);
        TimerText.gameObject.SetActive(true);
        CButton.gameObject.SetActive(true);
		CharacterSight.Instance.SightImage.enabled = true;
    }

    public void Fail()
    {
        TimerText.gameObject.SetActive(false);
        if (MusicAudioSource != null && MusicAudioSource.isPlaying)
        {
            MusicAudioSource.Stop();
        }

		TimerText.gameObject.SetActive(false);
		CButton.gameObject.SetActive(false);
		Arrow.gameObject.SetActive(false);

		RoshiManager.MissionFailed = true;
		RoshiManager.ActivateRoshiTalk();
        // Retry();
    }

    public override void Retry()
    {
        StartRace();
    }

    void IAreaCourse.FocusNextArea(AreaTutorialScript currentArea)
    {
        _focusArea.gameObject.SetActive(false);

        var nextIndex = TargetAreas.IndexOf(currentArea) + 1;
        if (nextIndex >= TargetAreas.Count)
        {
            Arrow.gameObject.SetActive(false);
			CButton.gameObject.SetActive(false);
			TimerText.gameObject.SetActive(false);

            Successfull = true;
            RoshiManager.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/RoshiMaestro/youhou"));
			StartCoroutine(WaitGroundedAndActivateNextTutorial());
        }
        else
        {
            _currentIndex = nextIndex;
            _focusArea = TargetAreas[nextIndex].transform;
            _focusArea.gameObject.SetActive(true);
        }
    }

	private IEnumerator WaitGroundedAndActivateNextTutorial(){
		while(!MyCharacterController.Instance.Grounded){
			yield return new WaitForFixedUpdate();
		}
		TutorialManager.CurrentTutorial++;
		TutorialManager.ActivateNextTutorial();
	}

    public void ContinueTraveling()
    {
        _currentIndex++;
        if (!_travelingCameraEnabled || _currentIndex >= TargetAreas.Count)
        {
            _travelingCameraEnabled = false;
            TravelingCamera.gameObject.SetActive(false);
            StartRace();
        }
        else
        {
            MoveTravelingCamera();
        }
    }

    private void MoveTravelingCamera()
    {
        var area = TargetAreas[_currentIndex].transform;
        var camTravelingSequence = DOTween.Sequence();
        camTravelingSequence.Append(TravelingCamera.transform.DOLookAt(area.transform.position, 0.5f));
        camTravelingSequence.Join(TravelingCamera.transform.DOMove(area.transform.position, 1.5f));
        camTravelingSequence.OnComplete(ContinueTraveling);
    }


}
