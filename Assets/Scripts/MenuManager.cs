using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        // Load level progress
    }
    public void UserSelectLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < GameManager.instance.levels.Count)
        {
            GameManager.instance.selectedLevelIndex = levelIndex;
            SceneManager.LoadScene("Game");
        }
    }
}