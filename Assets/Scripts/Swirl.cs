using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Swirl : MonoBehaviour
{
    public List<Transform> l_swirls;

    // Use this for initialization
    void Start()
    {
        if (l_swirls == null)
            l_swirls = new List<Transform>();

        //iTween.ShakeScale(this.gameObject, Vector3.one, 500f * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {

        l_swirls[0].Rotate(new Vector3(0f, 0f, 5f));
        l_swirls[1].Rotate(new Vector3(0f, 0f, 8f));
        l_swirls[2].Rotate(new Vector3(0f, 0f, -10f));
        l_swirls[3].Rotate(new Vector3(0f, 0f, -3f));

        var randomNumber = Random.Range(1.5f,4f);

        //this.transform.localScale = Vector3.Slerp(this.transform.localScale, new Vector3(randomNumber, randomNumber, randomNumber), 10 * Time.deltaTime);

       
    }
}
