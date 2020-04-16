using TMPro;
using System.Linq;
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
    private TextMeshProUGUI levelText, progressText, quoteText, backgroundText, encouragementText, answerButtonText, finalQuoteAuthorText;
    [SerializeField]
    private Animator pausePanelAnimator, backgroundTextAnimator, encouragementTextAnimator, optionPanelAnimator, bottomPanelAnimator, nextButtonAnimator;
    [SerializeField]
    private Transform optionPanel, chancePanel, answerButtonTransform, answerButtonResultTransform;
    private bool moveAnswerButtonToResultPosition;
    [SerializeField]
    private GameObject completePanel, optionButtonPrefab;
    [SerializeField]
    private Button hintButton;
    private Dictionary<int, Button> optionButtons;
    private GameManager gameManager;
    private void Awake()
    {
        gameManager = GameManager.instance;
        isPause = false;
        isAnswered = false;
        moveAnswerButtonToResultPosition = false;
        if (gameManager.isEndless)
        {
            selectedLevel = gameManager.endlessLevel;
            progressText.text = string.Format("Quote {0}", gameManager.selectedLevelProgress + 1);
        }
        else
        {
            selectedLevel = gameManager.levels[gameManager.selectedLevelIndex];
            progressText.text = string.Format("Quote {0}/{1}", gameManager.selectedLevelProgress + 1, selectedLevel.questions.Length);
        }
        levelText.text = gameManager.selectedLevelName;
        SetQuestion();
    }
    private void SetQuestion()
    {
        question = selectedLevel.questions[gameManager.selectedLevelProgress];
        quoteText.text = "\"" + question.q + "\"";
        backgroundText.text = question.backgroundStory;
        if (backgroundText.text == selectedLevel.defaultBackgroundStory)
        {
            backgroundText.fontStyle = FontStyles.Italic;
            backgroundText.alignment = TextAlignmentOptions.Center;
        }
        optionButtons = new Dictionary<int, Button>();
        List<int> optionIndexes = new List<int>();
        for (int i = 0; i < question.options.Length; ++i)
        {
            optionIndexes.Add(i);
        }
        if (selectedLevel.shuffleOptionPosition)
        {
            int optionIndexesLength = optionIndexes.Count;
            for (int i = 0; i < optionIndexesLength; ++i)
            {
                var tempValue = optionIndexes[i];
                int randomIndex = Random.Range(0, optionIndexesLength);
                optionIndexes[i] = optionIndexes[randomIndex];
                optionIndexes[randomIndex] = tempValue;
            }
        }
        foreach (int i in optionIndexes)
        {
            optionButtons.Add(i, AddOptionButton(i, question.options[i]));
        }
    }
    private Button AddOptionButton(int optionIndex, string optionText)
    {
        GameObject n = Instantiate(optionButtonPrefab);
        Button btn = n.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            // gameManager.PlaySound("Pop1");
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
            // Debug.Log("CORRECT");
            isAnswered = true;
            for (int i = optionButtons.Count - 1; i >= 0; --i)
            {
                if (i != question.correctIndex)
                {
                    optionButtons[i].interactable = false;
                }
            }
            gameManager.PlaySound("Bell");
            StartCoroutine(TransitionToResult());
        }
        else
        {
            // Debug.Log("INCORRECT");
            gameManager.PlaySound("Pop1");
            Destroy(chancePanel.GetChild(0).gameObject);
            encouragementText.text = selectedLevel.incorrectResponse;
            encouragementTextAnimator.SetTrigger("In");
            optionButtons[optionIndex].interactable = false;
            int interactableCount = 0;
            for (int i = optionButtons.Count - 1; i >= 0; --i) {
                if (optionButtons[i].interactable) {
                    ++interactableCount;
                }
            }
            if (interactableCount < 2) {
                hintButton.interactable = false;
            }
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
        // If background story exists
        if (question.backgroundStory != selectedLevel.defaultBackgroundStory)
        {
            // Give a moment
            yield return new WaitForSeconds(0.2f);
        }
        nextButtonAnimator.SetTrigger("In");
    }
    public void UserSelectHint()
    {
        if (hintButton.interactable)
        {
            for (int i = optionButtons.Count - 1; i >= 0; --i)
            {
                if (i != question.correctIndex && optionButtons[i].interactable)
                {
                    optionButtons[i].interactable = false;
                    break;
                }
            }
            hintButton.interactable = false;
            gameManager.PlaySound("Bell High");
        }
    }
    public void UserSelectNext()
    {
        gameManager.PlaySound("Pop1");
        gameManager.selectedLevelProgress++;
        gameManager.SaveProgress();
        if (gameManager.selectedLevelProgress < selectedLevel.questions.Length)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            if (gameManager.isEndless)
            {
                gameManager.selectedLevelProgress = 0;
            }
            else
            {
                // Complete
                finalQuoteAuthorText.text = "— " + gameManager.selectedLevelName;
                completePanel.SetActive(true);
            }
        }
    }
    public void UserSelectHome()
    {
        gameManager.PlaySound("Pop1");
        Destroy(this);
        SceneManager.LoadScene("Menu");
    }
    public void UserSelectPause()
    {
        gameManager.PlaySound("Pop2");
        isPause = !isPause;
        pausePanelAnimator.SetTrigger(isPause ? "In" : "Out");
    }
}