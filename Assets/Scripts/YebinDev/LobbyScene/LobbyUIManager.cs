using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    public GameObject tutorialPanel;

    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
    }   
    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }   
}
