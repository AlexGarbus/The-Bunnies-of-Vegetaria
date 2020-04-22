// TODO: Move functionality to Area Menu
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaButton : MonoBehaviour
{
    private enum Area { LettuceFields = 1, CeleryWoods, BroccoliForest, BokChoyBluff, CarrotTop }

    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Image areaImage;
    [SerializeField] private Area area;

    private bool locked = false;
    private const string lockedText = "???";
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;

        if((int)area > SaveData.current.areasUnlocked)
        {
            locked = true;
            buttonText.text = lockedText;
            areaImage.color = Color.black;
        }
    }

    public void LoadArea()
    {
        if(!locked)
        {
            // TODO: Set area in Game Manager
        }
    }
}
