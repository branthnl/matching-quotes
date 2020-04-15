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
    private bool isTransitioning = true;
    private void Start()
    {
        // Load level progress
        state = MenuState.MainMenu;
        Invoke("doneTransitioning", 0.4f);
    }
    private void doneTransitioning()
    {
        isTransitioning = false;
    }
    private void Update()
    {
        if (isTransitioning) return;
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
            GameManager.instance.isEndless = false;
            GameManager.instance.selectedLevelIndex = levelIndex;
            GameManager.instance.LoadProgress();
            SceneManager.LoadScene("Level");
        }
    }
    public void UserSelectEndless()
    {
        GameManager.instance.isEndless = true;
        SceneManager.LoadScene("Level");
    }
    public void UserSelectBackToMainMenu()
    {
        ChangeState(MenuState.MainMenu);
    }
}