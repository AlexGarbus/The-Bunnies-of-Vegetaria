using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class SaveLoad
{
    /// <summary>
    /// Save the current save data to save.dat.
    /// </summary>
    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream file = File.Create(Application.persistentDataPath + "/save.dat"))
        {
            bf.Serialize(file, SaveData.current);
        }
        Debug.Log("DATA SAVED");
    }

    /// <summary>
    /// Load save data from save.dat.
    /// </summary>
    public static void Load()
    {
        if(SaveExists())
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open))
            {
                SaveData.current = (SaveData)bf.Deserialize(file);
            }
            Debug.Log("DATA LOADED");
        }
        else
        {
            SaveData.current = new SaveData();
            Debug.Log("SAVE DATA NOT FOUND! NEW SAVE DATA CREATED!");
        }
    }

    /// <summary>
    /// Check whether save.dat exists.
    /// </summary>
    /// <returns>True if save.dat exists. False otherwise.</returns>
    public static bool SaveExists()
    {
        return File.Exists(Application.persistentDataPath + "/save.dat");
    }
}
