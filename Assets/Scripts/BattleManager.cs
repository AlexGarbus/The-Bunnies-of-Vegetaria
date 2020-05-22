using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private AudioClip bossMusic;
        [SerializeField] private BattleBackground battleBackground;

        [Header("Actors")]
        [Tooltip("The bunny objects in the scene. These should be ordered according to the BunnyType enum.")]
        [SerializeField] private BunnyActor[] bunnyActors;
        [Tooltip("The enemyobjects in the scene. These should be in ascending order by spawn position.")]
        [SerializeField] private EnemyActor[] enemyActors;

        [Header("Enemy Positioning")]
        [Tooltip("The start point of the line that enemies are positioned along.")]
        [SerializeField] private Vector2 enemySpawnPointA;
        [Tooltip("The end point of the line that enemies are positioned along.")]
        [SerializeField] private Vector2 enemySpawnPointB;

        [Header("Battle Flow")]
        [SerializeField] private int maxWaves = 5;
        [SerializeField] private float turnTime = 1f;
        [SerializeField] private float travelSpeed = 1f;

        [Header("User Interface")]
        [SerializeField] private BattleMenu battleMenu;
        [SerializeField] private SceneTransition sceneTransition;

        private void OnDrawGizmosSelected()
        {
            // Draw enemy spawn line
            Gizmos.color = Color.red;
            Gizmos.DrawLine(enemySpawnPointA, enemySpawnPointB);
        }

        private BunnyActor SelectedBunny 
        {
            get 
            {
                if (inputsReceived >= 0 && inputsReceived < bunnyActors.Length)
                {
                    return bunnyActors[inputsReceived];
                }
                else
                {
                    Debug.LogWarning("Attempting to get selected bunny when none is selected.");
                    return null;
                }
            }
        }

        private enum BattleState { Idle, Traveling, SettingUpInput, HandlingInput, SettingUpTurns, HandlingTurns, DoneHandlingTurns }

        private int inputsReceived = -1;
        private int wave = 0;
        private BattleState battleState;
        private AudioClip healSound;
        private GameManager gameManager;
        private Enemy[] enemies;
        private Enemy boss;
        private TurnList turnList = new TurnList(8);

        private void Start()
        {
            healSound = Resources.Load<AudioClip>("Sounds/heal");
            gameManager = GameManager.Instance;

            // Get area
            Globals.Area area = gameManager.BattleArea;

            // Load enemies
            EnemyRepository enemyRepository = EnemyRepository.LoadFromJSON();
            enemies = enemyRepository.GetEnemiesFromArea(area).ToArray();
            boss = enemyRepository.GetBossFromArea(area);

            // Initialize bunny actors
            bunnyActors[(int)Globals.BunnyType.Bunnight].FighterData = gameManager.Bunnight;
            bunnyActors[(int)Globals.BunnyType.Bunnecromancer].FighterData = gameManager.Bunnecromancer;
            bunnyActors[(int)Globals.BunnyType.Bunnurse].FighterData = gameManager.Bunnurse;
            bunnyActors[(int)Globals.BunnyType.Bunneerdowell].FighterData = gameManager.Bunneerdowell;
            battleMenu.SetPlayerStatText(bunnyActors);

            // Subscribe to bunny actor events
            foreach (BunnyActor bunnyActor in bunnyActors)
            {
                bunnyActor.OnHealthZero += InsertDefeatTurn;
                bunnyActor.OnDefeat += OnDefeatTurn;
                bunnyActor.OnLevelUp += InsertLevelUpTurn;
            }

            // Subscribe to enemy actor events
            foreach (EnemyActor enemyActor in enemyActors)
            {
                enemyActor.OnHealthZero += InsertDefeatTurn;
                enemyActor.OnDefeat += OnDefeatTurn;
            }

            SetNextWave();
            StartTravel();
        }

        private void OnDisable()
        {
            // Unsubscribe from bunny actor events
            foreach (BunnyActor bunnyActor in bunnyActors)
            {
                bunnyActor.OnHealthZero -= InsertDefeatTurn;
                bunnyActor.OnDefeat -= OnDefeatTurn;
                bunnyActor.OnLevelUp -= InsertLevelUpTurn;
            }

            // Unsubscribe from enemy actor events
            foreach (EnemyActor enemyActor in enemyActors)
            {
                enemyActor.OnHealthZero -= InsertDefeatTurn;
                enemyActor.OnDefeat -= OnDefeatTurn;
            }
        }

        private void Update()
        {
            switch(battleState)
            {
                case BattleState.Idle:
                    break;
                case BattleState.Traveling:
                    // Wait for background to finish scrolling
                    if (!battleBackground.IsScrolling)
                    {
                        foreach (BunnyActor bunnyActor in bunnyActors)
                            bunnyActor.SetMoving(false);
                        battleMenu.ShowPlayerStatPanel(true);
                        battleState = BattleState.SettingUpInput;
                    }
                    break;
                case BattleState.SettingUpInput:
                    // Prepare to receive input for the next bunny
                    NextInput();
                    battleState = BattleState.HandlingInput;
                    break;
                case BattleState.HandlingInput:
                    if(inputsReceived >= bunnyActors.Length)
                    {
                        // Input for all bunnies received
                        inputsReceived = -1;
                        battleState = BattleState.SettingUpTurns;
                    }
                    break;
                case BattleState.SettingUpTurns:
                    // Prepare to handle turns
                    InsertEnemyTurns();
                    battleMenu.ShowPlayerInputPanel(false);
                    battleMenu.ShowTurnPanel(true);
                    StartCoroutine(HandleTurns());
                    battleState = BattleState.HandlingTurns;
                    break;
                case BattleState.HandlingTurns:
                    // Coroutine is running
                    break;
                case BattleState.DoneHandlingTurns:
                    if (GetAliveBunnies().Length == 0)
                    {
                        // Battle is lost
                        battleState = BattleState.Idle;
                    }
                    else if (GetAliveEnemies().Length == 0)
                    {
                        // Battle is won
                        FinishWave();
                    }
                    else
                    {
                        // Battle is undecided
                        foreach (BunnyActor bunnyActor in bunnyActors)
                            bunnyActor.isDefending = false;
                        battleState = BattleState.SettingUpInput;
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Perform each turn in the turn list.
        /// </summary>
        private IEnumerator HandleTurns()
        {
            WaitForSeconds turnDelay = new WaitForSeconds(turnTime);

            while(!turnList.IsEmpty)
            {
                // Perform turn
                Turn turn = turnList.Pop();
                turn.TurnAction?.Invoke();

                // Display turn in menu
                battleMenu.SetTurnText(turn.Message);
                battleMenu.SetPlayerStatText(bunnyActors);

                yield return turnDelay;
            }

            battleState = BattleState.DoneHandlingTurns;
        }

        #region Action Turns

        /// <summary>
        /// Insert a standard bunny attack turn into the turn list.
        /// </summary>
        public void InsertBunnyAttackTurn(GameObject targetObject)
        {
            // Create and insert turn
            IActor user = SelectedBunny;
            IActor target = targetObject.GetComponent<IActor>();
            Turn turn = new Turn(user, target, $"{user.FighterName} attacks {target.FighterName}!", () => user.DoDamage(target));
            turnList.Insert(turn);

            // Prepare for next input
            battleMenu.ShowEnemyPanel(false);
            battleState = BattleState.SettingUpInput;
        }

        /// <summary>
        /// Insert a bunny defend turn into the turn list.
        /// </summary>
        public void InsertBunnyDefendTurn()
        {
            SelectedBunny.isDefending = true;
            
            // Create and insert turn
            IActor user = SelectedBunny;
            Turn turn = new Turn(user, user, $"{user.FighterName} is defending!", null);
            turnList.Insert(turn);

            // Prepare for next input
            battleState = BattleState.SettingUpInput;
        }

        /// <summary>
        /// Insert a bunny skill turn into the turn list.
        /// </summary>
        public void InsertBunnySkillTurn(int skillIndex)
        {
            BunnyActor user = SelectedBunny;

            // Make sure bunny can use the selected skill
            if (!user.CanUseSkill(skillIndex))
                return;

            // Get turn targets
            IActor[] targets;
            if (user.GetSkillTarget(skillIndex) == Skill.TargetType.Bunny)
                targets = bunnyActors;
            else
                targets = GetAliveEnemies();

            // Create and insert turn
            Turn turn = new Turn(user, targets, $"{user.FighterName} used {user.GetSkillName(skillIndex)}!", () => user.UseSkill(skillIndex, targets));
            turnList.Insert(turn);

            // Prepare for next input
            battleMenu.ShowSkillPanel(false);
            battleState = BattleState.SettingUpInput;
        }

        /// <summary>
        /// Insert turns for each enemy into the turn list.
        /// </summary>
        private void InsertEnemyTurns()
        {
            BunnyActor[] aliveBunnies = GetAliveBunnies();
            EnemyActor[] aliveEnemies = GetAliveEnemies();

            // Insert turns
            foreach (EnemyActor enemyActor in aliveEnemies)
            {
                Turn turn = enemyActor.GetTurn(aliveBunnies, aliveEnemies);
                turnList.Insert(turn);
            }
        }

        #endregion

        #region Status Turns

        /// <summary>
        /// Insert a defeat turn for an actor.
        /// </summary>
        /// <param name="actor">The actor to defeat.</param>
        private void InsertDefeatTurn(IActor actor)
        {
            if (actor is BunnyActor)
                InsertBunnyDefeatTurn(actor as BunnyActor);
            else if (actor is EnemyActor)
                InsertEnemyDefeatTurn(actor as EnemyActor);
            else
                Debug.LogError("Unrecognized actor detected!");
        }

        /// <summary>
        /// Insert a defeat turn for a bunny actor and lose the battle if all bunnies are defeated.
        /// </summary>
        /// <param name="bunnyActor">The bunny actor to defeat.</param>
        private void InsertBunnyDefeatTurn(BunnyActor bunnyActor)
        {
            // Remove actor turns
            turnList.RemoveUserTurns(bunnyActor);
            turnList.RemoveTargetTurns(bunnyActor);

            // Create and push defeat turn
            Turn turn = new Turn(bunnyActor, $"{bunnyActor.FighterName} was defeated!", () => bunnyActor.Defeat());
            turnList.Push(turn);
        }

        /// <summary>
        /// Insert a defeat turn for an enemy actor.
        /// </summary>
        /// <param name="enemyActor">The enemy actor to defeat.</param>
        private void InsertEnemyDefeatTurn(EnemyActor enemyActor)
        {
            GainExperience(enemyActor.Experience);
            
            // Remove actor turns
            turnList.RemoveUserTurns(enemyActor);
            turnList.RemoveTargetTurns(enemyActor);

            // Create and push defeat turn
            Turn turn = new Turn(enemyActor, $"{enemyActor.FighterName} was defeated!", () => enemyActor.Defeat());
            turnList.Push(turn);
        }

        /// <summary>
        /// Check whether the player or enemy party has won when an actor is defeated.
        /// </summary>
        /// <param name="actor">The actor that has been defeated.</param>
        private void OnDefeatTurn(IActor actor)
        {
            if (GetAliveBunnies().Length == 0)
            {
                // Bunnies have lost
                turnList.RemoveEnemyTurns();
                Turn turn = new Turn(actor, "The bunnies have lost! Retreat!", () => sceneTransition.SaveAndLoadScene("AreaSelect"));
                turnList.Append(turn);
            }
            else if (GetAliveEnemies().Length == 0)
            {
                // Enemies have lost
                turnList.RemoveNonemptyTargetTurns();

                if (wave == maxWaves)
                {
                    // Boss has been defeated
                    Turn turn = new Turn(actor, "This area is clear!", () =>
                        {
                            // Unlock next area
                            int areasUnlocked = SaveData.current.areasUnlocked;
                            if (areasUnlocked == (int)gameManager.BattleArea
                                && areasUnlocked < (int)Globals.Area.CarrotTop)
                            {
                                SaveData.current.areasUnlocked++;
                            }

                            sceneTransition.SaveAndLoadScene("AreaSelect");
                        }
                    );
                    turnList.Append(turn);
                }
            }
        }

        /// <summary>
        /// Insert a level up turn for a bunny actor.
        /// </summary>
        /// <param name="bunnyActor">The bunny actor that has leveled up.</param>
        private void InsertLevelUpTurn(BunnyActor bunnyActor)
        {
            Turn turn = new Turn(bunnyActor, $"{bunnyActor.FighterName} leveled up!", () =>
                {
                    bunnyActor.Heal(100);
                    bunnyActor.RestoreSkillPoints(100);
                    GlobalAudioSource.Instance.PlaySoundEffect(healSound);
                }
            );
            turnList.Push(turn);
        }

        #endregion

        #region Actor Management

        /// <summary>
        /// Get all bunny actors with health greater than 0.
        /// </summary>
        /// <returns>An array with all bunny actors considered as alive.</returns>
        private BunnyActor[] GetAliveBunnies()
        {
            return bunnyActors.Where(actor => actor.IsAlive).ToArray();
        }

        /// <summary>
        /// Get all bunny actors with health equal to 0.
        /// </summary>
        /// <returns>An array with all bunny actors considered as defeated.</returns>
        private BunnyActor[] GetDefeatedBunnies()
        {
            return bunnyActors.Where(actor => !actor.IsAlive).ToArray();
        }

        /// <summary>
        /// Get all enemy actors with health greater than 0.
        /// </summary>
        /// <returns>An array with all enemy actors considered as alive.</returns>
        private EnemyActor[] GetAliveEnemies()
        {
            return enemyActors.Where(actor => actor.IsAlive).ToArray();
        }

        /// <summary>
        /// Get all enemy actors with health equal to 0.
        /// </summary>
        /// <returns>An array with all enemy actors considered as defeated.</returns>
        private EnemyActor[] GetDefeatedEnemies()
        {
            return enemyActors.Where(actor => !actor.IsAlive).ToArray();
        }

        /// <summary>
        /// Add experience to all living bunnies.
        /// </summary>
        /// <param name="experience">The amount of experience to give.</param>
        private void GainExperience(int experience)
        {
            foreach(BunnyActor bunnyActor in GetAliveBunnies())
                bunnyActor.GainExperience(experience);
        }

        /// <summary>
        /// Finish the current wave of enemies and determine what to do next.
        /// </summary>
        private void FinishWave()
        {
            BunnyActor[] defeatedBunnies = GetDefeatedBunnies();
            if (defeatedBunnies.Length > 0)
                GlobalAudioSource.Instance.PlaySoundEffect(healSound);
            foreach (BunnyActor bunnyActor in defeatedBunnies)
                bunnyActor.Revive();

            battleMenu.SetPlayerStatText(bunnyActors);

            if(wave == maxWaves)
            {
                battleState = BattleState.Idle;
            }
            else
            {
                SetNextWave();
                StartTravel();
            }
        }

        /// <summary>
        /// Set the next wave of enemies.
        /// </summary>
        private void SetNextWave()
        {
            if (wave == maxWaves)
                return;

            wave++;
            int i = 0;

            if (wave == maxWaves)
            {
                // Boss wave
                enemyActors[i].gameObject.SetActive(true);
                enemyActors[i].FighterData = boss;
                enemyActors[i].transform.position = Vector2.Lerp(enemySpawnPointA, enemySpawnPointB, 0.5f);
                GlobalAudioSource.Instance.PlayMusic(bossMusic);
                i++;
            }
            else
            {
                // Regular enemy wave
                bool penultimateWave = wave == maxWaves - 1;

                for(i = 0; i < wave && i < enemyActors.Length; i++)
                {
                    // Set random enemies
                    int enemyIndex = Random.Range(0, enemies.Length);
                    enemyActors[i].gameObject.SetActive(true);
                    enemyActors[i].FighterData = enemies[enemyIndex];

                    // Set enemy position
                    float interpolant;
                    if (penultimateWave)
                        interpolant = (float) i / (wave - 1);
                    else
                        interpolant = (float)(i + 1) / (wave + 1);
                    enemyActors[i].transform.position = Vector2.Lerp(enemySpawnPointA, enemySpawnPointB, interpolant);
                }
            }

            // Deactivate remaining actors
            for (int j = i; j < enemyActors.Length; j++)
            {
                if (enemyActors[j].gameObject.activeSelf)
                    enemyActors[j].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Start scrolling the background and moving enemy actors so that the bunnies appear to be traveling.
        /// </summary>
        private void StartTravel()
        {
            battleState = BattleState.Traveling;

            battleMenu.ShowPlayerStatPanel(false);
            battleMenu.ShowTurnPanel(false);

            foreach (BunnyActor bunnyActor in bunnyActors)
                bunnyActor.SetMoving(true);

            List<Transform> enemyTransforms = new List<Transform>();

            foreach(EnemyActor enemyActor in enemyActors)
            {
                if (enemyActor.gameObject.activeSelf)
                    enemyTransforms.Add(enemyActor.transform);
            }

            foreach (Transform enemyTransform in enemyTransforms)
            {
                enemyTransform.Translate(Vector2.right * battleBackground.ScreenWidth);
            }

            StartCoroutine(battleBackground.ScrollBackground(1, travelSpeed, enemyTransforms.ToArray()));
        }

        #endregion

        #region Player Input

        /// <summary>
        /// Count the input for the current bunny as received and prepare to either receive the next input or begin handling turns.
        /// </summary>
        private void NextInput()
        {
            do
            {
                inputsReceived++;
            }
            while (inputsReceived < bunnyActors.Length && !SelectedBunny.IsAlive);

            if(inputsReceived < bunnyActors.Length)
                PromptInput();
        }

        /// <summary>
        /// Set up the input menu and prompt the user to input their turn for the current bunny.
        /// </summary>
        private void PromptInput()
        {
            battleMenu.SelectedBunny = SelectedBunny;
            battleMenu.ShowPlayerInputPanel(true);
            battleMenu.ShowOptionPanel(true);
            battleMenu.ShowTurnPanel(false);
            battleMenu.SetEnemyButtons(enemyActors); 
            battleMenu.SetSkillButtons();
            battleMenu.PromptPlayerInput();
        }

        #endregion
    }
}