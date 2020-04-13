using UnityEngine;
using UnityEngine.SceneManagement;

public enum MenuState
{
    MainMenu,
    LevelSelection
}

public class MenuManager : MonoBehaviour
{
    public MenuState state;
    [SerializeField]
    private Animator menuPanelAnimator;
    private void Start()
    {
        // Load level progress
        state = MenuState.MainMenu;
    }
    private void Update()
    {
        if (state == MenuState.MainMenu && Input.GetMouseButtonDown(0))
        {
            ChangeState(MenuState.LevelSelection);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state == MenuState.LevelSelection)
            {
                UserSelectBackToMainMenu();
            }
            else
            {
                Application.Quit();
            }
        }
    }
    public void ChangeState(MenuState newState)
    {
        state = newState;
        switch (state)
        {
            case MenuState.MainMenu:
                menuPanelAnimator.SetTrigger("Transition To Main");
                break;
            case MenuState.LevelSelection:
                menuPanelAnimator.SetTrigger("Transition To Level");
                break;
        }
    }
    public void UserSelectLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < GameManager.instance.levels.Count)
        {
            GameManager.instance.selectedLevelIndex = levelIndex;
            SceneManager.LoadScene("Level");
        }
    }
    public void UserSelectEndless() {
        SceneManager.LoadScene("Level");
    }
    public void UserSelectBackToMainMenu()
    {
        ChangeState(MenuState.MainMenu);
    }
}