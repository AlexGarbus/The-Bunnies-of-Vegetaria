using System.Collections;
using System.Collections.Generic;
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

    // TODO: Methods for getting enemies by area
}
