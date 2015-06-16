using UnityEngine;

/// <summary>
/// Ce script permet de gérer la collision entre le joueur et la zone ciblée (pour le tutoriel de déplacement)
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class AreaTutorialScript : MonoBehaviour
{

    public IAreaCourse AreaCourse;

    private void Awake()
    {
        if (AreaCourse == null)
        {
            AreaCourse = transform.parent.GetComponent(typeof(IAreaCourse)) as IAreaCourse;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            var bip = Resources.Load<AudioClip>("Sounds/bip");
            AudioSource.PlayClipAtPoint(bip, transform.position);
            AreaCourse.FocusNextArea(this);
        }
        else if (other.tag.Equals("TravelingCamera"))
        {
            var bip = Resources.Load<AudioClip>("Sounds/bip");
            AudioSource.PlayClipAtPoint(bip, transform.position);
            transform.gameObject.SetActive(false);
        }
    }

}
