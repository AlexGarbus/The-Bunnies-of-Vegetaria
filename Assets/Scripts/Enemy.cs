using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy : Fighter
{
    public string spriteFileName, area;
    public bool singleAttack, multiAttack, singleHeal, multiHeal;

    public Enemy()
    {
        maxHealth = 100;
    }
}
