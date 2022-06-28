using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlackBoard
{
    //reference to the player, there is only one player at a time
    public static Player player { get; private set; }

    public static int killCount { get; private set; }

    //List of all the spawned things in the scene
    public static List<ISpawnable> spawned { get; private set; } = new List<ISpawnable>();


    //List of all throwables in the scene
    public static List<IThrowable> throwables { get; private set; } = new List<IThrowable>();

    public static List<IAudible> currentSounds { get; private set; } = new List<IAudible>();

    public static void AddSpawnable(ISpawnable spawnable) { spawned.Add(spawnable); }

    public static void RemoveSpawnable(ISpawnable spawnable){ spawned.Remove(spawnable); }

    public static void AddThrowable(IThrowable throwable){ throwables.Add(throwable); }

    public static void RemoveThrowable(IThrowable throwable){ throwables.Remove(throwable); }

    public static void SetPlayer(Player _player){ player = _player; }

    public static void AddSound(IAudible audible) { currentSounds.Add(audible); }
    public static void RemoveSound(IAudible audible) { currentSounds.Remove(audible); }

    public static void AddKill()
    {
        killCount++;
    }

    public static void ClearEverything()
    {
        spawned.Clear();
        throwables.Clear();
        currentSounds.Clear();
        killCount = 0;
    }
}
