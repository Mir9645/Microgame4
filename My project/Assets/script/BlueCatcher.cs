using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCatcher : MonoBehaviour
{
    public int ObjectiveNumberBlue;
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

            ObjectiveNumberBlue -= 1;
        }

    }
}
