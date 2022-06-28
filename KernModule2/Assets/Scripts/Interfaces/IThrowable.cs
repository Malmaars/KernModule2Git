using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interface for objects that can be picked up and thrown
public interface IThrowable : ITargetable
{
    CollissionDetector collissionDetector { get; set; }
    Rigidbody rb { get; set; }
    bool isBeingHeld { get; set; }
}
