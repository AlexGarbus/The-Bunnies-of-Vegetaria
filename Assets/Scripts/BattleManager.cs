using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private AudioClip bossMusic;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private BattleBackground battleBackground;
        [SerializeField] private BattleMenu battleMenu;
        [SerializeField] private SceneTransition sceneTransition;

        [Header("Battle Flow")]
        [SerializeField] private int maxWaves = 5;
        [SerializeField] private float turnTime = 1f;
        [SerializeField] private float travelSpeed = 1f;

        [Header("Actors")]
        [Tooltip("The bunny objects in the scene. These should be ordered according to the BunnyType enum.")]
        [SerializeField] private BunnyActor[] bunnyActors;
        [Tooltip("The enemyobjects in the scene. These should be in ascending order by spawn position.")]
        [SerializeField] private EnemyActor[] enemyActors;
        
        private BunnyActor SelectedBunny 
        {
            get 
            {
                if (inputsReceived >= 0 && inputsReceived < bunnyActors.Length)
                    return bunnyActors[inputsReceived];
                else
                    return null;
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

            // Set up bunny actors
            foreach (BunnyActor actor in bunnyActors)
                actor.Observer = gameObject;
            bunnyActors[(int)Globals.BunnyType.Bunnight].FighterData = gameManager.Bunnight;
            bunnyActors[(int)Globals.BunnyType.Bunnecromancer].FighterData = gameManager.Bunnecromancer;
            bunnyActors[(int)Globals.BunnyType.Bunnurse].FighterData = gameManager.Bunnurse;
            bunnyActors[(int)Globals.BunnyType.Bunneerdowell].FighterData = gameManager.Bunneerdowell;
            battleMenu.SetPlayerStatText(bunnyActors);

            // Set up enemy actors
            foreach (EnemyActor actor in enemyActors)
                actor.Observer = gameObject;

            SetNextWave();
            StartTravel();
        }

        private void Update()
        {
            switch(battleState)
            {
                case BattleState.Idle:
                    break;
                case BattleState.Traveling:
                    if (!battleBackground.IsScrolling)
                    {
                        foreach (BunnyActor bunnyActor in bunnyActors)
                            bunnyActor.SetMoving(false);
                        battleMenu.ShowPlayerStatPanel(true);
                        battleState = BattleState.SettingUpInput;
                    }
                    break;
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
            while(!turnList.IsEmpty)
            {
                Turn turn = turnList.Pop();

                // Perform turn
                turn.TurnAction?.Invoke();
                battleMenu.SetTurnText(turn.Message);
                battleMenu.SetPlayerStatText(bunnyActors);

                yield return new WaitForSeconds(turnTime);
            }

            battleState = BattleState.DoneHandlingTurns;
        }

        #region Action Turns

        /// <summary>
        /// Insert a standard bunny attack turn into the turn list.
        /// </summary>
        public void InsertBunnyAttackTurn(GameObject targetObject)
        {
            IActor user = SelectedBunny;
            IActor target = targetObject.GetComponent<IActor>();
            Turn turn = new Turn(user, target, $"{user.FighterName} attacks {target.FighterName}!", () => user.DoDamage(target));
            turnList.Insert(turn);

            battleMenu.ShowEnemyPanel(false);
            NextInput();
        }

        /// <summary>
        /// Insert a bunny defend turn into the turn list.
        /// </summary>
        public void InsertBunnyDefendTurn()
        {
            SelectedBunny.isDefending = true;
            IActor user = SelectedBunny;
            Turn turn = new Turn(user, user, $"{user.FighterName} is defending!", null);
            turnList.Insert(turn);

            NextInput();
        }

        /// <summary>
        /// Insert a bunny skill turn into the turn list.
        /// </summary>
        public void InsertBunnySkillTurn(int skillIndex)
        {
            BunnyActor user = SelectedBunny;

            if (!SelectedBunny.CanUseSkill(skillIndex))
                return;

            IActor[] targets;
            if (user.GetSkillTarget(skillIndex) == Skill.TargetType.Bunny)
                targets = bunnyActors;
            else
                targets = GetAliveEnemies();

            Turn turn = new Turn(user, targets, $"{user.FighterName} used {user.GetSkillName(skillIndex)}!", () => user.UseSkill(skillIndex, targets));
            turnList.Insert(turn);

            battleMenu.ShowSkillPanel(false);
            NextInput();
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

        #region Status Turns

        /// <summary>
        /// Insert a defeat turn for a bunny actor and lose the battle if all bunnies are defeated.
        /// </summary>
        /// <param name="bunnyActor">The bunny that has been defeated.</param>
        private void BunnyDefeat(BunnyActor bunnyActor)
        {
            turnList.RemoveUserTurns(bunnyActor);
            turnList.RemoveTargetTurns(bunnyActor);

            Turn turn = new Turn(bunnyActor, $"{bunnyActor.FighterName} was defeated!", () => bunnyActor.Defeat());
            turnList.Push(turn);

            if(GetAliveBunnies().Length == 0)
            {
                // Battle has been lost
                turnList.RemoveEnemyTurns();
                turn = new Turn(bunnyActor, "The bunnies have lost! Retreat!", () => sceneTransition.SaveAndLoadScene("AreaSelect"));
                turnList.Append(turn);
            }
        }

        /// <summary>
        /// Insert a defeat turn for an enemy actor.
        /// </summary>
        /// <param name="enemyActor">The enemy that has been defeated.</param>
        private void EnemyDefeat(EnemyActor enemyActor)
        {
            turnList.RemoveUserTurns(enemyActor);
            turnList.RemoveTargetTurns(enemyActor);

            GainExperience(enemyActor.Experience);

            Turn turn = new Turn(enemyActor, $"{enemyActor.FighterName} was defeated!", () => enemyActor.Defeat());
            turnList.Push(turn);

            if(GetAliveEnemies().Length == 0)
            {
                turnList.RemoveNonemptyTargetTurns();

                if (wave == maxWaves)
                {
                    // Boss has been defeated
                    turn = new Turn(enemyActor, "This area is clear!", () =>
                        {
                            if (SaveData.current.areasUnlocked == (int)gameManager.BattleArea 
                                && SaveData.current.areasUnlocked < (int)Globals.Area.CarrotTop)
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
        private void LevelUp(BunnyActor bunnyActor)
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

            int i = 0;
            wave++;

            if (wave == maxWaves)
            {
                // Boss wave
                enemyActors[i].gameObject.SetActive(true);
                enemyActors[i].FighterData = boss;
                musicSource.clip = bossMusic;
                musicSource.Play();
                i++;
            }
            else
            {
                // Regular enemy wave
                for(i = 0; i < wave && i < enemyActors.Length; i++)
                {
                    int enemyIndex = UnityEngine.Random.Range(0, enemies.Length);
                    enemyActors[i].gameObject.SetActive(true);
                    enemyActors[i].FighterData = enemies[enemyIndex];
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