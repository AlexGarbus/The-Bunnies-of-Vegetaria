using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMenu : MonoBehaviour
{
    [SerializeField] private GameObject playerInputPanel;
    [SerializeField] private GameObject turnPanel;

    private void Start()
    {
        if (playerInputPanel.activeSelf)
            playerInputPanel.SetActive(false);
        if (turnPanel.activeSelf)
            turnPanel.SetActive(false);
    }

    public void ShowPlayerInputPanel(bool isActive)
    {
        playerInputPanel.SetActive(isActive);
    }

    public void ShowTurnPanel(bool isActive)
    {
        turnPanel.SetActive(isActive);
    }
}
