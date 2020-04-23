using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyActor : MonoBehaviour, IActor
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
            if(value is Enemy)
            {
                fighter = value as Enemy;
                spriteRenderer.sprite = Resources.Load<Sprite>($"Sprites/Enemies/{fighter.spriteFileName}");
            }
        }
    }

    private int currentHealth;
    private Enemy fighter;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AttackDamage()
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }
}
