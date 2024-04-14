using Eflatun.SceneReference;
using TMPro;
using UnityEngine;

namespace _Scripts.MainMenu
{
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
}
