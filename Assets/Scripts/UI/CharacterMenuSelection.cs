using UnityEngine;
using UnityEngine.UI;

public class CharacterMenuSelection : MonoBehaviour
{

    public GameObject Pedestal;
    public CharacterSelection[] CharactersList;
    public Slider StrengthSlider, AgilitySlider, SpeedSlider;
    public Transform PlayerRespawn;

    public Image NotAvailable;
    public Button RightArrowBtn, LeftArrowBtn;

    private int _currentIndex;
    private bool _menuInitialized;

    public Transform SelectedCharacter
    {
        get
        {
            return CharactersList[_currentIndex].InstantiatedCharacter.transform;
        }
    }

    void Awake()
    {
        _menuInitialized = false;
        _currentIndex = 0;

    }

    void Start()
    {
        if (CharactersList.Length > 0)
        {

            RightArrowBtn.gameObject.SetActive(true);
            LeftArrowBtn.gameObject.SetActive(true);

            var characters = new GameObject("Characters");
            ReloadCharModel();

        }
        ReloadCharInfos();
    }

    void Update()
    {
        if (!_menuInitialized)
        {
            if (Camera.main.transform.position.y < 1.82f)
            {
                _menuInitialized = true;
            }
            else
            {
                Camera.main.transform.position -= (Vector3.up * Time.deltaTime);
                Camera.main.transform.position += (Vector3.forward * Time.deltaTime);
            }
        }
        else
        {

        }
    }

    public void ReloadCharInfos()
    {
        if (SelectedCharacter != null)
        {
            var player = SelectedCharacter.GetComponent<Player>();
            StrengthSlider.value = player.Strength;
            AgilitySlider.value = player.Agility;
            SpeedSlider.value = player.Speed;

            NotAvailable.gameObject.SetActive(!CharactersList[_currentIndex].IsAvailable);
        }
    }

    public void ReloadCharModel(Transform charactersTransform = null)
    {
        CharactersList[_currentIndex].InstantiatedCharacter = (GameObject)Instantiate(CharactersList[_currentIndex].PrefabCharacter,
            PlayerRespawn.position, Quaternion.LookRotation(-Vector3.forward));
        CharactersList[_currentIndex].InstantiatedCharacter.transform.parent = charactersTransform ?? GameObject.Find("Characters").transform;
    }

    public void ChangeCharacter(bool right)
    {
        Destroy(SelectedCharacter.gameObject);

        if (right)
        {
            if (_currentIndex + 1 == CharactersList.Length)
            {
                _currentIndex = 0;
            }
            else
            {
                ++_currentIndex;
            }
        }
        else
        {
            if (_currentIndex - 1 < 0)
            {
                _currentIndex = CharactersList.Length - 1;
            }
            else
            {
                --_currentIndex;
            }
        }

        ReloadCharModel();
        ReloadCharInfos();


    }

}
