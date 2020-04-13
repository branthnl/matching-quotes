using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public bool isPause, isAnswered;
    public Question question;
    private Level selectedLevel;
    private Animator pausePanelAnimator;
    public static LevelManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start() {
        selectedLevel = GameManager.instance.levels[GameManager.instance.selectedLevelIndex];
        Setup();
    }
    private void Setup() {
        isAnswered = false;
        question = selectedLevel.questions[GameManager.instance.selectedLevelProgress];
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isPause) {
                UserSelectResume();
            }
            else {
                UserSelectPause();
            }
        }
    }
    public void UserAnswer(int answerIndex) {
        if (answerIndex == question.correctIndex) {
            Debug.Log("CORRECT");
        }
        else {
            Debug.Log("INCORRECT");
        }
    }
    public void UserSelectNext() {
        GameManager.instance.selectedLevelProgress++;
        if (GameManager.instance.selectedLevelProgress < selectedLevel.questions.Length) {
            Setup();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else {
            // Complete
        }
    }
    public void UserSelectPause() {
        isPause = true;
        pausePanelAnimator.SetBool("isPause", true);
    }
    public void UserSelectResume() {
        isPause = false;
        pausePanelAnimator.SetBool("isPause", false);
    }
    public void UserSelectHome() {
        SceneManager.LoadScene("Menu");
    }
}