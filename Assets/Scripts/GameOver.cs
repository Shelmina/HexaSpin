using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public static GameOver instance = null;
    int score;
    private void Start()
    {
        instance = GetComponent<GameOver>();
    }
    public void EndGame()
    {
        GameObject temp = this.transform.parent.GetComponent<HexagonManager>().gameoverPanel;
        temp.SetActive(true);
        temp.GetComponentInChildren<Text>().text = "Your Score:" + score.ToString();
        //get highscore
    }
}
