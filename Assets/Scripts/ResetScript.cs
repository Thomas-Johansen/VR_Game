using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResetScript : MonoBehaviour
{
    public GameObject target1;
    public GameObject target2;
    public GameObject target3;
    public GameObject target4;
    public GameObject target5;
    public GameObject target6;
    public GameObject target7;
    
    void OnTriggerEnter(Collider collision)
    {
        target1.SetActive(true);
        target2.SetActive(true);
        target3.SetActive(true);
        target4.SetActive(true);
        target5.SetActive(true);
        target6.SetActive(true);
        target7.SetActive(true);
    }
}
