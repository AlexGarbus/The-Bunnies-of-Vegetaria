using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyActor : MonoBehaviour, IActor
    {
        [SerializeField] private float stepDistance;
        [SerializeField] private int stepSpeed;
        [SerializeField] private float deathTime;
        [SerializeField] private int deathFrames;

        public bool IsAlive => CurrentHealth > 0;
        public int CurrentHealth { get; private set; }
        public int Attack => fighter.attack;
        public int Defense => fighter.defense;
        public int Speed => fighter.speed;
        public int Experience => Attack + Defense + Speed;
        public string FighterName => fighter.name;
        public Vector2 StartPosition => startPosition;
        public BattleEffect Effect { get; private set; }
        public GameObject Observer { set => observer = value; }
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

        private enum EnemyTurnType { SingleAttack, MultiAttack, SingleHeal, MultiHeal }

        private const int normalHealAmount = 5;
        private Vector2 startPosition;
        private AudioClip attackSound, healSound, defeatSound;
        private GlobalAudioSource audioSource;
        private GameObject observer;
        private Enemy fighter;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            Effect = GetComponentInChildren<BattleEffect>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            attackSound = Resources.Load<AudioClip>("Sounds/attack");
            healSound = Resources.Load<AudioClip>("Sounds/heal");
            defeatSound = Resources.Load<AudioClip>("Sounds/defeat_enemy");
            audioSource = GlobalAudioSource.Instance;
        }

        private void Start()
        {
            startPosition = transform.position;
        }
        
        /// <summary>
        /// Generate a randomly-selected turn for this enemy actor.
        /// </summary>
        /// <param name="bunnyActors">All bunny actors that this enemy can attack.</param>
        /// <param name="enemyActors">All enemy actors that this enemy can heal, including itself.</param>
        public Turn GetTurn(BunnyActor[] bunnyActors, EnemyActor[] enemyActors)
        {
            EnemyTurnType[] availableTurnTypes = GetAvailableTurnTypes();

            if (availableTurnTypes.Length == 0)
            {
                Debug.LogError("Enemy has no available turns!");
                return null;
            }

            EnemyTurnType selectedTurn = availableTurnTypes[Random.Range(0, availableTurnTypes.Length)];

            switch(selectedTurn)
            {
                case EnemyTurnType.SingleAttack:
                    BunnyActor bunnyActor = bunnyActors[Random.Range(0, bunnyActors.Length)];
                    return new Turn(this, bunnyActor, $"{FighterName} attacks {bunnyActor.FighterName}!", () => DoDamage(bunnyActor));
                case EnemyTurnType.MultiAttack:
                    return new Turn(this, bunnyActors, $"{FighterName} attacks the whole party!", () => DoDamage(bunnyActors));
                case EnemyTurnType.SingleHeal:
                    return new Turn(this, this, $"{FighterName} healed itself!", () =>
                        {
                            Heal(normalHealAmount * 2);
                            audioSource.PlaySoundEffect(healSound);
                        }
                    );
                case EnemyTurnType.MultiHeal:
                    return new Turn(this, enemyActors, $"{FighterName} healed all enemies!", () =>
                        {
                            foreach (EnemyActor enemyActor in enemyActors)
                                enemyActor.Heal(normalHealAmount);
                            audioSource.PlaySoundEffect(healSound);
                        }
                    );
                default:
                    return null;
            }
        }

        public IEnumerator TakeStep()
        {
            if (!IsAlive)
                yield break;

            Vector2 startPosition = transform.position;
            Vector2 targetPosition = startPosition + Vector2.left * stepDistance;

            // Step forward
            while ((Vector2)transform.position != targetPosition)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, stepSpeed * Time.deltaTime);
                yield return null;
            }

            // Step back
            while ((Vector2)transform.position != startPosition)
            {
                transform.position = Vector2.MoveTowards(transform.position, startPosition, stepSpeed * Time.deltaTime);
                yield return null;
            }
        }

        /// <summary>
        /// Get an array of turn types that this enemy can use.
        /// </summary>
        /// <returns>An array of all turn types available to this actor.</returns>
        private EnemyTurnType[] GetAvailableTurnTypes()
        {
            List<EnemyTurnType> availableTurns = new List<EnemyTurnType>(4);

            if (fighter.singleAttack)
                availableTurns.Add(EnemyTurnType.SingleAttack);
            if (fighter.multiAttack)
                availableTurns.Add(EnemyTurnType.MultiAttack);
            if (fighter.singleHeal)
                availableTurns.Add(EnemyTurnType.SingleHeal);
            if (fighter.multiHeal)
                availableTurns.Add(EnemyTurnType.MultiHeal);

            return availableTurns.ToArray();
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

        #region Health

        public int CalculateDamage(IActor target)
        {
            return Mathf.CeilToInt(10 * Attack * (1 - (target.Defense - 1) * 0.2f));
        }

        public void DoDamage(IActor target, float multiplier = 1)
        {
            if (!IsAlive)
                return;

            int damage = CalculateDamage(target) * (int)multiplier;
            target.TakeDamage(damage);
            audioSource.PlaySoundEffect(attackSound);
            StartCoroutine(TakeStep());
        }

        public void DoDamage(IActor[] targets, float multiplier = 0.5f)
        {
            if (!IsAlive)
                return;

            foreach (IActor target in targets)
            {
                int damage = Mathf.CeilToInt(CalculateDamage(target) * multiplier);
                target.TakeDamage(damage);
            }
            audioSource.PlaySoundEffect(attackSound);
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
                observer.SendMessage("EnemyDefeat", this);
            }
        }

        public void Defeat()
        {
            if (IsAlive)
                return;

            audioSource.PlaySoundEffect(defeatSound);
            StartCoroutine(FadeOut());
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

        #endregion
    }
}