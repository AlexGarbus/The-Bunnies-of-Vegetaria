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
                currentHealth = 100;
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

    public void DoDamage(IActor target)
    {
        int damage = Mathf.FloorToInt(10 * Attack * (1 - (target.Defense - 1) * 0.2f));
        target.TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Die()
    {
        // TODO: Insert death turn
        Debug.Log("dead");
    }
}
