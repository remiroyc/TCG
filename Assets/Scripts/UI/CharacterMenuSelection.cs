using UnityEngine;

public class CharacterMenuSelection : MonoBehaviour
{

    public GameObject Pedestal;
    public CharacterSelection[] CharactersList;

    private Vector3 _characterPosition;
    private int _currentIndex;
    private bool _menuInitialized;

    void Awake()
    {
        _menuInitialized = false;
        _currentIndex = 0;
    }

    void Start()
    {
        if (CharactersList.Length > 0)
        {
            var characters = new GameObject("Characters");
            _characterPosition = Pedestal.transform.position + Vector3.up;
            CharactersList[0].InstantiatedCharacter = (GameObject)Instantiate(CharactersList[0].PrefabCharacter, _characterPosition, Quaternion.LookRotation(-Vector3.forward));
            CharactersList[0].InstantiatedCharacter.transform.parent = characters.transform;
        }
    }

    void Update()
    {
        if (!_menuInitialized)
        {
            if (Camera.main.transform.position.y - 1 < (Pedestal.transform.position.y * 0.2f))
            {
                Pedestal.GetComponent<Rigidbody>().velocity = Vector3.zero;
                _menuInitialized = true;
            }
            else
            {
                Camera.main.transform.position -= (Vector3.up * Time.deltaTime);
                Camera.main.transform.position += (Vector3.forward * Time.deltaTime) / 2;
            }
        }
        else
        {

        }
    }
}
