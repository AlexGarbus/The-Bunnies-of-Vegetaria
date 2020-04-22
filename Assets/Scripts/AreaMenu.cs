using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// TODO: Move Area to separate file/class/namespace
public enum Area { LettuceFields = 1, CeleryWoods, BroccoliForest, BokChoyBluff, CarrotTop }

public class AreaMenu : MonoBehaviour
{
    [Header("Map")]
    [Tooltip("Buttons should be in ascending order by area.")]
    [SerializeField] private AreaButton[] areaButtons;

    [Header("Party")]
    [SerializeField] private TMP_Text statText;

    private const string spacer = "  ";
    private Bunny bunnight;
    private Bunny bunnecromancer;
    private Bunny bunnurse;
    private Bunny bunneerdowell;

    private void Start()
    {
        // Set up map buttons
        for (int i = 0; i < areaButtons.Length; i++)
        {
            if (i + 1 > SaveData.current.areasUnlocked)
                areaButtons[i].SetLocked();
        }

        // TODO: Store in Game Manager instead
        SaveData save = SaveData.current;
        bunnight = new Bunny(BunnyType.Bunnight, save.bunnightName, save.bunnightExp);
        bunnecromancer = new Bunny(BunnyType.Bunnecromancer, save.bunnecromancerName, save.bunnecromancerExp);
        bunnurse = new Bunny(BunnyType.Bunnurse, save.bunnurseName, save.bunnurseExp);
        bunneerdowell = new Bunny(BunnyType.Bunneerdowell, save.bunneerdowellName, save.bunneerdowellExp);
    }

    /// <summary>
    /// Load the selected area if it is unlocked.
    /// </summary>
    /// <param name="areaName">The name of the area to load.</param>
    public void LoadArea(string areaName)
    {
        Area area;
        areaName = areaName.Replace(" ", string.Empty);

        if (Enum.TryParse(areaName, false, out area) && (int)area <= SaveData.current.areasUnlocked)
        {
            // TODO: Load area
        }
    }

    public void DisplayBunnyStats(int typeIndex)
    {
        BunnyType type = (BunnyType)typeIndex;
        Bunny bunny = null;
        string typeString = "";

        switch (type)
        {
            case BunnyType.Bunnight:
                bunny = bunnight;
                typeString = "BUNNIGHT";
                break;
            case BunnyType.Bunnecromancer:
                bunny = bunnecromancer;
                typeString = "BUNNECROMANCER";
                break;
            case BunnyType.Bunnurse:
                bunny = bunnurse;
                typeString = "BUNNURSE";
                break;
            case BunnyType.Bunneerdowell:
                bunny = bunneerdowell;
                typeString = "BUNNE'ER-DO-WELL";
                break;
        }

        string stats = $"{bunny.Name} the {typeString}" + "\n\n"
            + $"LEVEL: {bunny.Level}" + spacer + $"EXPERIENCE: {bunny.Experience}" + "\n\n"
            + $"HEALTH: {bunny.MaxHealth}" + spacer + $"SKILL: {bunny.MaxSkill}" + "\n\n"
            + $"ATTACK: {new string('*', bunny.Attack)}" + spacer 
            + $"DEFENSE: {new string('*', bunny.Defense)}" + spacer
            + $"SPEED: {new string('*', bunny.Speed)}";
        statText.text = stats;
    }
}
