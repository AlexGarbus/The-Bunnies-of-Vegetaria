using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class BunnyActor : MonoBehaviour, IActor
    {
        [SerializeField] private float stepDistance;
        [SerializeField] private int stepFrames;

        [HideInInspector] public bool isDefending = false;

        public bool IsAlive => CurrentHealth > 0;
        public int CurrentHealth { get; private set; }
        public int Attack => fighter.attack;
        public int Defense => fighter.defense;
        public int Speed => fighter.speed;
        public int Experience => fighter.Experience;
        public string FighterName => fighter.name;
        public Vector2 StartPosition => startPosition;
        public BattleEffect Effect { get; private set; }
        public BattleManager Manager { set => battleManager = value; }
        public Fighter FighterInfo 
        {
            set
            {
                if (value is Bunny)
                {
                    fighter = value as Bunny;
                    CurrentHealth = fighter.maxHealth;
                    CurrentSkill = fighter.maxSkill;
                }
            }
        }
        public int CurrentSkill { get; private set; }

        private Vector2 startPosition;
        private BattleManager battleManager;
        private Bunny fighter;

        private void Awake()
        {
            Effect = GetComponentInChildren<BattleEffect>();
        }

        private void Start()
        {
            startPosition = transform.position;
        }

        public int CalculateDamage(IActor target)
        {
            return Mathf.CeilToInt((Attack * 5 + fighter.Level * (1 - Attack * 5f / 100f)) * (1 - (target.Defense - 1) * 0.2f));
        }

        public void DoDamage(IActor target)
        {
            int damage = CalculateDamage(target);
            target.TakeDamage(damage);
            StartCoroutine(TakeStep());
        }

        public void DoDamage(IActor[] targets)
        {
            foreach(IActor target in targets)
            {
                int damage = Mathf.CeilToInt(CalculateDamage(target) / 2f);
                target.TakeDamage(damage);
            }
            StartCoroutine(TakeStep());
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive)
                return;

            if (isDefending)
                damage = Mathf.CeilToInt(damage / 2f);

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
            battleManager.InsertDeathTurn(this, () => transform.Rotate(new Vector3(0, 0, 90)));
        }

        public void Heal(int healAmount)
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

        /// <summary>
        /// Gain experience and level up if enough experience has been gained.
        /// </summary>
        /// <param name="experience">The amount of experience to gain.</param>
        public void GainExperience(int experience)
        {
            int previousLevel = fighter.Level;

            fighter.AddExperience(experience);

            if (fighter.Level != previousLevel)
            {
                battleManager.PushTurn(new Turn(this, $"{FighterName} leveled up!", () =>
                        {
                            CurrentHealth = fighter.maxHealth;
                            CurrentSkill = fighter.maxSkill;
                        }
                    )
                );
            }
        }
    }
}