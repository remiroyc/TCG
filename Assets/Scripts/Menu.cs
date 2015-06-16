using UnityEngine;

public class Menu : MonoBehaviour
{
    private const float ScreenHeight = 720f;
    private const float ScreenWidth = 1280f;
    public GUIStyle ButtonStyle;
    public GUIStyle DescStyle;
    public GUIStyle MaxStyle;
    public GUIStyle TitleStyle;
    private int _cpt = 0;
    private Matrix4x4 _matrix;
    private bool _started;

    private void Start()
    {
        _matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / ScreenWidth, Screen.height / ScreenHeight, 1.0f));
    }

    private void OnGUI()
    {
        GUI.matrix = _matrix;
        GUI.Label(new Rect(0, ScreenHeight * 0.3f, ScreenWidth, ScreenHeight * 0.05f), "CELL GAMES", TitleStyle);
        GUI.Label(new Rect(200, ScreenHeight * 0.5f, ScreenWidth * 0.5f, ScreenHeight * 0.05f), LocalizationStrings.Instance.Values["GameShortDesc"], DescStyle);

        if (_started)
        {
            var rect = new Rect(0, ScreenHeight * 0.5f, ScreenWidth, ScreenHeight * 0.5f);
            GUI.Label(rect, LocalizationStrings.Instance.Values["Loading"], MaxStyle);
            Application.LoadLevel("cell_game_scene");
        }
        else
        {
            var rect = new Rect(200f, ScreenHeight * 0.8f, 500f, 100f);
            if (GUI.Button(rect, LocalizationStrings.Instance.Values["Play"], ButtonStyle))
            {
                _started = true;
                GameObject.Find("Part1").gameObject.SetActive(false);
                Camera.main.transform.GetComponent<Skybox>().enabled = false;
                // StartCoroutine (StartGame ());
            }
        }
    }

    /*
		IEnumerator StartGame ()
		{
				cpt = 3;
				yield return new WaitForSeconds (1);
				cpt = 2;
				yield return new WaitForSeconds (1);
				cpt = 1;
				yield return new WaitForSeconds (1);
				cpt = 0;
				Application.LoadLevel ("cell_game_scene");
		}
*/
}