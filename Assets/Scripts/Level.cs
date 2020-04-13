[System.Serializable]
public class Level {
    public string levelName;
    public string correctResponse;
    public string incorrectResponse;
    public int defaultOptionAmount;
    public bool shuffleOptionPosition;
    public string defaultBackgroundStory;
    public Question[] questions;
    public Level(string levelName, string correctResponse, string incorrectResponse, int defaultOptionAmount, bool shuffleOptionPosition, string defaultBackgroundStory, Question[] questions) {
        this.levelName = levelName;
        this.correctResponse = correctResponse;
        this.incorrectResponse = incorrectResponse;
        this.defaultOptionAmount = defaultOptionAmount;
        this.shuffleOptionPosition = shuffleOptionPosition;
        this.defaultBackgroundStory = defaultBackgroundStory;
        this.questions = questions;
    }
}