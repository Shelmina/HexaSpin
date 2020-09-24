using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonManager : MonoBehaviour
{
    public GameObject circle;
    public GameObject hexagon;
    public GameObject frame;
    public Color[] colors;
    public float xPosition;
    public float yPosition;
    public int difficulty;
    public Board gameBoard;
    GameObject[,] tiles;
    int row = 0;
    int column = 0;
    float xReminder;
    bool valid = false;


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
        tiles[row, column].name = row + ", " + column;
        tiles[row, column].transform.parent = gameBoard.transform;
    }
    //Color hexagons when the game started.
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
    //Find the center of three hexagons.
    public Vector2 FindCenter(RaycastHit2D[] hit)
    {
        float xPos = 0;
        float yPos = 0;
        foreach (var item in hit)
        {
            if (item.collider.CompareTag("Hexagon"))
            {
                xPos += item.transform.position.x;
                yPos += item.transform.position.y;
            }
            else
            {
                valid = false;
                return new Vector2(-10f, 0f); //Not selected hexagons.
            }
        }
        valid = true;
        return new Vector2(xPos / 3, yPos / 3);
    }
    //Select the group of hexagons.
    public void Select(RaycastHit2D[] hit)
    {
        Vector2 center = FindCenter(hit);
        if (valid)
        {
            circle.transform.position = center;
            circle.SetActive(true);
            HexFrame(hit, center);
        }
    }
    //Deselect and reposition frame and circle.
    public void Deselect()
    {
        valid = false;
        circle.transform.position = new Vector2(-10f, 0f);
        circle.SetActive(false);
        frame.transform.position = new Vector2(-10f, 0f);
        frame.SetActive(false);
    }
    public void HexFrame(RaycastHit2D[] rayhit, Vector2 vec)
    {
        double firstColumn = System.Char.GetNumericValue(rayhit[0].collider.name[3]);
        double secondColumn = System.Char.GetNumericValue(rayhit[1].collider.name[3]);
        double thirdColumn = System.Char.GetNumericValue(rayhit[2].collider.name[3]);

        if ((firstColumn == secondColumn && firstColumn > thirdColumn)
            || (secondColumn == thirdColumn && secondColumn > firstColumn)
            || (firstColumn == thirdColumn && firstColumn > secondColumn))
        {
            frame.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            frame.transform.rotation = Quaternion.Euler(0f, 0f, 60f);
        }
        frame.transform.position = vec;
        frame.SetActive(true);
        Rotator(rayhit);
    }
    //Call the coroutine for all.
    public void Rotator(RaycastHit2D[] rayhit){
        for (int i = 0; i < 3; i++)
        {
            if (true)
            {
                StartCoroutine(RotateFunction(rayhit[0].collider.gameObject, circle));
                StartCoroutine(RotateFunction(rayhit[1].collider.gameObject, circle));
                StartCoroutine(RotateFunction(rayhit[2].collider.gameObject, circle));
                StartCoroutine(RotateFunction(frame, circle));
            }
        }
    }
    //Animating the rotation of hexagons. It is called 4 times for a rotation.
    IEnumerator RotateFunction(GameObject hexobject, GameObject circ)
    {
        float timeToRotate = 0.25f;
        float rotateThreshold = 0.01f; //Pick a number that divides timeToRotate perfectly.
        int numberOfIterations = Mathf.RoundToInt(timeToRotate / rotateThreshold);
        float rotateAngle = 120 / (timeToRotate / rotateThreshold);

        for (int i = 0; i < numberOfIterations; ++i)
        {
            hexobject.transform.RotateAround(circ.transform.position, new Vector3(0, 0, 120), rotateAngle);
            yield return new WaitForSeconds(rotateThreshold);
        }
        if (hexobject.CompareTag("Hexagon"))
        {
            hexobject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
