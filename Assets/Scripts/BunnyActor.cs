using System;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(Animator))]
    public class BunnyActor : Actor<Bunny>
    {
        protected override Vector2 StepDirection { get => Vector2.right; }

        private Animator animator;
        // TODO: Move audio playing to separate class
        private AudioClip attackSound, healSound, defeatSound;
        private GlobalAudioSource audioSource;

        private void OnEnable()
        {
            BattleBackground.OnScrollStart += BattleBackground_OnScrollStart;
            BattleBackground.OnScrollComplete += BattleBackground_OnScrollComplete;
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            attackSound = Resources.Load<AudioClip>("Sounds/attack");
            healSound = Resources.Load<AudioClip>("Sounds/heal");
            defeatSound = Resources.Load<AudioClip>("Sounds/defeat_bunny");
            audioSource = GlobalAudioSource.Instance;
        }

        protected override void Initialize()
        {
            if (Fighter != null)
            {
                Fighter.OnHealthChange += Fighter_OnHealthChange;
                Fighter.OnSkillPointsChange += Fighter_OnSkillPointsChange;
            }
        }

        private void OnDisable()
        {
            BattleBackground.OnScrollStart -= BattleBackground_OnScrollStart;
            BattleBackground.OnScrollComplete -= BattleBackground_OnScrollComplete;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Fighter != null)
            {
                Fighter.OnHealthChange -= Fighter_OnHealthChange;
                Fighter.OnSkillPointsChange -= Fighter_OnSkillPointsChange;
            }
        }

        private void BattleBackground_OnScrollStart(object sender, EventArgs e) => animator.SetBool("Moving", true);

        private void BattleBackground_OnScrollComplete(object sender, EventArgs e) => animator.SetBool("Moving", false);

        private void Fighter_OnHealthChange(object sender, PointEventArgs e)
        {
            if (e.PreviousPoints == 0)
            {
                // Reset defeat rotation
                transform.Rotate(new Vector3(0, 0, -90));
            }
        }

        private void Fighter_OnSkillPointsChange(object sender, PointEventArgs e) => battleEffect.PlayHealthEffect(e.DeltaPoints);
        
        public override void Defeat()
        {
            transform.Rotate(new Vector3(0, 0, 90));
            audioSource.PlaySoundEffect(defeatSound);
        }
    }
}