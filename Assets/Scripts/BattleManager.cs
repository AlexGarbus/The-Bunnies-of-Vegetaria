using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private int maxWaves = 5;
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private BattleMenu battleMenu;

    // TODO: Maybe load from JSON instead?
    [Tooltip("The backgrounds for each area. These should be in ascending order by area.")]
    [SerializeField] private Sprite[] backgrounds;

    [Header("Actors")]
    [SerializeField] private BunnyActor bunnyActors;
    [SerializeField] private EnemyActor[] enemyActors;

    private enum BattleState { WaitingForInput, HandlingTurns }

    private int inputsReceived = 0;
    private int wave = 1;
    private BattleState battleState = BattleState.WaitingForInput;
    private GameManager gameManager;
    private Enemy[] enemies;
    private Enemy boss;
    private TurnList turns = new TurnList(8);

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

        SetWave();

        battleMenu.ShowPlayerInputPanel(true);
    }

    /// <summary>
    /// Insert an attack turn into the turn list.
    /// </summary>
    public void InsertAttackTurn()
    {

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
    /// Count the input for the current bunny as received and move to the next.
    /// </summary>
    public void ReceiveInput()
    {

    }

    /// <summary>
    /// Set the current wave of enemies.
    /// </summary>
    private void SetWave()
    {
        if(wave == maxWaves)
        {
            enemyActors[0].FighterInfo = boss;
            musicSource.clip = bossMusic;
            musicSource.Play();
        }
        else
        {
            for(int i = 0; i < wave && i < enemyActors.Length; i++)
            {
                int enemyIndex = Random.Range(0, enemies.Length);
                enemyActors[i].FighterInfo = enemies[enemyIndex];
            }
        }
    }
}
