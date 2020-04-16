[System.Serializable]
public class Level
{
    public string levelName;
    public string correctResponse;
    public string incorrectResponse;
    public string completionResponse;
    public string completionQuote;
    public string outOfChanceResponse;
    public string outOfChanceQuote;
    public int defaultOptionAmount;
    public bool shuffleOptionPosition;
    public string defaultBackgroundStory;
    public Question[] questions;
    public Level(string levelName, string correctResponse, string incorrectResponse, string completionResponse, string completionQuote, string outOfChanceResponse, string outOfChanceQuote, int defaultOptionAmount, bool shuffleOptionPosition, string defaultBackgroundStory, Question[] questions)
    {
        this.levelName = levelName;
        this.correctResponse = correctResponse;
        this.incorrectResponse = incorrectResponse;
        this.completionResponse = completionResponse;
        this.completionQuote = completionQuote;
        this.outOfChanceResponse = outOfChanceResponse;
        this.outOfChanceQuote = outOfChanceQuote;
        this.defaultOptionAmount = defaultOptionAmount;
        this.shuffleOptionPosition = shuffleOptionPosition;
        this.defaultBackgroundStory = defaultBackgroundStory;
        this.questions = questions;
    }
}