using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class BattleHandler : MonoBehaviour
    {
        // TODO: Move audio playing to separate class
        [SerializeField] private AudioClip bossMusic;
        [SerializeField] private BattleBackground battleBackground;
        [SerializeField] private SceneTransition sceneTransition;

        [Header("Actors")]
        [Tooltip("The bunny objects in the scene. These should be ordered according to the BunnyType enum.")]
        [SerializeField] private BunnyActor[] bunnyActors;
        [Tooltip("The enemy objects in the scene. These should be in ascending order by spawn position.")]
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

        [Header("Messages")]
        [Tooltip("The message that will display when a bunny attacks. Use {0} in place of a bunny's name and {1} in place of a target's name.")]
        [SerializeField] private string attackMessage = "{0} attacks {1}!";
        [Tooltip("The message that will display when a bunny defends. Use {0} in place of a bunny's name.")]
        [SerializeField] private string defendMessage = "{0} is defending!";
        [Tooltip("The message that will display when a bunny uses a skill. Use {0} in place of a bunny's name and {1} in place of a skill's name.")]
        [SerializeField] private string skillMessage = "{0} used {1}!";
        [Tooltip("The message that will display when a fighter is defeated. Use {0} in place of a fighter's name.")]
        [SerializeField] private string defeatMessage = "{0} was defeated!";
        [Tooltip("The message that will display when a bunny levels up. Use {0} in place of a bunny's name.")]
        [SerializeField] private string levelUpMessage = "{0} leveled up!";
        [Tooltip("The message that will display when the bunnies win the battle.")]
        [SerializeField] private string winMessage = "This area is clear!";
        [Tooltip("The message that will display when the bunnies lose the battle.")]
        [SerializeField] private string loseMessage = "The bunnies have lost! Retreat!";

        private void OnDrawGizmosSelected()
        {
            // Draw enemy spawn line
            Gizmos.color = Color.red;
            Gizmos.DrawLine(enemySpawnPointA, enemySpawnPointB);
        }

        public static event EventHandler<BattleEventArgs> OnBunniesInitialized, OnEnemiesInitialized, OnSettingUpInput, OnSelectedBunnyChanged, OnTurnPerformed, OnWaveWon, OnWaveLost;

        private bool IsBossWave => wave >= maxWaves;
        private Bunny SelectedBunny
        {
            get
            {
                if (inputCount > 0 && inputCount <= currentBunnies.Length && (battleState == BattleState.SettingUpInput || battleState == BattleState.HandlingInput))
                {
                    return currentBunnies[inputCount - 1];
                }
                else
                {
                    Debug.LogWarning("Attempting to get selected bunny when none is selected.");
                    return null;
                }
            }
        }

        private enum BattleState { Idle, SettingUpInput, HandlingInput, HandlingTurns }

        private int inputCount = 0;
        private int wave = 1;
        private BattleState battleState;
        private AudioClip healSound;
        private Bunny[] currentBunnies;
        private Enemy[] currentEnemies;
        private Enemy[] areaEnemies;
        private Enemy areaBoss;
        private GameManager gameManager;
        private TurnCollection turnCollection = new TurnCollection(8);

        private void Start()
        {
            healSound = Resources.Load<AudioClip>("Sounds/heal");
            gameManager = GameManager.Instance;

            LoadEnemies(gameManager.BattleArea);
            InitializeBunnies(gameManager.Party);

            if (gameManager.startBattleAtBoss.Pop())
            {
                wave = maxWaves;
                InitializeWave();
                battleState = BattleState.SettingUpInput;
            }
            else
            {
                InitializeWave();
                StartTravel();
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from bunny events
            foreach (Bunny bunny in currentBunnies)
            {
                bunny.OnDefeat -= Bunny_OnDefeat;
                bunny.OnLevelUp -= Bunny_OnLevelUp;
            }

            // Unsubscribe from enemy events
            foreach (Enemy enemy in GetAliveEnemies())
            {
                enemy.OnDefeat -= Enemy_OnDefeat;
            }
        }

        private void Update()
        {
            // Battle state machine
            switch (battleState)
            {
                case BattleState.Idle:
                    break;
                case BattleState.SettingUpInput:
                    // Prepare to receive input
                    inputCount = 0;
                    NextInput();
                    OnSettingUpInput?.Invoke(this, new BattleEventArgs(currentBunnies, currentEnemies, SelectedBunny, null));
                    battleState = BattleState.HandlingInput;
                    break;
                case BattleState.HandlingInput:
                    if (inputCount > currentBunnies.Length)
                    {
                        // Input for all bunnies received
                        InsertEnemyTurns();
                        StartCoroutine(HandleTurns());
                        battleState = BattleState.HandlingTurns;
                    }
                    break;
                case BattleState.HandlingTurns:
                    // Coroutine is running
                    break;
            }
        }

        private void Bunny_OnDefeat(object sender, EventArgs e) => PushBunnyDefeatTurn(sender as Bunny);

        private void Bunny_OnLevelUp(object sender, EventArgs e) => PushLevelUpTurn(sender as Bunny);

        private void Enemy_OnDefeat(object sender, EventArgs e) => PushEnemyDefeatTurn(sender as Enemy);

        /// <summary>
        /// Find the next bunny to get input for.
        /// </summary>
        private void NextInput()
        {
            // Find next bunny
            do
            {
                inputCount++;
            }
            while (inputCount <= bunnyActors.Length && !SelectedBunny.IsAlive);

            // Raise event
            BattleEventArgs args;
            if (inputCount <= bunnyActors.Length)
                args = new BattleEventArgs(currentBunnies, currentEnemies, SelectedBunny, null);
            else
                args = new BattleEventArgs(currentBunnies, currentEnemies, null, null);
            OnSelectedBunnyChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Initialize the stats, sprites, and positions of the current wave of enemies.
        /// </summary>
        private void InitializeWave()
        {
            if (IsBossWave)
            {
                // Boss wave
                InitializeEnemies(new Enemy[] { areaBoss });
                GlobalAudioSource.Instance.PlayMusic(bossMusic);
                enemyActors[0].transform.position = Vector2.Lerp(enemySpawnPointA, enemySpawnPointB, 0.5f);
            }
            else
            {
                // Regular enemy wave
                bool isPenultimateWave = wave == maxWaves - 1;
                Enemy[] randomEnemies = new Enemy[enemyActors.Length < wave ? enemyActors.Length : wave];

                // Set random enemies
                for (int i = 0; i < wave && i < enemyActors.Length; i++)
                {
                    int enemyIndex = UnityEngine.Random.Range(0, areaEnemies.Length);
                    randomEnemies[i] = areaEnemies[enemyIndex];

                    // Set actor positions
                    float interpolant;
                    if (isPenultimateWave)
                        interpolant = (float)i / (wave - 1);
                    else
                        interpolant = (float)(i + 1) / (wave + 1);
                    enemyActors[i].transform.position = Vector2.Lerp(enemySpawnPointA, enemySpawnPointB, interpolant);
                }
                InitializeEnemies(randomEnemies);
            }
        }

        /// <summary>
        /// Move the enemy actors offscreen, then start scrolling the background and actors so that the bunnies appear to be traveling.
        /// </summary>
        private void StartTravel()
        {
            // Get active enemy transforms
            List<Transform> enemyTransformList = new List<Transform>();
            foreach (EnemyActor enemyActor in enemyActors)
            {
                if (enemyActor.gameObject.activeSelf)
                    enemyTransformList.Add(enemyActor.transform);
            }
            Transform[] enemyTransforms = enemyTransformList.ToArray();

            // Move enemies offscreen and begin scrolling
            battleBackground.TranslateOffscreen(1, enemyTransforms);
            battleBackground.StartScrollBackground(1, travelSpeed, enemyTransforms, () => battleState = BattleState.SettingUpInput);
        }

        /// <summary>
        /// Win the current enemy wave and set the battle state accordingly.
        /// </summary>
        private void WinWave()
        {
            battleState = BattleState.Idle;

            if (IsBossWave)
            {
                // Unlock next area
                int areasUnlocked = SaveData.current.areasUnlocked;
                if (areasUnlocked == (int)gameManager.BattleArea
                    && areasUnlocked < (int)Globals.Area.CarrotTop)
                {
                    SaveData.current.areasUnlocked++;
                }

                // Area is clear
                if (gameManager.cutsceneFileName.HasValue())
                    sceneTransition.SaveAndLoadScene("Cutscene");
                else
                    sceneTransition.SaveAndLoadScene("AreaSelect");
            }
            else
            {
                // Revive defeated bunnies
                foreach (Bunny bunny in GetDefeatedBunnies())
                    bunny.Revive();

                // Move to next wave
                wave++;
                if (IsBossWave && gameManager.cutsceneFileName.HasValue())
                    sceneTransition.SaveAndLoadScene("Cutscene");
                else
                    InitializeWave();
                StartTravel();
            }

            BattleEventArgs args = new BattleEventArgs(currentBunnies, currentEnemies, null, null);
            OnWaveWon?.Invoke(this, args);
        }

        /// <summary>
        /// Lose the current enemy wave and exit the battle scene.
        /// </summary>
        private void LoseWave()
        {
            battleState = BattleState.Idle;
            sceneTransition.SaveAndLoadScene("AreaSelect");

            BattleEventArgs args = new BattleEventArgs(currentBunnies, currentEnemies, null, null);
            OnWaveLost?.Invoke(this, args);
        }

        /// <summary>
        /// Perform each turn in the turn collection. Sets the battle state once all turns have been performed.
        /// </summary>
        private IEnumerator HandleTurns()
        {
            WaitForSeconds turnDelay = new WaitForSeconds(turnTime);

            while (!turnCollection.IsEmpty)
            {
                // Perform turn
                Turn turn = turnCollection.Pop();
                turn.TurnAction?.Invoke();
                BattleEventArgs args = new BattleEventArgs(currentBunnies, currentEnemies, null, turn);
                OnTurnPerformed?.Invoke(this, args);

                yield return turnDelay;
            }

            HandleEndOfTurns();
        }

        /// <summary>
        /// Check the current fighter status and set the battle state accordingly.
        /// </summary>
        private void HandleEndOfTurns()
        {
            if (GetAliveBunnies().Length == 0)
            {
                // Battle is lost
                LoseWave();
            }
            else if (GetAliveEnemies().Length == 0)
            {
                // Battle is won
                WinWave();
            }
            else
            {
                // Battle is undecided
                foreach (Bunny bunny in currentBunnies)
                    bunny.IsDefending = false;
                battleState = BattleState.SettingUpInput;
            }
        }

        #region Fighter Management

        /// <summary>
        /// Get all bunnies with health greater than 0.
        /// </summary>
        /// <returns>An array with all bunnies considered as alive.</returns>
        private Bunny[] GetAliveBunnies() => currentBunnies.Where(bunny => bunny.IsAlive).ToArray();

        /// <summary>
        /// Get all bunnies with health equal to 0.
        /// </summary>
        /// <returns>An array with all bunnies considered as defeated.</returns>
        private Bunny[] GetDefeatedBunnies() => currentBunnies.Where(bunny => !bunny.IsAlive).ToArray();

        /// <summary>
        /// Get all enemies with health greater than 0.
        /// </summary>
        /// <returns>An array with all enemies considered as alive.</returns>
        private Enemy[] GetAliveEnemies() => currentEnemies.Where(enemy => enemy.IsAlive).ToArray();

        /// <summary>
        /// Get all enemy actors with health equal to 0.
        /// </summary>
        /// <returns>An array with all enemies considered as defeated.</returns>
        private Enemy[] GetDefeatedEnemies() => currentEnemies.Where(enemy => !enemy.IsAlive).ToArray();

        /// <summary>
        /// Get the actor associated with a specific bunny.
        /// </summary>
        /// <param name="bunny">The bunny to get the actor for.</param>
        /// <returns>The actor associated with this bunny. Null if none is found.</returns>
        private BunnyActor GetActor(Bunny bunny)
        {
            foreach (BunnyActor bunnyActor in bunnyActors)
            {
                if (bunnyActor.Fighter.Equals(bunny))
                    return bunnyActor;
            }

            return null;
        }

        /// <summary>
        /// Get the actor associated with a specific enemy.
        /// </summary>
        /// <param name="enemy">The enemy to get the actor for.</param>
        /// <returns>The actor associated with this enemy. Null if none is found.</returns>
        private EnemyActor GetActor(Enemy enemy)
        {
            foreach (EnemyActor enemyActor in enemyActors)
            {
                if (enemyActor.Fighter.Equals(enemy))
                    return enemyActor;
            }

            return null;
        }

        private void InitializeBunnies(Bunny[] bunnies)
        {
            this.currentBunnies = bunnies;
            for (int i = 0; i < bunnies.Length; i++)
                bunnyActors[i].Fighter = bunnies[i];

            BattleEventArgs args = new BattleEventArgs(bunnies, currentEnemies, null, null);
            OnBunniesInitialized?.Invoke(this, args);

            // Subscribe to events
            foreach (Bunny bunny in bunnies)
            {
                bunny.OnDefeat += Bunny_OnDefeat;
                bunny.OnLevelUp += Bunny_OnLevelUp;
            }
        }

        private void InitializeEnemies(Enemy[] enemies)
        {
            // Create copies of enemies and assign to actors
            currentEnemies = new Enemy[enemies.Length];
            for (int i = 0; i < enemies.Length; i++)
            {
                currentEnemies[i] = enemies[i].ShallowCopy();
                enemyActors[i].Fighter = currentEnemies[i];
            }

            BattleEventArgs args = new BattleEventArgs(currentBunnies, currentEnemies, null, null);
            OnEnemiesInitialized?.Invoke(this, args);

            // Subscribe to events
            foreach (Enemy enemy in currentEnemies)
            {
                enemy.OnDefeat += Enemy_OnDefeat;
            }
        }

        /// <summary>
        /// Load the enemies for a particular area.
        /// </summary>
        /// <param name="area">The area to load enemies for.</param>
        private void LoadEnemies(Globals.Area area)
        {
            EnemyRepository enemyRepository = EnemyRepository.LoadFromJSON();
            areaEnemies = enemyRepository.GetEnemiesFromArea(area).ToArray();
            areaBoss = enemyRepository.GetBossFromArea(area);
        }

        #endregion

        #region Turn Creation

        /// <summary>
        /// Insert a standard attack turn during which a bunny attacks a single enemy.
        /// </summary>
        /// <param name="enemyIndex">The index of the enemy to target.</param>
        public void InsertBunnyAttackTurn(int enemyIndex)
        {
            // Create and insert turn
            Bunny user = SelectedBunny;
            Enemy target = currentEnemies[enemyIndex];
            Turn turn = new Turn(user, target, string.Format(attackMessage, user.name, target.name), () => user.DoDamage(target));
            turnCollection.Insert(turn);

            // Move to next input
            NextInput();
        }

        /// <summary>
        /// Insert a defend turn during which a bunny takes reduced damage.
        /// </summary>
        public void InsertBunnyDefendTurn()
        {
            // Create and insert turn
            Bunny user = SelectedBunny;
            user.IsDefending = true;
            Turn turn = new Turn(user, user, string.Format(defendMessage, user.name), null);
            turnCollection.Insert(turn);

            // Move to next input
            NextInput();
        }

        /// <summary>
        /// Insert a skill turn during which a bunny uses a special skill.
        /// </summary>
        /// <param name="skillIndex">The index of the skill to use.</param>
        public void InsertBunnySkillTurn(int skillIndex)
        {
            Bunny user = SelectedBunny;

            // Make sure bunny can use the selected skill
            if (!user.CanUseSkill(skillIndex))
                return;

            // Get turn targets
            Fighter[] targets;
            if (user.Skills[skillIndex].Target == Globals.FighterType.Bunny)
                targets = currentBunnies;
            else
                targets = GetAliveEnemies();

            // Create and insert turn
            Turn turn = new Turn(user, targets, string.Format(skillMessage, user.name, user.Skills[skillIndex].Name), () => user.UseSkill(skillIndex, targets));
            turnCollection.Insert(turn);

            // Move to next input
            NextInput();
        }

        /// <summary>
        /// Insert turns for each enemy into the turn list.
        /// </summary>
        private void InsertEnemyTurns()
        {
            Bunny[] aliveBunnies = GetAliveBunnies();
            Enemy[] aliveEnemies = GetAliveEnemies();

            if (IsBossWave)
            {
                // Insert 2 turns for the single boss enemy
                for (int i = 0; i < 2; i++)
                {
                    Turn turn = aliveEnemies[0].GetTurn(aliveBunnies, aliveEnemies);
                    turnCollection.Insert(turn);
                }
            }
            else
            {
                // Insert 1 turn for each regular enemy
                foreach (Enemy enemy in aliveEnemies)
                {
                    Turn turn = enemy.GetTurn(aliveBunnies, aliveEnemies);
                    turnCollection.Insert(turn);
                }
            }
        }

        /// <summary>
        /// Insert a defeat turn for a bunny and lose the battle if all bunnies are defeated.
        /// </summary>
        /// <param name="bunny">The defeated bunny.</param>
        private void PushBunnyDefeatTurn(Bunny bunny)
        {
            // Remove turns
            turnCollection.RemoveUserTurns(bunny);
            turnCollection.RemoveTargetTurns(bunny);

            // Create and push defeat turn
            BunnyActor bunnyActor = GetActor(bunny);
            Turn turn = new Turn(bunny, string.Format(defeatMessage, bunny.name), () => bunnyActor.Defeat());
            turnCollection.Push(turn);

            if (GetAliveBunnies().Length == 0)
            {
                // Bunnies have lost
                turnCollection.RemoveEnemyTurns();
                turn = new Turn(bunny, loseMessage, null);
                turnCollection.Append(turn);
            }
        }

        /// <summary>
        /// Insert a defeat turn for an enemy.
        /// </summary>
        /// <param name="enemy">The defeated enemy.</param>
        private void PushEnemyDefeatTurn(Enemy enemy)
        {
            // Remove turns
            turnCollection.RemoveUserTurns(enemy);
            turnCollection.RemoveTargetTurns(enemy);

            // Create and push defeat turn
            EnemyActor enemyActor = GetActor(enemy);
            Turn turn = new Turn(enemy, string.Format(defeatMessage, enemy.name), () =>
            {
                enemyActor.Defeat();
                foreach (Bunny bunny in GetAliveBunnies())
                    bunny.AddExperience(enemy.Attack + enemy.Defense + enemy.Speed);
                // TODO: bunny.AddExperience(enemy.Experience);
            });
            turnCollection.Push(turn);

            // Unsubscribe from enemy events
            enemy.OnDefeat -= Enemy_OnDefeat;

            if (GetAliveEnemies().Length == 0)
            {
                // Enemies have lost
                turnCollection.RemoveNonemptyTargetTurns();

                if (IsBossWave)
                {
                    // Boss has been defeated
                    turn = new Turn(enemy, winMessage, null);
                    turnCollection.Append(turn);
                }
            }
        }

        /// <summary>
        /// Insert a level up turn for a bunny.
        /// </summary>
        /// <param name="bunny">The bunny that has leveled up.</param>
        private void PushLevelUpTurn(Bunny bunny)
        {
            Turn turn = new Turn(bunny, string.Format(levelUpMessage, bunny.name), () =>
            {
                bunny.Heal(100);
                bunny.RestoreSkillPoints(100);
                GlobalAudioSource.Instance.PlaySoundEffect(healSound);
            }
            );
            turnCollection.Push(turn);
        }

        #endregion
    }
}