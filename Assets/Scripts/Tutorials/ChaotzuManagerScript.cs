using DG.Tweening;
using System.Linq;
using UnityEngine;

/// <summary>
/// Script qui gère le déplacement du PNJ Chaotzu
/// </summary>
public class ChaotzuManagerScript : MonoBehaviour
{
    public Transform[] Waypoints;
    private Animator _charAnimator;

    private void Awake()
    {
        _charAnimator = GetComponent<Animator>();
        transform.position = Waypoints[0].transform.position;
    }

    private void Start()
    {
        MoveToWaypoint();
        _charAnimator.SetBool("Run", true);
    }

    /// <summary>
    /// Méthode qui permet au PNJ de se déplacer grâce aux différents waypoints.
    /// </summary>
    private void MoveToWaypoint()
    {
        transform.DOPath(Waypoints.Select(w => w.transform.position).ToArray(), 20f)
           .SetLookAt(0.01f) 
           .SetEase(Ease.Linear)
           .SetLoops(-1);
    }

}
