using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public static SaveData current;

    // Play time
    public float hours;
    public float minutes;
    public float seconds;

    // Constructor
    public SaveData()
    {
        hours = 0;
        minutes = 0;
        seconds = 0;
    }
}
