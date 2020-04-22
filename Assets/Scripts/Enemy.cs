// TODO: Use inheritance or interfaces to share values between Enemy and Bunny
// TODO: Replace area string with Area enum
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy
{
    public string name, spriteFileName, area;
    public int attack, defense, speed;
    public bool singleAttack, multiAttack, singleHeal, multiHeal;
}
