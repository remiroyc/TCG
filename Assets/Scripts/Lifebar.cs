using UnityEngine;

public class Lifebar : MonoBehaviour
{
    public const float MaxScale = 0.5f; // Longueur maximale de l'objet barre de vie
    private Enemy _enemy; // Référence du personnage
    private float _maxLife; // Vie maximum du personnage enemie

    private void Start()
    {
        _enemy = transform.parent.GetComponent<Enemy>();
        _maxLife = _enemy.Life;
    }

    private void Update()
    {
        // Mise à jour de la valeur de la barre de vie
        if (_enemy != null)
        {
            float newScale;
            if (_enemy.Life >= _maxLife)
            {
                newScale = MaxScale;
            }
            else
            {
                newScale = (MaxScale * _enemy.Life) / _maxLife;
            }
            transform.localScale = new Vector3(newScale, transform.localScale.y, 0);
        }

        // Mise à jour de l'orientation de la barre de vie
        if (MyCharacterController.Instance.transform != null)
        {
            Vector3 relativePos = MyCharacterController.Instance.transform.position - transform.position;
            Quaternion lookRot = Quaternion.LookRotation(relativePos);
            lookRot.eulerAngles = new Vector3(0, lookRot.eulerAngles.y, lookRot.eulerAngles.z);
            transform.rotation = lookRot;
        }
    }

}
