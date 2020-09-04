using System;
using System.Collections;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public abstract class Actor<T> : MonoBehaviour where T : Fighter
    {
        [SerializeField] private float stepDistance;
        [SerializeField] private float stepSpeed;

        public Vector2 StartPosition { get; protected set; }
        public T Fighter 
        {
            get => fighter;
            set
            {
                // Unsubscribe from previous fighter events
                if (fighter != null)
                {
                    fighter.OnHealthChange -= Fighter_OnHealthChange;
                    fighter.OnDoDamage -= Fighter_OnDoDamage;
                }

                fighter = value;

                // Subscribe to events
                fighter.OnHealthChange += Fighter_OnHealthChange;
                fighter.OnDoDamage += Fighter_OnDoDamage;

                // Initialize fighter and actor
                fighter.Initialize();
                Initialize();
            }
        }

        protected abstract Vector2 StepDirection { get; }
        protected BattleEffect battleEffect;

        private T fighter;

        private void Fighter_OnDoDamage(object sender, EventArgs e)
        {
            TakeStep(StepDirection);
        }

        private void Fighter_OnHealthChange(object sender, PointEventArgs e)
        {
            battleEffect.PlayHealthEffect(e.DeltaPoints);
        }

        private void Start()
        {
            StartPosition = transform.position;
            battleEffect = GetComponentInChildren<BattleEffect>();
        }

        protected abstract void Initialize();

        protected virtual void OnDestroy()
        {
            if (fighter != null)
            {
                fighter.OnHealthChange -= Fighter_OnHealthChange;
                fighter.OnDoDamage -= Fighter_OnDoDamage;
            }
        }

        /// <summary>
        /// Perform this actor's defeat behavior.
        /// </summary>
        public abstract void Defeat();

        /// <summary>
        /// Quickly step forward and back in a desired direction.
        /// </summary>
        /// <param name="direction">The direction to step in.</param>
        protected IEnumerator TakeStep(Vector2 direction)
        {
            Vector2 startPosition = transform.position;
            Vector2 targetPosition = startPosition + direction.normalized * stepDistance;

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
    }
}