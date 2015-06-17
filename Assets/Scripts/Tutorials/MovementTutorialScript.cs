using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTutorialScript : BaseTutorial, IAreaCourse
{

    /// <summary>
    /// Tableaux des cibles où sangoku doit se rendre pour réussir l'épreuve
    /// </summary>
    public List<AreaTutorialScript> TargetAreas;
    public Transform Arrow;
	public RectTransform FirstTutorialPanel;

    private Transform _focusArea;
    private int _currentIndex;


    private void Update()
    {
        Arrow.LookAt(TargetAreas[_currentIndex].transform);
    }

    public override void StartTutorial()
    {
        Arrow.gameObject.SetActive(true);
        MusicAudioSource.Play();
        _currentIndex = 0;
        _focusArea = TargetAreas[_currentIndex].transform;
        _focusArea.gameObject.SetActive(true);
		CharacterSight.Instance.SightImage.enabled = true;
		StartCoroutine(DisplayTempHelp());
    }

	private IEnumerator DisplayTempHelp(){

		FirstTutorialPanel.gameObject.SetActive(true);
		yield return new WaitForSeconds(5f);
		FirstTutorialPanel.gameObject.SetActive(false);
	}

    public override void Retry()
    {
    }

    void IAreaCourse.FocusNextArea(AreaTutorialScript currentArea)
    {
        _focusArea.gameObject.SetActive(false);
        var nextIndex = TargetAreas.IndexOf(currentArea) + 1;
        if (nextIndex >= TargetAreas.Count)
        {
            Arrow.gameObject.SetActive(false);
            Successfull = true;
            RoshiManager.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/RoshiMaestro/youhou"));
            TutorialManager.CurrentTutorial++;
            TutorialManager.ActivateNextTutorial();
        }
        else
        {
            _currentIndex = nextIndex;
            _focusArea = TargetAreas[nextIndex].transform;
            _focusArea.gameObject.SetActive(true);
        }
    }
}
