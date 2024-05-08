using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{   
    //target dat word gevolg door camera
    [SerializeField] private Transform target;
    //hoe snel de camera volgt 
    [SerializeField]
    [Range(0.01f, 1f)]
    private float smoothSpeed = 0.125f;
    //verschil tussen target en camera
    [SerializeField] private Vector3 offset;
    //standaard geen verschil tussen target en camrera
    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update

    private void LateUpdate()
    {
        //waar de camera naar toe moet
        Vector3 desiredPosition = target.position + offset;
        //bewegen van de camera
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
    }
    
}
