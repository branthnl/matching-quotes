using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public bool isPause, isAnswered;
    public Question question;
    private Level selectedLevel;
    [SerializeField]
    private TextMeshProUGUI levelText, progressText, quoteText, encouragementText, backgroundText;
    [SerializeField]
    private Animator pausePanelAnimator, encouragementTextAnimator;
    [SerializeField]
    private Transform optionPanel;
    [SerializeField]
    private GameObject optionButtonPrefab;
    private List<Button> optionButtons;
    public static LevelManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            Setup();
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    private void Setup()
    {
        isAnswered = false;
        selectedLevel = GameManager.instance.levels[GameManager.instance.selectedLevelIndex];
        levelText.text = GameManager.instance.selectedLevelName;
        progressText.text = string.Format("Quote {0}/{1}", GameManager.instance.selectedLevelProgress, selectedLevel.questions.Length);
        question = selectedLevel.questions[GameManager.instance.selectedLevelProgress];
        quoteText.text = question.q;
        optionButtons = new List<Button>();
        for (int i = 0; i < question.options.Length; ++i)
        {
            optionButtons.Add(AddOptionButton(i, question.options[i]));
        }
    }
    private Button AddOptionButton(int optionIndex, string optionText)
    {
        GameObject n = Instantiate(optionButtonPrefab);
        Button btn = n.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            UserAnswer(optionIndex);
        });
        n.GetComponentInChildren<TextMeshProUGUI>().text = optionText;
        n.transform.SetParent(optionPanel);
        n.GetComponent<RectTransform>().localScale = Vector3.one;
        return btn;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UserSelectPause();
        }
    }
    public void UserAnswer(int optionIndex)
    {
        if (isAnswered) return;
        if (optionIndex == question.correctIndex)
        {
            Debug.Log("CORRECT");
            encouragementText.text = selectedLevel.correctResponse;
            encouragementTextAnimator.SetTrigger("In");
            isAnswered = true;
        }
        else
        {
            Debug.Log("INCORRECT");
            encouragementText.text = selectedLevel.incorrectResponse;
            encouragementTextAnimator.SetTrigger("In");
            optionButtons[optionIndex].interactable = false;
        }
    }
    public void UserSelectNext()
    {
        GameManager.instance.selectedLevelProgress++;
        if (GameManager.instance.selectedLevelProgress < selectedLevel.questions.Length)
        {
            Setup();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            // Complete
        }
    }
    public void UserSelectHome()
    {
        Destroy(this);
        SceneManager.LoadScene("Menu");
    }
    public void UserSelectPause()
    {
        isPause = !isPause;
        pausePanelAnimator.SetTrigger(isPause ? "In" : "Out");
    }
}