using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interface for things that can receive damage
public interface IDamagable
{
    CollissionDetector collissionDetector { get; set; }
    int Health { get; set; }
}
