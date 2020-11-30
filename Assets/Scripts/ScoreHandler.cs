using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
    public Text scoreText;
    public Text highscoreText;
    public static ScoreHandler instance = null;
    public int highscore = 0;
    // Start is called before the first frame update
   void Start()
    {
        instance = GetComponent<ScoreHandler>();
        if (!PlayerPrefs.HasKey("Highscore"))
        {
            PlayerPrefs.SetInt("Highscore", highscore);
        }
        else
        {
            highscore = PlayerPrefs.GetInt("Highscore");
        }
        highscoreText.text = highscore.ToString();
        scoreText.text = (0).ToString();
    }
    public void UpdateScore(int totalExploded)
    {
        int temp = int.Parse(scoreText.text.ToString()) + totalExploded;
        scoreText.text = temp.ToString();
        if (temp > highscore)
        {
            highscore = temp;
            highscoreText.text = "Highscore: " +highscore.ToString();
        }
    }
}
