using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollissionDetector : MonoBehaviour
{
    public bool collisionBool;
    public float collisionSpeed;
    private void OnCollisionEnter(Collision collision)
    {
        collisionBool = true;

        if(collision.rigidbody != null)
        collisionSpeed = collision.rigidbody.velocity.magnitude;
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    collisionBool = false;
    //    collisionSpeed = 0;
    //}

    private void Update()
    {
        
    }
}
