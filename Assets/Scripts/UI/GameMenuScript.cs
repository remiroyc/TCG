using UnityEngine;

public class GameMenuScript : MonoBehaviour
{

    public GameObject MenuPanel;
    private bool _menuEnabled = false;

    private void Update()
    {
        if (!_menuEnabled && Input.GetKey(KeyCode.Return))
        {
            DisplayMenu();
        }
    }

    public void DisplayMenu()
    {
        _menuEnabled = true;
        GameManager.Instance.PauseEnabled = true;
    }

    public void HideMenu()
    {
        _menuEnabled = false;
        GameManager.Instance.PauseEnabled = false;
    }

}
