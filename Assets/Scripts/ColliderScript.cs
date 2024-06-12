using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class ColliderScript : MonoBehaviour
{
    public GameObject explosion;
    private bool hasHit = false;
    private float size = 0;
    private bool sizeMax = false;
    private GameObject currentObject;

    private void Awake()
    {
        explosion.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("Planet"))
        {
            hasHit = true;
            explosion.SetActive(true);
            currentObject = collision.gameObject;
            explosion.transform.position = this.transform.position;
        } else if (collision.gameObject.CompareTag("Target"))
        {
            collision.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (hasHit)
        {
            explosion.transform.localScale = new Vector3(size, size, size);
            if (size < 200 & !sizeMax)
            {
                size += 0.1f;
            } 
            else if (size >= 200 & !sizeMax)
            {
                sizeMax = true;
                currentObject.SetActive(false);
                Planets.makeUnactive(currentObject.gameObject.name);
            }
            else if (size > 0)
            {
                size -= 1f;
            }
            else
            {
                hasHit = false;
                explosion.SetActive(false);
                sizeMax = false;
            }
        }
    }
}
