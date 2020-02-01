using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(gameObject.transform.localRotation.x, gameObject.transform.localRotation.y, -(gameObject.transform.localRotation.z + rotationSpeed));        
    }
}
