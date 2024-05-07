using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour
{   
    private Vector3 startposition = Vector3.zero;
    private Vector3 endposition = Vector3.zero;
    [SerializeField] private float speed = 1.0f;
    
    // Start is called before the first frame update
    void Start()
    {   //startpositie wordt huidige positie enemy
       startposition = gameObject.transform.localPosition;
        //beweegd op en neer van start naar eind positie
       endposition = new Vector3(startposition.x,startposition.y + 5);
    }

    // Update is called once per frame
    void Update()
    {
        //math.f zorgt voor soepele beweging tussen 2 punten
        transform.position = Vector3.Lerp(startposition, endposition, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f);
    }
}
