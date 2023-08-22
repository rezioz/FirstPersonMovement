using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveStuff : MonoBehaviour
{
    public Vector3 movement;


    private void Start()
    {
        
        
        
    }


    private void FixedUpdate()
    {

        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position +movement*Time.deltaTime);

        

    }
    
    

}
