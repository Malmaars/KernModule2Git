using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IListener
{
    IAudible audibleClosest { get; set; }
    GameObject body { get; set; }
    float hearingRange { get; set; }
}
