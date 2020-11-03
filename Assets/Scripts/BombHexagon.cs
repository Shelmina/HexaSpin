using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BombHexagon : Hexagon
{
    public TextMeshPro clockText;
    private int clock;
    void Start()
    {
        clock = 10;
        clockText.text = clock.ToString();
    }
    public void ClockTick()
    {
        clock--;
        clockText.text = clock.ToString();
        if (clock == 0)
        {
            GameObject temp = this.transform.parent.GetComponent<HexagonManager>().gameoverPanel;
            temp.SetActive(true);
            temp.GetComponentInChildren<Text>().text = "Your Score:" + 1000;
        }
    }
    public void Respawn()
    {
        clock = 10;
        clockText.text = clock.ToString();
        this.gameObject.SetActive(true);
    }
}
