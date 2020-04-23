﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyActor : MonoBehaviour, IActor
{
    public int CurrentHealth => currentHealth;
    public int Attack => fighter.attack;
    public int Defense => fighter.defense;
    public int Speed => fighter.speed;
    public string FighterName => fighter.name;

    public Fighter FighterInfo 
    {
        set
        {
            if (value is Bunny)
                fighter = value as Bunny;
        }
    }

    private int currentHealth;
    private Bunny fighter;

    public void DoDamage(IActor target)
    {
        int damage = Mathf.FloorToInt((Attack + Attack * (fighter.level / 100f)) * (1 - (target.Defense - 1) * 0.2f));
        target.TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }
}
