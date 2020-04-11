using TMPro;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public string[] lines;
    public TextMeshProUGUI quoteText;

    private void Start() {
        lines = File.ReadAllLines(Application.dataPath + "/Texts/quotes.txt");
        quoteText.text = Application.dataPath;//lines[0];
    }

    private void Update() {
    }
}