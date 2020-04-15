using TMPro;
using System.Collections;
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
    private TextMeshProUGUI levelText, progressText, quoteText, backgroundText, encouragementText, answerButtonText;
    [SerializeField]
    private Animator pausePanelAnimator, backgroundTextAnimator, encouragementTextAnimator, optionPanelAnimator, bottomPanelAnimator, nextButtonAnimator;
    [SerializeField]
    private Transform optionPanel, chancePanel, answerButtonTransform, answerButtonResultTransform;
    private bool moveAnswerButtonToResultPosition;
    [SerializeField]
    private GameObject optionButtonPrefab;
    private List<Button> optionButtons;
    private void Awake()
    {
        isPause = false;
        isAnswered = false;
        moveAnswerButtonToResultPosition = false;
        if (GameManager.instance.isEndless) {
            selectedLevel = GameManager.instance.endlessLevel;
            // progressText.text = string.Format("Quote {0}", GameManager.instance.selectedLevelProgress + 1);
        }
        else {
            selectedLevel = GameManager.instance.levels[GameManager.instance.selectedLevelIndex];
            progressText.text = string.Format("Quote {0}/{1}", GameManager.instance.selectedLevelProgress + 1, selectedLevel.questions.Length);
        }
        levelText.text = GameManager.instance.selectedLevelName;
        SetQuestion();
    }
    private void SetQuestion()
    {
        question = selectedLevel.questions[GameManager.instance.selectedLevelProgress];
        quoteText.text = "\"" + question.q + "\"";
        backgroundText.text = question.backgroundStory;
        if (backgroundText.text == selectedLevel.defaultBackgroundStory)
        {
            backgroundText.fontStyle = FontStyles.Italic;
            backgroundText.alignment = TextAlignmentOptions.Center;
        }
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
        if (moveAnswerButtonToResultPosition)
        {
            answerButtonTransform.position = Vector3.Lerp(answerButtonTransform.position, answerButtonResultTransform.position, 0.2f);
        }
    }
    public void UserAnswer(int optionIndex)
    {
        if (isAnswered) return;
        if (optionIndex == question.correctIndex)
        {
            Debug.Log("CORRECT");
            isAnswered = true;
            for (int i = optionButtons.Count - 1; i >= 0; --i)
            {
                if (i != question.correctIndex)
                {
                    optionButtons[i].interactable = false;
                }
            }
            StartCoroutine(TransitionToResult());
        }
        else
        {
            Debug.Log("INCORRECT");
            Destroy(chancePanel.GetChild(0).gameObject);
            encouragementText.text = selectedLevel.incorrectResponse;
            encouragementTextAnimator.SetTrigger("In");
            optionButtons[optionIndex].interactable = false;
        }
    }
    IEnumerator TransitionToResult()
    {
        encouragementText.text = selectedLevel.correctResponse;
        encouragementTextAnimator.SetTrigger("In");
        optionButtons[question.correctIndex].GetComponent<Animator>().SetTrigger("Grow Outline");
        bottomPanelAnimator.SetTrigger("Out");
        answerButtonText.text = question.options[question.correctIndex];
        yield return new WaitForSeconds(1.0f);
        answerButtonTransform.position = optionButtons[question.correctIndex].transform.position;
        optionButtons[question.correctIndex].transform.localScale = Vector3.zero;
        moveAnswerButtonToResultPosition = true;
        optionPanelAnimator.SetTrigger("Out");
        encouragementTextAnimator.SetTrigger("Transition To Result");
        yield return new WaitForSeconds(1.0f);
        backgroundTextAnimator.SetTrigger("In");
        if (question.backgroundStory == selectedLevel.defaultBackgroundStory)
        {
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        nextButtonAnimator.SetTrigger("In");
    }
    public void UserSelectNext()
    {
        GameManager.instance.selectedLevelProgress++;
        GameManager.instance.SaveProgress();
        if (GameManager.instance.selectedLevelProgress < selectedLevel.questions.Length)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            if (GameManager.instance.isEndless) {
                GameManager.instance.selectedLevelProgress = 0;
            }
            else {
                // Complete
            }
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