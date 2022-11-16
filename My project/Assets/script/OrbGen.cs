using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbGen : MonoBehaviour
{
    public GameObject Circle;
    public float SpawnRate;
    public float Timer;
    public GameObject Player;
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        SpawnRate = 1f;
        Timer = 0;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            Player.transform.position -= transform.right * (Time.deltaTime * Speed);
        }    
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Player.transform.position += transform.right * (Time.deltaTime * Speed);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Timer += Time.fixedDeltaTime;
        if (Timer >= SpawnRate)
        {
            Spawn();
            Timer = 0;
            Time.fixedDeltaTime = Timer;
        }
    }

    public void Spawn()
    {
      Instantiate(Circle, transform.position, Quaternion.identity);
    }
}
