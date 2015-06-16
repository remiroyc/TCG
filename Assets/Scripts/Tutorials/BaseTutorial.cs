using UnityEngine;

public abstract class BaseTutorial : MonoBehaviour
{
    public RoshiMaestroScript RoshiManager;
    public AudioSource MusicAudioSource;
    public TutorialManager TutorialManager;

    public bool Successfull { get; set; }
    public abstract void Retry();
    public abstract void StartTutorial();

}

