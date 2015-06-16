using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject LoadingPanel;
    public BaseTutorial[] Tutorials;

    private bool _messageBoxDisplayed = false;
    public bool MessageBoxDisplayed
    {
        get { return _messageBoxDisplayed; }
        set
        {
            _messageBoxDisplayed = value;
            TalkPanel.gameObject.SetActive(value);
        }
    }

    public int CurrentTutorial = 0;

    public RectTransform TalkPanel;
    public Text MessageBoxText;

    public OrbitCamera CameraScript;
    public RoshiMaestroScript RoshiScript;


    public void ChangeLevel()
    {
        Application.LoadLevel(1);
    }

    void Awake()
    {
        MessageBoxText.supportRichText = true;
    }

    public void DisplayMessage(string msg)
    {
        MessageBoxDisplayed = true;
        MessageBoxText.text = msg;
    }

    public void ActivateNextTutorial()
    {
        if (CurrentTutorial < Tutorials.Length)
        {
            if (CurrentTutorial != 0 && Tutorials[CurrentTutorial - 1] != null)
            {
                var previousTuto = Tutorials[CurrentTutorial - 1];
                var tutorial = previousTuto.GetComponent<BaseTutorial>();
                if (tutorial.MusicAudioSource != null)
                {
                    tutorial.MusicAudioSource.Stop();
                }
                previousTuto.gameObject.SetActive(false);
            }
            RoshiScript.ActivateRoshiTalk();
        }
        else
        {
            LoadingPanel.SetActive(true);
            Application.LoadLevel(1);
        }


    }

}
