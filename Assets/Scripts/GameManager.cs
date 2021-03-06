﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct MyAudioClip
{
    public string key;
    public AudioClip clip;
}

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
    public bool soundMute;
    [SerializeField]
    private AudioSource seAudioSource;
    [SerializeField]
    private MyAudioClip[] audioClips;
    private Dictionary<string, AudioClip> audioClipsDict;
    private AudioSource[] myAudioSources;
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
    private void Start()
    {
        audioClipsDict = new Dictionary<string, AudioClip>();
        for (int i = 0; i < audioClips.Length; ++i)
        {
            audioClipsDict.Add(audioClips[i].key, audioClips[i].clip);
        }
        myAudioSources = GetComponents<AudioSource>();
        soundMute = false;
        if (PlayerPrefs.HasKey("Mute"))
        {
            soundMute = PlayerPrefs.GetInt("Mute", 0) == 0;
        }
        // Because we want to use UserTriggerSoundSetting()
        soundMute = !soundMute;
        UserTriggerSoundSetting();
    }
    public void UserTriggerSoundSetting()
    {
        soundMute = !soundMute;
        foreach (AudioSource a in myAudioSources)
        {
            a.mute = soundMute;
        }
        PlayerPrefs.SetInt("Mute", soundMute ? 0 : 1);
    }
    public void PlaySound(string audioClipKey)
    {
        if (audioClipsDict.ContainsKey(audioClipKey))
        {
            seAudioSource.PlayOneShot(audioClipsDict[audioClipKey]);
        }
    }
    private void LoadData()
    {
        authors = authorsData.text.Trim().Split('\n');
        List<Question> totalQuestions = new List<Question>();
        for (int i = 0; i < levelsData.Length; ++i)
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
                            do
                            {
                                // Otherwise get a random author to pass
                                int randomIndex = Random.Range(0, tempAuthors.Count);
                                string randomAuthor = tempAuthors[randomIndex];
                                temp[k] = randomAuthor;
                                // Make sure no similar author in the options
                                tempAuthors.RemoveAt(randomIndex);
                            }
                            while (temp[k].Trim().ToLower().Equals(correctOption.Trim().ToLower()) && tempAuthors.Count > 0);
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
                                 "Congratulations!",
                                 "You have just completed this level.",
                                 "Good game.",
                                 "You are out of chance.",
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