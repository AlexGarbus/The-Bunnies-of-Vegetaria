using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyActor : MonoBehaviour, IActor
    {
        [SerializeField] private float stepDistance;
        [SerializeField] private int stepFrames;
        [SerializeField] private float deathTime;
        [SerializeField] private int deathFrames;

        public bool IsAlive => CurrentHealth > 0;
        public int CurrentHealth { get; private set; }
        public int Attack => fighter.attack;
        public int Defense => fighter.defense;
        public int Speed => fighter.speed;
        public int Experience => Attack + Defense + Speed + Random.Range(7, 10);
        public string FighterName => fighter.name;
        public Vector2 StartPosition => startPosition;
        public BattleEffect Effect { get; private set; }
        public BattleManager Manager { set => battleManager = value; }

        public Fighter FighterData 
        {
            set
            {
                if(value is Enemy)
                {
                    fighter = value as Enemy;
                    spriteRenderer.sprite = Resources.Load<Sprite>($"Sprites/Enemies/{fighter.spriteFileName}");
                    CurrentHealth = 100;
                }
            }
        }

        private enum TurnType { SingleAttack, MultiAttack, SingleHeal, MultiHeal }

        private const int normalHealAmount = 5;
        private Vector2 startPosition;
        private BattleManager battleManager;
        private Enemy fighter;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            Effect = GetComponentInChildren<BattleEffect>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            startPosition = transform.position;
        }

        public int CalculateDamage(IActor target)
        {
            return Mathf.CeilToInt(10 * Attack * (1 - (target.Defense - 1) * 0.2f));
        }

        public void DoDamage(IActor target, float multiplier = 1)
        {
            int damage = CalculateDamage(target) * (int)multiplier;
            target.TakeDamage(damage);
            StartCoroutine(TakeStep());
        }

        public void DoDamage(IActor[] targets, float multiplier = 0.5f)
        {
            foreach (IActor target in targets)
            {
                int damage = Mathf.CeilToInt(CalculateDamage(target) * multiplier);
                target.TakeDamage(damage);
            }
            StartCoroutine(TakeStep());
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive)
                return;

            Effect.PlaySlash();
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Die();
            }
        }

        public void Die()
        {
            battleManager.InsertDeathTurn(this, () => StartCoroutine(FadeOut()));
        }

        public void Heal(int healAmount)
        {
            if (!IsAlive)
                return;

            CurrentHealth += healAmount;
            if (CurrentHealth > fighter.MaxHealth)
                CurrentHealth = fighter.MaxHealth;
            Effect.PlayHeal();
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
            if (fighter.multiHeal)
                availableTurns.Add(TurnType.MultiHeal);

            if (availableTurns.Count == 0)
            {
                Debug.LogError("Enemy has no available turns!");
                return null;
            }

            TurnType selectedTurn = availableTurns[Random.Range(0, availableTurns.Count)];

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
}