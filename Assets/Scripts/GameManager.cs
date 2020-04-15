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
    public bool isEndless = false;
    public int selectedLevelIndex = -1;
    public int selectedLevelProgress = 0;
    public string selectedLevelName {
        get {
            return levels[selectedLevelIndex].levelName;
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
                string correctQuestion = question.options[question.correctIndex];
                if (question.options.Length < level.defaultOptionAmount)
                {
                    List<string> tempAuthors = authors.ToList<string>();
                    string[] temp = new string[level.defaultOptionAmount];
                    for (int k = 0; k < level.defaultOptionAmount; k++)
                    {
                        if (question.options.Length > k)
                        {
                            temp[k] = question.options[k];
                        }
                        else
                        {
                            do
                            {
                                int randomIndex = Random.Range(0, tempAuthors.Count);
                                temp[k] = tempAuthors[randomIndex];
                                tempAuthors.RemoveAt(randomIndex);
                            }
                            while (temp[k].Equals(correctQuestion) && tempAuthors.Count > 0);
                        }
                    }
                    question.options = temp;
                }
            }
            levels.Add(level);
        }
    }
    public void SaveProgress() {
        PlayerPrefs.SetInt(selectedLevelName, selectedLevelProgress);
    }
    public void LoadProgress() {
        selectedLevelProgress = PlayerPrefs.GetInt(selectedLevelName, 0);
    }
}