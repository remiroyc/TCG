using UnityEngine;
using System.Collections;

public class SeaManager : MonoBehaviour
{
    private Vector3 minHeight, maxHeight;

    // Use this for initialization
    void Start()
    {
        maxHeight = transform.position;
        minHeight = (transform.up * -500);
    }

    // Update is called once pevr frame
    void Update()
    {

        //if(!up)
        //{
        //    if(transform.position.y >= minHeight.y){
        //        up = !up;
        //    }else{
        //     transform.position = Vector3.Slerp(transform.position, minHeight, Time.deltaTime * 2);
        //    }
        //}else{
            
        //     if(transform.position.y <= maxHeight.y){
        //          up = !up;
        //     }else{
        //          transform.position = Vector3.Slerp(this.transform.position, maxHeight, Time.deltaTime * 2);
        //     }

        //}

      
        //transform.position = Vector3.Slerp(transform.position, Vector3.down * 4f, Time.deltaTime * 2);

        
    }

}
