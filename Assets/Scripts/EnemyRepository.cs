using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [Serializable]
    public class EnemyRepository
    {
        public Enemy[] enemies;
        public Enemy[] bosses;

        public static EnemyRepository LoadFromJSON()
        {
            return JsonUtility.FromJson<EnemyRepository>(Resources.Load<TextAsset>("Text Assets/enemies").text);
        }

        /// <summary>
        /// Get every regular enemy from an area.
        /// </summary>
        /// <param name="area">The area to get enemies from.</param>
        /// <returns>An IEnumerable containing all enemies from an area.</returns>
        public IEnumerable<Enemy> GetEnemiesFromArea(Globals.Area area)
        {
            IEnumerable<Enemy> enemiesFromArea = enemies.Where(enemy =>
                {
                    string enemyArea = enemy.area.Replace(" ", string.Empty);
                    return area == (Globals.Area)Enum.Parse(typeof(Globals.Area), enemyArea, false);
                }
            );

            return enemiesFromArea;
        }

        /// <summary>
        /// Get the boss enemy from an area.
        /// </summary>
        /// <param name="area">The area to get the boss from.</param>
        /// <returns>The boss from an area.</returns>
        public Enemy GetBossFromArea(Globals.Area area)
        {
            Enemy boss = bosses.First(enemy =>
                {
                    string enemyArea = enemy.area.Replace(" ", string.Empty);
                    return area == (Globals.Area)Enum.Parse(typeof(Globals.Area), enemyArea, false);
                }
            );

            return boss;
        }
    }
}