using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interface for things that can be spawned in
public interface ISpawnable
{
    //the Gameobject in scene of what spawns
    GameObject body { get; set; }
    Vector3 position { get; set; }
    Quaternion rotation { get; set; }

    bool LogicUpdate();
}
