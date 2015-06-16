using UnityEngine;

public class RoshiMuscularScript : MonoBehaviour
{

    [HideInInspector]
    public bool Grounded;
    public Animator CharAnimator;
    public float HeightGroundRaycast = 0.9f; // Hauteur qui permet de tracer un raycast pour savoir si le personnage est en contact avec le sol
    public LayerMask GroundLayer;

    private void Start()
    {
    }

    private void Update()
    {
        Grounded = Physics.Raycast(transform.position, Vector3.down, HeightGroundRaycast, GroundLayer);
        CharAnimator.SetBool("grounded", Grounded);
    }
}
