using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleThrowable : IThrowable
{
    public CollissionDetector collissionDetector { get; set; }
    public Rigidbody rb { get ; set ; }
    public bool isBeingHeld { get; set; }
    public GameObject body { get; set; }

    public SimpleThrowable(GameObject _body)
    {
        body = _body;
        rb = body.GetComponent<Rigidbody>();
        collissionDetector = body.GetComponent<CollissionDetector>();
        BlackBoard.AddThrowable(this);
    }
}
