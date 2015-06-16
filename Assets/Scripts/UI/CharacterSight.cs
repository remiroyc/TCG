using UnityEngine;
using UnityEngine.UI;

public class CharacterSight : MonoBehaviour
{
   // public Transform SightObj;
    public LayerMask TargetLayerMask;
    public Image SightImage;

    #region SINGLETON

    private static CharacterSight _instance;
    public static CharacterSight Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<CharacterSight>()); }
    }

    #endregion

    protected void Awake()
    {
        _instance = this;
    }

    public void SetVisible(bool visible)
    {
        SightImage.enabled = visible;
        //   SightImage.GetComponent<Animation>().Play(visible ? "CharacterSight.Display" : "CharacterSight.Hide");
    }

    public Vector3 GetImpactPoint()
    {
        var dir = transform.position - Camera.main.transform.position;
        var ray = new Ray(Camera.main.transform.position, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, TargetLayerMask))
        {
			/*
#if UNITY_EDITOR
            if (SightObj != null)
            {
                SightObj.transform.position = hit.point;
            }
#endif
*/
            return hit.point;
        }
        return Vector3.zero;
    }

    //#if UNITY_EDITOR
    //    void FixedUpdate()
    //    {
    //        Debug.Log(GetImpactPoint());
    //    }
    //#endif

}
