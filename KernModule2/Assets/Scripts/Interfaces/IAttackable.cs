using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interface for things that can attack Damagables
public interface IAttackable
{
    int Damage { get; set; }
}
