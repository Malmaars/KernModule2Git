using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface for things that can grab throwables
public interface IGrabbable
{
    bool isThrowing { get; set; }
    GameObject body { get; set; }
    Vector3 hand { get; set; }
    Quaternion objectRotation { get; set; }

    IThrowable nearestThrowable { get; set; }
    IThrowable currentlyHeldThrowable { get; set; }

    float throwStrength { get; set; }
    float grabRange { get; set; }
}
