using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public int difficulty = 3;
    public Color[] colors;
    public GameObject thisBoard;
    public GameObject hexagon;
    GameObject[,] tiles;
    public int quantity;
    public float xPosition;
    public float yPosition;
    public Text testing;
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
        /*if (Input.touchCount > 0)
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            //print(Input.touchCount);
            testing.text = touchPos.ToString();
        }*/
        if((Input.touchCount > 0) && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider != null)
                {
                    Color newColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                    hit.collider.GetComponent<MeshRenderer>().material.color = newColor;
                    hit.transform.position = new Vector2(0f, 0f);
                    testing.text = Input.touchCount.ToString();
                }
            }
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            // Ray2D ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            /*RaycastHit2D hit;

            if (hit = Physics2D.Raycast(Camera.main.transform.position, Input.mousePosition))

                    //Raycast(ray, Vector2.zero, out hit))
            {*/
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    testing.text = hit.transform.name;
                }
            
        }
#endif
    }
    private void CreateHexTiles(float xCoord, float yCoord)
    {
        for (int i = 0; i < quantity * 2; i++)
        {
            CreateTile(xCoord, yCoord);
            tiles[row, column].name = "( " + row + ", " + column + ")";
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
    private void CreateTile(float xPos, float yPos)
    {
        tiles[row, column] = (GameObject)Instantiate(hexagon, new Vector2(xPos, yPos), Quaternion.identity) as GameObject;
        //tiles[row, column].transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        tiles[row, column].transform.parent = thisBoard.transform;
    }
    private void ColorHexTiles(int x, int y)
    {
        int counter;
        do
        {
            rand = Random.Range(0, difficulty);
            counter = 0;
            //sol alt tek
            if (y > 0 && (y % 2) == 1 && (colors[rand] == tiles[x, y - 1].GetComponent<SpriteRenderer>().color))
                counter++;
            //sol alt cift
            else if (y > 0 && (y % 2) == 0 && x > 0 && (colors[rand] == tiles[x - 1, y - 1].GetComponent<SpriteRenderer>().color))
                counter++;
            //alt
            if (x > 0 && (colors[rand] == tiles[x - 1, y].GetComponent<SpriteRenderer>().color))
                counter++;
            //sag alt tek
            if (y < 7 && x > 0 && (y % 2 == 1) && (colors[rand] == tiles[x, y + 1].GetComponent<SpriteRenderer>().color))
                counter++;
            else if (y < 7 && x > 0 && (y % 2 == 0) && (colors[rand] == tiles[x - 1, y + 1].GetComponent<SpriteRenderer>().color))
                counter++;
        } while (counter >= 2);
        tiles[x, y].GetComponent<SpriteRenderer>().color = colors[rand];
    }
}


