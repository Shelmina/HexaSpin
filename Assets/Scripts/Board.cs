using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Color [] colors;
    public GameObject thisBoard;
    public GameObject hexagon;
    GameObject[,] tiles;
    public int quantity;
    public float xPosition;
    public float yPosition;
  
    // Start is called before the first frame update
    void Start()
    {
        CreateHexTiles(xPosition, yPosition);
        ColorHexTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void CreateHexTiles(float xCoord, float yCoord)
        {
        int column = 0;
        int row = 0;
        for (int i = 0; i < quantity; i++)
            {
            //bir tile olusturup rotasyonunu degistiriyorum
            
            tiles[row, column] = (GameObject)Instantiate(hexagon, new Vector2(xCoord, yCoord), Quaternion.identity) as GameObject;
            tiles[row, column].transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            tiles[row, column].transform.parent = thisBoard.transform;
            tiles[row, column].name = "( " + row + ", " + column + ")";
            column++;
            //ayni dongu icerisinde capraz sekilde digerlerini de olusturuyorum
            tiles[row, column] = (GameObject)Instantiate(hexagon, new Vector2(xCoord, yCoord), Quaternion.identity) as GameObject;
            tiles[row, column].transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            tiles[row, column].transform.parent = thisBoard.transform;
            tiles[row, column].name = "( " + row + ", " + column + ")";
            column++;
            xCoord += 1.1f; //ayni siradaki blokların arasındaki x mesafesi
            if (column == 8)
            {
                row++;
                column = 0;
                xCoord = xPosition;
                yCoord += 0.62f; //ayni sutundaki blokların arasındaki y mesafesi
            }
        }
 
        }
    private void ColorHexTiles()
    {

    }
}
