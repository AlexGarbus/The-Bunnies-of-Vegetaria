using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnemyRepository
{
    public Enemy[] enemies;
    public Enemy[] bosses;

    public static EnemyRepository LoadFromJSON()
    {
        return JsonUtility.FromJson<EnemyRepository>(Resources.Load<TextAsset>("JSON/enemies").text);
    }

    /// <summary>
    /// Get every regular enemy from an area.
    /// </summary>
    /// <param name="areaFilter">The area to get enemies from.</param>
    /// <returns>An IEnumerable containing all enemies from an area.</returns>
    public IEnumerable<Enemy> GetEnemiesFromArea(Area areaFilter)
    {
        IEnumerable<Enemy> enemiesFromArea = enemies.Where(enemy => 
            areaFilter == (Area)Enum.Parse(typeof(Area), enemy.area.Replace(" ", string.Empty), false)
        );
        return enemiesFromArea;
    }

    /// <summary>
    /// Get the boss enemy from an area.
    /// </summary>
    /// <param name="areaFilter">The area to get the boss from.</param>
    /// <returns>The boss from an area.</returns>
    public Enemy GetBossFromArea(Area areaFilter)
    {
        Enemy boss = enemies.First(enemy =>
            areaFilter == (Area)Enum.Parse(typeof(Area), enemy.area.Replace(" ", string.Empty), false)
        );
        return boss;
    }
}
