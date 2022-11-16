using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCatcher : MonoBehaviour
{
    public int ObjectiveNumberRed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Blue"))
        {
            Destroy(other.gameObject);
            
        }
        else if (other.CompareTag("Red"))
        {
            Destroy(other.gameObject);

            ObjectiveNumberRed -= 1;
        }

    }
}

