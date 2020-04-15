using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField]
    private Transform levelParent;
    [SerializeField]
    private GameObject levelButtonPrefab;
    private bool isTransitioning = true;
    private Dictionary<int, TextMeshProUGUI> levelButtonsText;
    private void Awake()
    {
        var levels = GameManager.instance.levels;
        levelButtonsText = new Dictionary<int, TextMeshProUGUI>();
        for (int i = levels.Count - 1; i >= 0; --i)
        {
            int levelProgress = PlayerPrefs.GetInt(levels[i].levelName, 0);
            AddLevelButton(i, levels[i].levelName, levelProgress, levels[i].questions.Length);
        }
    }
    private void AddLevelButton(int levelIndex, string levelName, int levelProgress, int questionAmount)
    {
        GameObject n = Instantiate(levelButtonPrefab);
        TextMeshProUGUI buttonText = n.GetComponentInChildren<TextMeshProUGUI>();

        n.GetComponent<Button>().onClick.AddListener(() =>
        {
            UserSelectLevel(levelIndex);
        });

        if (levelProgress >= questionAmount)
        {
            buttonText.text = string.Format("{0} 100%", levelName);
        }
        else
        {
            buttonText.text = string.Format("{0} {1}/{2}", levelName, levelProgress, questionAmount);
        }

        levelButtonsText.Add(levelIndex, buttonText);

        Button resetButton = n.transform.GetChild(1).GetComponent<Button>();
        if (levelProgress > 0)
        {
            resetButton.gameObject.SetActive(true);
        }
        resetButton.onClick.AddListener(() =>
        {
            Debug.Log("REstting " + levelName);
            GameManager.instance.ResetProgress(levelName);
            levelButtonsText[levelIndex].text = string.Format("{0} {1}/{2}", levelName, 0, questionAmount);
        });
        n.transform.SetParent(levelParent);
        n.transform.localScale = Vector3.one;
        n.transform.SetAsFirstSibling();
    }
    private void Start()
    {
        state = MenuState.MainMenu;
        Invoke("doneTransitioning", 0.2f);
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