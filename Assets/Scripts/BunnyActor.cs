using System.Collections;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(Animator))]
    public class BunnyActor : MonoBehaviour, IActor
    {
        [SerializeField] private float stepDistance;
        [SerializeField] private int stepFrames;

        [HideInInspector] public bool isDefending = false;

        public bool IsAlive => CurrentHealth > 0;
        public int Attack => fighter.attack;
        public int Defense => fighter.defense;
        public int Speed => fighter.speed;
        public int Experience => fighter.Experience;
        public int CurrentHealth { get; private set; }
        public int CurrentSkillPoints { get; private set; }
        public string FighterName => fighter.name;
        public Vector2 StartPosition => startPosition;
        public BattleEffect Effect { get; private set; }
        // TODO: Try to remove references to this.
        public BattleManager Manager { set => battleManager = value; }
        public Fighter FighterData 
        {
            set
            {
                if (value is Bunny)
                {
                    fighter = value as Bunny;
                    CurrentHealth = fighter.MaxHealth;
                    CurrentSkillPoints = fighter.MaxSkillPoints;
                }
            }
        }
        public string[] AvailableSkillStrings => fighter.GetAvailableSkillStrings();
        
        private Vector2 startPosition;
        private Animator animator;
        private BattleManager battleManager;
        private Bunny fighter;

        private void Awake()
        {
            Effect = GetComponentInChildren<BattleEffect>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            startPosition = transform.position;
        }

        public int CalculateDamage(IActor target)
        {
            return Mathf.CeilToInt((Attack * 5 + fighter.Level * (1 - Attack * 5f / 100f)) * (1 - (target.Defense - 1) * 0.2f));
        }

        public void DoDamage(IActor target, float multiplier = 1)
        {
            int damage = CalculateDamage(target) * (int)multiplier;
            target.TakeDamage(damage);
            StartCoroutine(TakeStep());
        }

        public void DoDamage(IActor[] targets, float multiplier = 0.5f)
        {
            foreach(IActor target in targets)
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

        public bool CanUseSkill(int skillIndex)
        {
            Skill skill = fighter.Skills[skillIndex];
            return CurrentSkillPoints >= skill.Cost && fighter.Level >= skill.MinimumLevel;
        }

        public string GetSkillName(int skillIndex)
        {
            return fighter.Skills[skillIndex].Name;
        }

        public Skill.TargetType GetSkillTarget(int skillIndex)
        {
            return fighter.Skills[skillIndex].Target;
        }

        /// <summary>
        /// Use a skill.
        /// </summary>
        /// <param name="skillIndex">The index of the skill to use.</param>
        /// <param name="targets">The target actors of this skill.</param>
        public void UseSkill(int skillIndex, IActor[] targets)
        {
            if (!CanUseSkill(skillIndex))
                return;

            Skill skill = fighter.Skills[skillIndex];

            CurrentSkillPoints -= skill.Cost;
            skill.Use(this, targets);
        }

        public void RestoreSkill(int skillAmount)
        {
            if (!IsAlive)
                return;

            CurrentSkillPoints += skillAmount;
            if (CurrentSkillPoints > fighter.MaxSkillPoints)
                CurrentSkillPoints = fighter.MaxSkillPoints;
            Effect.PlayHeal();
        }
        
        public void Revive(int healthAmount = 10)
        {
            if (IsAlive)
                return;

            CurrentHealth = Mathf.Clamp(healthAmount, 0, fighter.MaxHealth);
            transform.Rotate(new Vector3(0, 0, -90));
            Effect.PlayHeal();
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
                            CurrentHealth = fighter.MaxHealth;
                            CurrentSkillPoints = fighter.MaxSkillPoints;
                        }
                    )
                );
            }
        }
    
        public void SetMoving(bool isMoving)
        {
            if (!IsAlive)
                return;

            animator.SetBool("Moving", isMoving);
        }
    }
}