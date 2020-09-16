using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int difficulty = 3;
    public Color [] colors;
    public GameObject thisBoard;
    public GameObject hexagon;
    GameObject[,] tiles;
    public int quantity;
    public float xPosition;
    public float yPosition;
    int column = 0;
    int row = 0;
    int rand;
  
    // Start is called before the first frame update
    void Start()
    {
        tiles = new GameObject[quantity / 4, 8];
        CreateHexTiles(xPosition, yPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void CreateHexTiles(float xCoord, float yCoord)
        {
        for (int i = 0; i < quantity*2; i++)
            {
            CreateTile(xCoord, yCoord);
            ColorHexTiles(row, column);
            column += 2;
            xCoord += 1.1f;
            if (column == 8)
            {
                column = 1;
                xCoord = xPosition + 0.55f;
                yCoord += 0.31f;
            }
            if (column == 9)
            {
                row++;
                xCoord = xPosition;
                yCoord += 0.31f;
                column = 0;
            }
        }
 
        }
    private void CreateTile(float xPos, float yPos) {
        tiles[row, column] = (GameObject)Instantiate(hexagon, new Vector2(xPos, yPos), Quaternion.identity) as GameObject;
        tiles[row, column].transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        tiles[row, column].transform.parent = thisBoard.transform;
        tiles[row, column].name = "( " + row + ", " + column + ")";
    }
    private void ColorHexTiles(int x, int y)
    {
        rand = Random.Range(0, difficulty);
        if ((x == 0)||(y == 0))
        {
            tiles[x, y].GetComponent<SpriteRenderer>().color = colors[rand];
            return;
        }
        
        else
        {
        if (y == 7)
        {
            while (tiles[x, y - 1].GetComponent<SpriteRenderer>().color == tiles[x - 1, y].GetComponent<SpriteRenderer>().color)
            {
                rand = Random.Range(0, difficulty);
            }
            tiles[x, y].GetComponent<SpriteRenderer>().color = colors[rand];
            return;
        }
        if (y % 2 == 1)
        {
            while ((tiles[x, y - 1].GetComponent<SpriteRenderer>().color == tiles[x - 1, y].GetComponent<SpriteRenderer>().color)
            || (tiles[x - 1, y].GetComponent<SpriteRenderer>().color == tiles[x, y + 1].GetComponent<SpriteRenderer>().color))
            {
                rand = Random.Range(0, difficulty);
            }
        }
        else
        {
            while ((tiles[x - 1, y - 1].GetComponent<SpriteRenderer>().color == tiles[x - 1, y].GetComponent<SpriteRenderer>().color)
            || (tiles[x - 1, y].GetComponent<SpriteRenderer>().color == tiles[x - 1, y + 1].GetComponent<SpriteRenderer>().color))
            {
                rand = Random.Range(0, difficulty);
            }
        }
        tiles[x, y].GetComponent<SpriteRenderer>().color = colors[rand];
        return;
        }
    }
}