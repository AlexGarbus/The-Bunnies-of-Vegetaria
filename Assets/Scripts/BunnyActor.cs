using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyActor : MonoBehaviour, IActor
{
    [SerializeField] private float stepDistance;
    [SerializeField] private int stepFrames;

    public int CurrentHealth => currentHealth;
    public int Attack => fighter.attack;
    public int Defense => fighter.defense;
    public int Speed => fighter.speed;
    public string FighterName => fighter.name;
    public Vector2 StartPosition => startPosition;
    public BattleEffect Effect => effect;

    public Fighter FighterInfo 
    {
        set
        {
            if (value is Bunny)
                fighter = value as Bunny;
        }
    }

    private int currentHealth;
    private Vector2 startPosition;
    private Bunny fighter;
    private BattleEffect effect;

    private void Awake()
    {
        effect = GetComponentInChildren<BattleEffect>();
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    public void DoDamage(IActor target)
    {
        int damage = Mathf.FloorToInt((Attack + Attack * (fighter.level / 100f)) * (1 - (target.Defense - 1) * 0.2f));
        target.TakeDamage(damage);
        StartCoroutine(TakeStep());
    }

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator TakeStep()
    {
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + Vector2.right * stepDistance;
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
}
