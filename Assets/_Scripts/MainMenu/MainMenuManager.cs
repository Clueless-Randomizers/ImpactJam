using System.Collections;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuStartPanel;
    [SerializeField] private GameObject levelSelectPanel;

    public void PlayButton()
    {
        mainMenuStartPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }
    
    public void QuitButton()
    {
        Application.Quit();
    }
    
    public void BackButton()
    {
        levelSelectPanel.SetActive(false);
        mainMenuStartPanel.SetActive(true);
    }

    public void StartGame(SceneReference level)
    {
        SceneManager.LoadScene(level.Name);
    }
}
