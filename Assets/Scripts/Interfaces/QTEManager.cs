using UnityEngine;
using System.Collections;
using Assets.Scripts.SkillScript;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class QTEManager : MonoBehaviour
{

    //public ICombo combo;
    public GameObject QteBtn;
    //public GameObject UIRoot;
    //public float TimeToWaitBetweenActions = 3f;

    void Awake()
    {
        //if (combo == null)
        //{
        //    combo = transform.GetComponent(typeof(ICombo)) as ICombo;
        //}
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void TeleportBehindEnemy(Enemy target)
    {
        // Son
        var rand = Random.value;
        if (rand <= .33f)
            GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/teleport"));
        else if (rand > .33f && rand < .77f)
            GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/teleport00"));
        else
            GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/teleport2"));

        // Effet

        // Changement Position
        transform.position = target.transform.position - target.transform.forward;
    }


    //public void GenerateTapSequence(int numberOfSteps)
    //{
    //    List<GameObject> l_pastilles = new List<GameObject>();


    //    for (int i = 0; i <= numberOfSteps; i++)
    //    {
    //        GameObject pastille = GameObject.Instantiate(PastillePrefab, Vector3.zero, Quaternion.identity) as GameObject;
    //        pastille.transform.parent = UIRoot.transform;
    //        pastille.SetActive(false);
    //        //On le place aleatoirement entre les bords de l'écran 
    //        pastille.transform.position = new Vector2(Random.Range(0f, Screen.width), Random.Range(0f, Screen.height));
    //        var btn = pastille.GetComponent<Button>();
    //        btn.onClick.AddListener(() => { OnPastilleHit(); });
    //        l_pastilles.Add(pastille.gameObject);
    //    }

    //    StartCoroutine (PlaySequence(l_pastilles));
        
    //}


    //private IEnumerator PlaySequence(List<GameObject> l_pastilles)
    //{
    //    print("Coroutine started");

    //    foreach (var pastille in l_pastilles)
    //    {
    //        pastille.transform.position = new Vector2(Random.Range(0f, Screen.width), Random.Range(0f, Screen.height));
    //        //TODO: A remettre a true
    //        pastille.SetActive(false);
    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    //public void OnPastilleHit()
    //{
    //    //print("Bien joué !!");
    //}
}
