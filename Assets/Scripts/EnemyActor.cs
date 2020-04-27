using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyActor : MonoBehaviour, IActor
{
    [SerializeField] private float stepDistance;
    [SerializeField] private int stepFrames;
    [SerializeField] private float deathTime;
    [SerializeField] private int deathFrames;

    public bool IsAlive => currentHealth > 0;
    public int CurrentHealth => currentHealth;
    public int Attack => fighter.attack;
    public int Defense => fighter.defense;
    public int Speed => fighter.speed;
    public string FighterName => fighter.name;
    public Vector2 StartPosition => startPosition;
    public BattleEffect Effect => effect;
    public BattleManager Manager { set => battleManager = value; }

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

    private enum TurnType { SingleAttack, MultiAttack, SingleHeal, MultiHeal }

    private int currentHealth;
    private const int normalHealAmount = 5;
    private Vector2 startPosition;
    private BattleManager battleManager;
    private BattleEffect effect;
    private Enemy fighter;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        effect = GetComponentInChildren<BattleEffect>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    public int CalculateDamage(IActor target)
    {
        return Mathf.FloorToInt(10 * Attack * (1 - (target.Defense - 1) * 0.2f));
    }

    public void DoDamage(IActor target)
    {
        int damage = CalculateDamage(target);
        target.TakeDamage(damage);
        StartCoroutine(TakeStep());
    }

    public void DoDamage(IActor[] targets)
    {
        foreach (IActor target in targets)
        {
            int damage = Mathf.FloorToInt(CalculateDamage(target) / 2f);
            target.TakeDamage(damage);
        }
        StartCoroutine(TakeStep());
    }

    public void TakeDamage(int damage)
    {
        effect.PlaySlash();
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Die()
    {
        battleManager.PushTurn(new Turn(this, $"{FighterName} was defeated!", () => StartCoroutine(FadeOut())));
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > fighter.maxHealth)
            currentHealth = fighter.maxHealth;
        effect.PlayHeal();
    }

    public IEnumerator TakeStep()
    {
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + Vector2.left * stepDistance;
        float maxDistanceDelta = stepDistance / (stepFrames / 2);

        // Step forward
        while ((Vector2)transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, maxDistanceDelta);
            yield return null;
        }

        // Step back
        while ((Vector2)transform.position != startPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPosition, maxDistanceDelta);
            yield return null;
        }
    }

    /// <summary>
    /// Get a randomly selected turn from the enemy.
    /// </summary>
    /// <param name="bunnyActors">All bunny actors that the enemy can attack.</param>
    /// <param name="enemyActors">All enemy actors that the enemy can heal, including itself.</param>
    public Turn GetTurn(BunnyActor[] bunnyActors, EnemyActor[] enemyActors)
    {
        List<TurnType> availableTurns = new List<TurnType>(4);
        if (fighter.singleAttack)
            availableTurns.Add(TurnType.SingleAttack);
        if (fighter.multiAttack)
            availableTurns.Add(TurnType.MultiAttack);
        if (fighter.singleHeal)
            availableTurns.Add(TurnType.SingleHeal);
        if (fighter.multiAttack)
            availableTurns.Add(TurnType.MultiHeal);

        if (availableTurns.Count == 0)
        {
            Debug.LogError("Enemy has no available turns!");
            return null;
        }

        TurnType selectedTurn = (TurnType)Random.Range(0, availableTurns.Count);

        switch(selectedTurn)
        {
            case TurnType.SingleAttack:
                BunnyActor bunnyActor = bunnyActors[Random.Range(0, bunnyActors.Length)];
                return new Turn(this, bunnyActor, $"{FighterName} attacks {bunnyActor.FighterName}!", () => DoDamage(bunnyActor));
            case TurnType.MultiAttack:
                return new Turn(this, bunnyActors, $"{FighterName} attacks the whole party!", () => DoDamage(bunnyActors));
            case TurnType.SingleHeal:
                return new Turn(this, $"{FighterName} healed itself!", () => Heal(normalHealAmount * 2));
            case TurnType.MultiHeal:
                return new Turn(this, $"{FighterName} healed all enemies!", () =>
                    {
                        foreach (EnemyActor enemyActor in enemyActors)
                            enemyActor.Heal(normalHealAmount);
                    }
                );
            default:
                return null;
        }
    }

    /// <summary>
    /// Fade the enemy's sprite to transparent, and then deactivate the enemy.
    /// </summary>
    private IEnumerator FadeOut()
    {
        int framesComplete = 0;
        float waitTime = deathTime / deathFrames;

        while (framesComplete < deathFrames)
        {
            framesComplete++;

            spriteRenderer.color = new Color(1, 1, 1, 1f - (float)framesComplete / deathFrames);

            yield return new WaitForSeconds(waitTime);
        }

        spriteRenderer.color = new Color(1, 1, 1, 1);
        gameObject.SetActive(false);
    }
}
