using System;
using UnityEngine;

[Obsolete]
public class HubScript : MonoBehaviour
{

    public GUISkin Skin;
    public MyCharacterController CharController;
    public Texture CharacterTexture, Bars, Lifebar;
    private const float MaxLifebarSize = 297, MaxKibarSize = 227;
    private float _initialLife, _initialKi;
    private const float ScreenHeight = 720f;
    private const float ScreenWidth = 1280f;
    private Matrix4x4 _matrix;

    private void Start()
    {
        _initialLife = CharController.MaxLife;
        _initialKi = CharController.MaxKi;
        _matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / ScreenWidth, Screen.height / ScreenHeight, 1.0f));
    }

    private void OnGUI()
    {
        GUI.skin = Skin;
        GUI.matrix = _matrix;

        GUI.DrawTexture(new Rect(190, 50, 300, 74), Bars);

        GUI.color = Color.red;
        var size = (CharController.Life * MaxLifebarSize) / _initialLife;
        GUI.DrawTexture(new Rect(192, 51, size, 39), Lifebar);
        GUI.color = Color.black;
        GUI.Label(new Rect(192, 52, MaxLifebarSize, 37), string.Format("{0} / {1}", (int)CharController.Life, _initialLife));

        GUI.color = Color.blue;
        var kiSizeBar = (CharController.Ki * MaxKibarSize) / _initialKi;

        GUI.DrawTexture(new Rect(191, 93, kiSizeBar, 29), Lifebar);
        GUI.color = Color.black;
        GUI.Label(new Rect(191, 93, MaxKibarSize, 29), string.Format("{0} / {1}", (int)CharController.Ki, _initialKi));

        GUI.color = Color.white;
        GUI.DrawTexture(new Rect(10, 10, 205, 205), CharacterTexture);
    }


}
