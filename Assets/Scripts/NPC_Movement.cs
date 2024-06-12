using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    public float speed;
    private float movement = 0;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (movement < 100)
        {
            movement += 0.05f;
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
        else
        {
            movement = 0;
            transform.forward = -transform.forward;
        }
        
    }
}
