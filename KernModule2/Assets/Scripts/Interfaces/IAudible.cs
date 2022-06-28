using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudible : ITargetable
{
    bool makingSound { get; set; }
    float soundRange { get; set; }
}
