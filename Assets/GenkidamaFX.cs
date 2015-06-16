using UnityEngine;

public class GenkidamaFX : MonoBehaviour
{

    private GameObject _myPlayer;
    private OrbitCamera _mainCamera;

    public void Start()
    {
        _myPlayer = GameObject.Find("MyPlayer");
        _mainCamera = FindObjectOfType<OrbitCamera>();
    }

    private void OnParticleCollision(GameObject other)
    {
        GameObject.Find("explosion_sound").GetComponent<AudioSource>().Play();
        //GameObject.Find("apocalypse_explosion").GetComponent<AudioSource>().Play();
        //GameObject.Find("apocalypse_heartbeat_sound").GetComponent<AudioSource>().Play();

        _myPlayer.GetComponent<Rigidbody>().useGravity = true;
        var myCharacterController = _myPlayer.GetComponent<MyCharacterController>();
        if (myCharacterController != null)
        {
            myCharacterController.CanAttack = true;
            myCharacterController.CanMove = true;
        }
        _myPlayer.GetComponentInChildren<Animator>().Play("Idle");

        // On met la camera comme il faut
        _mainCamera.enabled = true;
        // MainCamera.ShakeGenki(); // Il faut reporter la méthode
        //iTween.ShakePosition(_mainCamera.gameObject, Vector3.one *3, 3f * Time.deltaTime);

        //On change la map 
        myCharacterController.GenkidamaManager.mapToReveal.GetComponent<MeshRenderer>().enabled = true;
        myCharacterController.GenkidamaManager.mapToReveal.GetComponent<MeshCollider>().enabled = true;
        myCharacterController.GenkidamaManager.mapToDestroy.SetActive(false);

        GameObject.Find("CellGamePrefab").SetActive(false);
        //GameObject.Find("Group1").SetActive(false);


        foreach (var particle in GameObject.Find("FX_apocalypse").GetComponentsInChildren<ParticleSystem>())
        {
            particle.Play();
        }

    }
}
