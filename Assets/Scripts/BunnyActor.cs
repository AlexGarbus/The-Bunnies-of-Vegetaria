using System;
using System.Collections;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(Animator))]
    public class BunnyActor : MonoBehaviour, IActor
    {
        [SerializeField] private float stepDistance;
        [SerializeField] private float stepSpeed;

        [HideInInspector] public bool isDefending = false;

        public event Action<IActor> OnHealthZero;
        public event Action<IActor> OnDefeat;
        public event Action<BunnyActor> OnLevelUp;

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
        private AudioClip attackSound, healSound, defeatSound;
        private GlobalAudioSource audioSource;
        private Bunny fighter;

        private void Awake()
        {
            Effect = GetComponentInChildren<BattleEffect>();
            animator = GetComponent<Animator>();
            attackSound = Resources.Load<AudioClip>("Sounds/attack");
            healSound = Resources.Load<AudioClip>("Sounds/heal");
            defeatSound = Resources.Load<AudioClip>("Sounds/defeat_bunny");
            audioSource = GlobalAudioSource.Instance;
        }

        private void Start()
        {
            startPosition = transform.position;
        }

        public IEnumerator TakeStep()
        {
            Vector2 startPosition = transform.position;
            Vector2 targetPosition = startPosition + Vector2.right * stepDistance;

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
        /// Gain experience and level up if enough experience has been gained.
        /// </summary>
        /// <param name="experience">The amount of experience to gain.</param>
        public void GainExperience(int experience)
        {
            int previousLevel = fighter.Level;

            fighter.AddExperience(experience);

            if (fighter.Level != previousLevel)
                OnLevelUp?.Invoke(this);
        }
    
        /// <summary>
        /// Set whether this bunny actor should be playing its moving animation.
        /// </summary>
        /// <param name="isMoving"></param>
        public void SetMoving(bool isMoving)
        {
            if (!IsAlive)
                return;

            animator.SetBool("Moving", isMoving);
        }
        
        #region Health

        public int CalculateDamage(IActor target)
        {
            return Mathf.CeilToInt((Attack * 5 + fighter.Level * (1 - Attack * 5f / 100f)) * (1 - (target.Defense - 1) * 0.2f));
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

            if (isDefending)
                damage = Mathf.CeilToInt(damage / 2f);

            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                OnHealthZero?.Invoke(this);
            }
            
            Effect.PlaySlash(damage);
        }

        public void Defeat()
        {
            if (IsAlive)
                return;

            OnDefeat?.Invoke(this);
            transform.Rotate(new Vector3(0, 0, 90));
            audioSource.PlaySoundEffect(defeatSound);
        }

        public void Heal(int healAmount)
        {
            if (!IsAlive)
                return;

            CurrentHealth += healAmount;
            if (CurrentHealth > fighter.MaxHealth)
                CurrentHealth = fighter.MaxHealth;
            
            Effect.PlayHeal(healAmount);
        }
        
        /// <summary>
        /// Reverse the effects of defeat and restore this bunny actor's health.
        /// </summary>
        /// <param name="healthAmount">The amount of health that this bunny actor should have upon revival.</param>
        public void Revive(int healthAmount = 10)
        {
            if (IsAlive)
                return;

            CurrentHealth = Mathf.Clamp(healthAmount, 0, fighter.MaxHealth);
            transform.Rotate(new Vector3(0, 0, -90));
            Effect.PlayHeal(healthAmount);
        }

        #endregion

        #region Skills

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

            if (skill.Target == Skill.TargetType.Bunny)
                audioSource.PlaySoundEffect(healSound);

            CurrentSkillPoints -= skill.Cost;
            skill.Use(this, targets);
        }

        public void RestoreSkillPoints(int skillAmount)
        {
            if (!IsAlive)
                return;

            CurrentSkillPoints += skillAmount;
            if (CurrentSkillPoints > fighter.MaxSkillPoints)
                CurrentSkillPoints = fighter.MaxSkillPoints;
            Effect.PlayHeal(skillAmount);
        }

        #endregion
    }
}