using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheBunniesOfVegetaria
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private AudioClip bossMusic;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private BattleMenu battleMenu;
        [SerializeField] private SceneTransition sceneTransition;

        [Header("Background")]
        [SerializeField] private BattleBackground battleBackground;
        // TODO: Maybe load from JSON instead?
        [Tooltip("The backgrounds for each area. These should be in ascending order by area.")]
        [SerializeField] private Sprite[] backgrounds;

        [Header("Battle Flow")]
        [SerializeField] private int maxWaves = 5;
        [SerializeField] private float turnTime = 1f;
        [SerializeField] private float travelSpeed = 1f;

        [Header("Actors")]
        [SerializeField] private BunnyActor[] bunnyActors;
        [SerializeField] private EnemyActor[] enemyActors;

        private enum BattleState { Waiting, SettingUpInput, HandlingInput, SettingUpTurns, HandlingTurns }

        private int inputsReceived = -1;
        private int wave = 1;
        private BattleState battleState = BattleState.SettingUpInput;
        private GameManager gameManager;
        private Enemy[] enemies;
        private Enemy boss;
        private TurnList turnList = new TurnList(8);

        private void Start()
        {
            gameManager = GameManager.Instance;

            // Get area
            Globals.Area area;
            if (gameManager.AreaIndex == 0)
                area = Globals.Area.LettuceFields;
            else
                area = (Globals.Area)gameManager.AreaIndex;

            // Load enemies
            EnemyRepository enemyRepository = EnemyRepository.LoadFromJSON();
            enemies = enemyRepository.GetEnemiesFromArea(area).ToArray();
            boss = enemyRepository.GetBossFromArea(area);

            // TODO: Set background

            // Set up bunny actors
            foreach (BunnyActor actor in bunnyActors)
                actor.Manager = this;
            bunnyActors[(int)Globals.BunnyType.Bunnight].FighterInfo = gameManager.Bunnight;
            bunnyActors[(int)Globals.BunnyType.Bunnecromancer].FighterInfo = gameManager.Bunnecromancer;
            bunnyActors[(int)Globals.BunnyType.Bunnurse].FighterInfo = gameManager.Bunnurse;
            bunnyActors[(int)Globals.BunnyType.Bunneerdowell].FighterInfo = gameManager.Bunneerdowell;
            battleMenu.SetPlayerStatText(bunnyActors);

            // Set up enemy actors
            foreach (EnemyActor actor in enemyActors)
                actor.Manager = this;
            SetWave();
        }

        private void Update()
        {
            switch(battleState)
            {
                case BattleState.SettingUpInput:
                    NextInput();
                    battleState = BattleState.HandlingInput;
                    break;
                case BattleState.HandlingInput:
                    if(inputsReceived >= bunnyActors.Length)
                    {
                        inputsReceived = -1;
                        battleState = BattleState.SettingUpTurns;
                    }
                    break;
                case BattleState.SettingUpTurns:
                    battleMenu.ShowPlayerInputPanel(false);
                    battleMenu.ShowTurnPanel(true);
                    InsertEnemyTurns();
                    StartCoroutine(HandleTurns());
                    battleState = BattleState.HandlingTurns;
                    break;
                case BattleState.HandlingTurns:
                    break;
            }
        }

        /// <summary>
        /// Get all bunny actors with health greater than 0.
        /// </summary>
        /// <returns>An array with all alive bunny actors.</returns>
        public BunnyActor[] GetAliveBunnies()
        {
            List<BunnyActor> aliveActors = new List<BunnyActor>();
            foreach(BunnyActor actor in bunnyActors)
            {
                if (actor.IsAlive)
                    aliveActors.Add(actor);
            }
            return aliveActors.ToArray();
        }

        /// <summary>
        /// Get all enemy actors with health greater than 0.
        /// </summary>
        /// <returns>An array with all alive enemy actors.</returns>
        public EnemyActor[] GetAliveEnemies()
        {
            List<EnemyActor> aliveActors = new List<EnemyActor>();
            foreach (EnemyActor actor in enemyActors)
            {
                if (actor.IsAlive)
                    aliveActors.Add(actor);
            }
            return aliveActors.ToArray();
        }

        /// <summary>
        /// Add experience to all living bunnies.
        /// </summary>
        /// <param name="experience">The amount of experience to give.</param>
        private void GainExperience(int experience)
        {
            BunnyActor[] aliveBunnies = GetAliveBunnies();
            foreach(BunnyActor bunnyActor in aliveBunnies)
                bunnyActor.GainExperience(experience);
        }

        /// <summary>
        /// Count the input for the current bunny as received and prepare to either receive the next input or begin handling turns.
        /// </summary>
        private void NextInput()
        {
            do
            {
                inputsReceived++;
            }
            while (inputsReceived < bunnyActors.Length && !bunnyActors[inputsReceived].IsAlive);

            if(inputsReceived < bunnyActors.Length)
                PromptInput();
        }

        /// <summary>
        /// Prompt the user to input their turn for the current bunny.
        /// </summary>
        private void PromptInput()
        {
            battleMenu.ShowPlayerInputPanel(true);
            battleMenu.ShowOptionPanel(true);
            battleMenu.ShowTurnPanel(false);
            battleMenu.SetEnemyButtons(enemyActors);
            battleMenu.SetInputPromptText($"What will {bunnyActors[inputsReceived].FighterName} do?");
        }

        /// <summary>
        /// Set the current wave of enemies.
        /// </summary>
        private void SetWave()
        {
            if(wave == maxWaves)
            {
                // Boss wave
                enemyActors[0].gameObject.SetActive(true);
                enemyActors[0].FighterInfo = boss;
                musicSource.clip = bossMusic;
                musicSource.Play();
            }
            else
            {
                // Regular enemy wave
                int i;
                for(i = 0; i < wave && i < enemyActors.Length; i++)
                {
                    int enemyIndex = UnityEngine.Random.Range(0, enemies.Length);
                    enemyActors[i].gameObject.SetActive(true);
                    enemyActors[i].FighterInfo = enemies[enemyIndex];
                }
            
                for(int j = i; j < enemyActors.Length; j++)
                {
                    if (enemyActors[j].gameObject.activeSelf)
                        enemyActors[j].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Perform each turn in the turn list.
        /// </summary>
        private IEnumerator HandleTurns()
        {
            while(!turnList.IsEmpty)
            {
                Turn turn = turnList.Pop();

                // Perform turn
                turn.TurnAction?.Invoke();
                battleMenu.SetTurnText(turn.Message);
                battleMenu.SetPlayerStatText(bunnyActors);

                yield return new WaitForSeconds(turnTime);
            
                if(GetAliveEnemies().Length == 0)
                {
                    // Bunnies have won
                    if(turnList.IsEmpty)
                    {
                        wave++;
                        SetWave();
                        StartCoroutine(battleBackground.ScrollBackground(1, travelSpeed));
                    }
                }
            }

            if(GetAliveBunnies().Length == 0)
                battleState = BattleState.Waiting;
            else
                battleState = BattleState.SettingUpInput;
        }
    
        #region Turn Insertion

        /// <summary>
        /// Insert a turn into the turn list, making it the next turn that the Battle Manager handles.
        /// </summary>
        /// <param name="turn">The turn that the Battle Manager should handle next.</param>
        public void PushTurn(Turn turn)
        {
            turnList.Push(turn);
        }

        /// <summary>
        /// Insert a standard attack turn into the turn list.
        /// </summary>
        public void InsertAttackTurn(GameObject targetObject)
        {
            IActor user = bunnyActors[inputsReceived];
            IActor target = targetObject.GetComponent<IActor>();
            Turn turn = new Turn(user, target, $"{user.FighterName} attacks {target.FighterName}!", () => user.DoDamage(target));
            turnList.Insert(turn);
            NextInput();
        }

        /// <summary>
        /// Insert a defend turn into the turn list.
        /// </summary>
        public void InsertDefendTurn()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Insert a skill turn into the turn list.
        /// </summary>
        public void InsertSkillTurn()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Insert a death turn for a bunny actor and lose the battle if all bunnies are dead.
        /// </summary>
        /// <param name="bunnyActor">The bunny that has died.</param>
        /// <param name="deathAction">The action that the bunny should perform upon death.</param>
        public void InsertDeathTurn(BunnyActor bunnyActor, Action deathAction)
        {
            turnList.RemoveUserTurns(bunnyActor);
            turnList.RemoveTargetTurns(bunnyActor);
            turnList.Push(new Turn(bunnyActor, $"{bunnyActor.FighterName} was defeated!", deathAction));

            if(GetAliveBunnies().Length == 0)
            {
                turnList.RemoveEnemyTurns();
                turnList.Append(new Turn(bunnyActor, "The bunnies have lost! Retreat!", () => sceneTransition.LoadScene(3)));
            }
        }

        /// <summary>
        /// Insert a death turn for an enemy actor.
        /// </summary>
        /// <param name="enemyActor">The enemy that has died.</param>
        /// <param name="deathAction">The action that the bunny should perform upon death.</param>
        public void InsertDeathTurn(EnemyActor enemyActor, Action deathAction)
        {
            turnList.RemoveUserTurns(enemyActor);
            turnList.RemoveTargetTurns(enemyActor);

            GainExperience(enemyActor.Experience);

            turnList.Push(new Turn(enemyActor, $"{enemyActor.FighterName} was defeated!", deathAction));

            // TODO: Start new wave here
        }

        /// <summary>
        /// Insert turns for each enemy into the turn list.
        /// </summary>
        private void InsertEnemyTurns()
        {
            BunnyActor[] aliveBunnies = GetAliveBunnies();
            EnemyActor[] aliveEnemies = GetAliveEnemies();
            foreach (EnemyActor enemyActor in aliveEnemies)
            {
                Turn turn = enemyActor.GetTurn(aliveBunnies, aliveEnemies);
                if (turn != null)
                    turnList.Insert(turn);
            }
        }

        #endregion
    }
}