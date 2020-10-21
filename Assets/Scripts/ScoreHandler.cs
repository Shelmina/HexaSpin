using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
    public Text scoreText;
    public static ScoreHandler instance = null;
    // Start is called before the first frame update
   void Start()
    {
        instance = GetComponent<ScoreHandler>();
        scoreText.text = (0).ToString();
    }
    public void UpdateScore(int totalExploded)
    {
        scoreText.text = (int.Parse(scoreText.text.ToString()) + totalExploded).ToString();
    }
}
