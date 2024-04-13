using System.Collections;
using System.Collections.Generic;
using Eflatun.SceneReference;
using TMPro;
using UnityEngine;

public class LevelMenuInformation : MonoBehaviour
{
    [SerializeField] private SceneReference level;
    [SerializeField] private TMP_Text levelNameText;
    [SerializeField] private MainMenuManager mainMenuManager;
    
    void Start()
    {
        levelNameText.text = level.Name;
    }
    
    public void StartGame()
    {
        mainMenuManager.StartGame(level);
    }
}
