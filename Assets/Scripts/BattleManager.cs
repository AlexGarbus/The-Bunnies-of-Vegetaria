using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private BattleMenu battleMenu;

    // TODO: Maybe load from JSON instead?
    [Tooltip("The backgrounds for each area. These should be in ascending order by area.")]
    [SerializeField] private Sprite[] backgrounds;

    [Header("Battle Flow")]
    [SerializeField] private int maxWaves = 5;
    [SerializeField] private float turnTime = 1f;

    [Header("Actors")]
    [SerializeField] private BunnyActor[] bunnyActors;
    [SerializeField] private EnemyActor[] enemyActors;

    private enum BattleState { WaitingForInput, HandlingTurns }

    private int inputsReceived = 0;
    private int wave = 1;
    private BattleState battleState = BattleState.WaitingForInput;
    private GameManager gameManager;
    private Enemy[] enemies;
    private Enemy boss;
    private TurnList turnList = new TurnList(8);

    private void Start()
    {
        gameManager = GameManager.Instance;

        // Get area
        Area area;
        if (gameManager.AreaIndex == 0)
            area = Area.LettuceFields;
        else
            area = (Area)gameManager.AreaIndex;

        // Load enemies
        EnemyRepository enemyRepository = EnemyRepository.LoadFromJSON();
        enemies = enemyRepository.GetEnemiesFromArea(area).ToArray();
        boss = enemyRepository.GetBossFromArea(area);

        // TODO: Set background

        // Set up bunny actors
        bunnyActors[(int)BunnyType.Bunnight].FighterInfo = gameManager.Bunnight;
        bunnyActors[(int)BunnyType.Bunnecromancer].FighterInfo = gameManager.Bunnecromancer;
        bunnyActors[(int)BunnyType.Bunnurse].FighterInfo = gameManager.Bunnurse;
        bunnyActors[(int)BunnyType.Bunneerdowell].FighterInfo = gameManager.Bunneerdowell;

        // Set up enemy actors
        foreach (EnemyActor actor in enemyActors)
        {
            if (actor.gameObject.activeSelf)
                actor.gameObject.SetActive(false);
        }
        SetWave();

        PromptInput();
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
    /// Count the input for the current bunny as received and prepare to either receive the next input or begin handling turns.
    /// </summary>
    private void NextInput()
    {
        do
        {
            inputsReceived++;
        }
        while (inputsReceived < bunnyActors.Length && bunnyActors[inputsReceived].CurrentHealth != 0);

        if(inputsReceived >= bunnyActors.Length)
        {
            // Input received for all bunnies
            inputsReceived = 0;
            battleState = BattleState.HandlingTurns;
            battleMenu.ShowPlayerInputPanel(false);
            battleMenu.ShowTurnPanel(true);
            StartCoroutine(HandleTurns());
        }
        else
        {
            PromptInput();
        }
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
            enemyActors[0].gameObject.SetActive(true);
            enemyActors[0].FighterInfo = boss;
            musicSource.clip = bossMusic;
            musicSource.Play();
        }
        else
        {
            for(int i = 0; i < wave && i < enemyActors.Length; i++)
            {
                int enemyIndex = Random.Range(0, enemies.Length);
                enemyActors[i].gameObject.SetActive(true);
                enemyActors[i].FighterInfo = enemies[enemyIndex];
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

            battleMenu.SetTurnText(turn.Message);
            turn.TurnAction();

            yield return new WaitForSeconds(turnTime);
        }

        battleState = BattleState.WaitingForInput;
        PromptInput();
    }
}