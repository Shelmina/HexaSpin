using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonManager : MonoBehaviour
{
    public GameObject hexagon;
    public Color[] colors;
    public float xPosition;
    public float yPosition;
    public int difficulty;
    public Board gameBoard;
    GameObject[,] tiles;
    int row = 0;
    int column = 0;
    float xReminder;

    // Start is called before the first frame update
    private void Awake()
    {
        xReminder = xPosition;
        tiles = new GameObject[9, 8];
    }
    public void ConstructBoardTiles()
    {
        for (int i = 0; i < 72; i++)
        {
            CreateTile();
            ColorHexTiles();
            column += 2;
            xPosition += 1.1f;
            if (column == 8)
            {
                column = 1;
                xPosition = xReminder + 0.55f;
                yPosition += 0.31f;
            }
            if (column == 9)
            {
                row++;
                xPosition = xReminder; //Moving to upper row
                yPosition += 0.31f;
                column = 0;
            }
        }
    }
    private void CreateTile()
    {
        tiles[row, column] = (GameObject)Instantiate(hexagon, new Vector2(xPosition, yPosition), Quaternion.identity) as GameObject;
        tiles[row, column].name = "( " + row + ", " + column + ")";
        tiles[row, column].transform.parent = gameBoard.transform;
        Debug.Log("row: " + row + " column:" + column);
    }
    private void ColorHexTiles()
    {
        int counter, rand;
        do
        {
            rand = Random.Range(0, difficulty);
            counter = 0;
            //sol alt tek
            if (column > 0 && (column % 2) == 1 && (colors[rand] == tiles[row, column - 1].GetComponent<SpriteRenderer>().color))
                counter++;
            //sol alt cift
            else if (column > 0 && (column % 2) == 0 && row > 0 && (colors[rand] == tiles[row - 1, column - 1].GetComponent<SpriteRenderer>().color))
                counter++;
            //alt
            if (row > 0 && (colors[rand] == tiles[row - 1, column].GetComponent<SpriteRenderer>().color))
                counter++;
            //sag alt tek
            if (column < 7 && row > 0 && (column % 2 == 1) && (colors[rand] == tiles[row, column + 1].GetComponent<SpriteRenderer>().color))
                counter++;
            else if (column < 7 && row > 0 && (column % 2 == 0) && (colors[rand] == tiles[row - 1, column + 1].GetComponent<SpriteRenderer>().color))
                counter++;
        } while (counter > 1);
        tiles[row, column].GetComponent<SpriteRenderer>().color = colors[rand];
    }
}
