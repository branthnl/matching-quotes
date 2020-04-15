using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset authorsData;
    [SerializeField]
    private TextAsset[] levelsData;
    public string[] authors;
    public List<Level> levels;
    public Level endlessLevel;
    public bool isEndless = false;
    public int selectedLevelIndex = -1;
    public int selectedLevelProgress = 0;
    public string selectedLevelName
    {
        get
        {
            return isEndless ? endlessLevel.levelName : levels[selectedLevelIndex].levelName;
        }
    }
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            LoadData();
            instance = this;
            DontDestroyOnLoad(this);
            SceneManager.LoadScene("Menu");
        }
        else
        {
            Destroy(this);
        }
    }
    private void LoadData()
    {
        authors = authorsData.text.Trim().Split('\n');
        List<Question> totalQuestions = new List<Question>();
        for (int i = levelsData.Length - 1; i >= 0; --i)
        {
            Level level = JsonUtility.FromJson<Level>(levelsData[i].text);
            for (int j = level.questions.Length - 1; j >= 0; --j)
            {
                Question question = level.questions[j];
                if (question.correctIndex < 1)
                {
                    question.correctIndex = 0;
                }
                if (question.backgroundStory == null)
                {
                    question.backgroundStory = level.defaultBackgroundStory;
                }
                string correctOption = question.options[question.correctIndex];
                if (question.options.Length < level.defaultOptionAmount)
                {
                    List<string> tempAuthors = authors.ToList<string>();
                    tempAuthors.Remove(correctOption);
                    string[] temp = new string[level.defaultOptionAmount];
                    for (int k = 0; k < level.defaultOptionAmount; k++)
                    {
                        // If we have k'th options defined
                        if (question.options.Length > k)
                        {
                            // Copy it
                            temp[k] = question.options[k];
                        }
                        else
                        {
                            int randomIndex = Random.Range(0, tempAuthors.Count);
                            // Otherwise get a random author to pass
                            temp[k] = tempAuthors[randomIndex];
                            // Make sure no similar author in the options
                            tempAuthors.RemoveAt(randomIndex);
                        }
                    }
                    question.options = temp;
                }
                totalQuestions.Add(question);
            }
            levels.Add(level);
        }
        // Shuffle all available question to make endless level
        int totalQuestionsLength = totalQuestions.Count;
        for (int i = 0; i < totalQuestionsLength; ++i)
        {
            var tempValue = totalQuestions[i];
            int randomIndex = Random.Range(0, totalQuestionsLength);
            totalQuestions[i] = totalQuestions[randomIndex];
            totalQuestions[randomIndex] = tempValue;
        }
        endlessLevel = new Level("Endless",
                                 "Great Job! Let's try the next one",
                                 "Nice try, but missed. Let's try again",
                                 3, true, "Under research",
                                 totalQuestions.ToArray());
    }
    public void SaveProgress()
    {
        PlayerPrefs.SetInt(selectedLevelName, selectedLevelProgress);
    }
    public void LoadProgress()
    {
        selectedLevelProgress = PlayerPrefs.GetInt(selectedLevelName, 0);
    }
    public void ResetProgress(string levelName)
    {
        PlayerPrefs.SetInt(levelName, 0);
    }
}