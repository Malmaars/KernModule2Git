using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interface for things that can be targeted by IAgents
public interface ITargetable
{
    GameObject body { get; set; }
}
