﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HexagonManager : ProtectedClass
{
    public GameObject hexagonPrefab;
    public GameObject centerDot;
    public GameObject frame;
    public GameObject refHex;
    public Color[] colors;
    public bool clockwise;
    public bool moveEnded;
    GameObject[,] tiles;
    GameObject tile;
    private float xPosition;
    private float yPosition;
    private int counter = 0;
    private int rowNum = 0;
    private int colNum = 0;
    public int moveCounter;
    private bool valid = false;
    public bool rotateDetected = false;
    private Hexagon tempHexagon;
    public static HexagonManager instance = null;
    public List<GameObject> explodeList, objectPool;
    private void Awake()
    {
        instance = GetComponent<HexagonManager>();
        tiles = new GameObject[ROW, COLUMN];
        xPosition = GRID_START_POSITION.x;
        yPosition = GRID_START_POSITION.y;
        explodeList = new List<GameObject>();
    }
    public void ConstructBoardTiles()
    {
        for (int i = 0; i < TOTAL_TILE / 2; i++)
        {
            CreateTile();
            yPosition -= HALF_VERTICAL;
            CreateTile();
            yPosition += HALF_VERTICAL;
            if (counter == 8)
            {
                counter = 0;
                rowNum++;
                xPosition = GRID_START_POSITION.x;
                yPosition += VERTICAL_GRID_DISTANCE;
            }
        }
    }
    public void CreateTile()
    {
        tile = (GameObject)Instantiate(hexagonPrefab, new Vector2(xPosition, yPosition), Quaternion.identity) as GameObject;
        tempHexagon = tile.GetComponent<Hexagon>();
        tempHexagon.SetColor(ColorHexTile(tempHexagon));
        tempHexagon.SetParent(this.transform);
        tiles[rowNum, colNum] = tempHexagon.gameObject;
        xPosition += HALF_HORIZONTAL;
        colNum++;
        if (colNum == 8)
            colNum = 0;
        counter++;
    }
    private Color ColorHexTile(Hexagon temphex)
    {
        int rand;
        bool infinity;
        RaycastHit2D[] neighbours = StructToRaycastArray(temphex);
        do {
            infinity = false;
            rand = Random.Range(0, 5);
            if ((neighbours[1].collider != null) && (neighbours[2].collider != null)
             && (colors[rand] == neighbours[1].collider.GetComponent<SpriteRenderer>().color)
             && (colors[rand] == neighbours[2].collider.GetComponent<SpriteRenderer>().color))
            {
                infinity = true;
            }
            if ((neighbours[2].collider != null) && (neighbours[3].collider != null)
              && (colors[rand] == neighbours[2].collider.GetComponent<SpriteRenderer>().color)
              && (colors[rand] == neighbours[3].collider.GetComponent<SpriteRenderer>().color)) {
                infinity = true;
            }
            if ((neighbours[3].collider != null) && (neighbours[4].collider != null)
             && (colors[rand] == neighbours[3].collider.GetComponent<SpriteRenderer>().color)
             && (colors[rand] == neighbours[4].collider.GetComponent<SpriteRenderer>().color))
            {
                infinity = true;
            }
            if ((neighbours[4].collider != null) && (neighbours[5].collider != null)
             && (colors[rand] == neighbours[4].collider.GetComponent<SpriteRenderer>().color)
             && (colors[rand] == neighbours[5].collider.GetComponent<SpriteRenderer>().color))
            {
                infinity = true;
            }
           
        } while (infinity);
        return colors[rand];
    }
    //Select the group of hexagons.
    public bool Select(RaycastHit2D[] hit)
    {
        Vector2 center = FindCenter(hit);
        if (valid)
        {
            centerDot.transform.position = center;
            centerDot.SetActive(true);
            SetHexFrame(hit, center);
            return true;
        }
        return false;
    }
    public Vector2 FindCenter(RaycastHit2D[] hit)
    {
        //Find the center of three hexagons.
        float xPos = 0;
        float yPos = 0;
        if (HexagonManager.instance.IsValidGroup(hit))
        {
            foreach (var item in hit)
            {

                xPos += item.transform.position.x;
                yPos += item.transform.position.y;
            }
            valid = true;
            return new Vector2(xPos / 3, yPos / 3);
        }
        else
            return OUT_OF_CAMERA;

    }
    
    //Deselect and reposition frame and circle.
    public bool Deselect()
    {
        valid = false;
        centerDot.transform.position = OUT_OF_CAMERA;
        centerDot.SetActive(false);
        frame.transform.position = OUT_OF_CAMERA;
        frame.SetActive(false);
        return false;
    }
    //Adjust the rotation of frame for the best fit.
    public void SetHexFrame(RaycastHit2D[] rayhit, Vector2 vec)
    {
        float firstColumn = (float)System.Math.Round(rayhit[0].transform.position.x, 2);
        float secondColumn = (float)System.Math.Round(rayhit[1].transform.position.x, 2); ;
        float thirdColumn = (float)System.Math.Round(rayhit[2].transform.position.x, 2); ;

        if ((Mathf.Approximately(firstColumn, secondColumn) && firstColumn > thirdColumn)
            || (Mathf.Approximately(thirdColumn, secondColumn) && secondColumn > firstColumn)
            || (Mathf.Approximately(firstColumn, thirdColumn) && firstColumn > secondColumn))
        {
            frame.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            frame.transform.rotation = Quaternion.Euler(0f, 0f, 60f);
        }
        frame.transform.position = vec;
        frame.SetActive(true);
    }
    public bool IsValidGroup(RaycastHit2D[] hit)
    {
        if (hit.Length != 3)
            return false;
        foreach (var item in hit)
        {
            if (!item.collider.CompareTag("Hexagon"))
            {
                return false;
            }
        }
        return true;
    }
    //Examines hexagons that are moved recently
    public void SearchExplosion(RaycastHit2D[] hit)
    {
        foreach (var item in hit)
        {
            tempHexagon = item.transform.GetComponent<Hexagon>();
            FillExplodeList(tempHexagon);
        }
    }
    //Fill the list if there is any match for explosion
    public void FillExplodeList(Hexagon item)
    {
        RaycastHit2D[] neighbours = StructToRaycastArray(item);
        Color color = item.GetColor();
        //Compare neighbours towards clockwise direction
        for (int i = 0; i < 6; i++)
        {
            if ((i == 5)) {
                if ((neighbours[0].collider != null) && (neighbours[i].collider != null)
                && (color == neighbours[0].collider.GetComponent<SpriteRenderer>().color)
                && (color == neighbours[i].collider.GetComponent<SpriteRenderer>().color))
                {
                    if (!explodeList.Contains(item.gameObject))
                    {
                        explodeList.Add(item.gameObject);
                    }
                    if (!explodeList.Contains(neighbours[i].transform.gameObject))
                    {
                        explodeList.Add(neighbours[i].transform.gameObject);
                    }
                    if (!explodeList.Contains(neighbours[0].transform.gameObject))
                    {
                        explodeList.Add(neighbours[0].transform.gameObject);
                    }                }
            }
            else if((neighbours[i].collider != null)&&(neighbours[i + 1].collider != null)
                &&(color == neighbours[i].collider.GetComponent<SpriteRenderer>().color)
                &&(color == neighbours[i + 1].collider.GetComponent<SpriteRenderer>().color))
            {
                if (!explodeList.Contains(item.gameObject))
                {
                    explodeList.Add(item.gameObject);
                }
                if (!explodeList.Contains(neighbours[i].transform.gameObject))
                {
                    explodeList.Add(neighbours[i].transform.gameObject);
                }
                if (!explodeList.Contains(neighbours[i + 1].transform.gameObject))
                {
                    explodeList.Add(neighbours[i + 1].transform.gameObject);
                }
            }
        }
    }
    //This is a helper that returns an array of neighbours
    public RaycastHit2D[] StructToRaycastArray(Hexagon item)
    {
        RaycastHit2D[] tempArray = new RaycastHit2D[6];
        Hexagon.NearbyTiles neighbours = item.GetNearbies();
        RaycastHit2D up = Physics2D.Raycast(neighbours.up, Vector2.zero);
        RaycastHit2D down = Physics2D.Raycast(neighbours.down, Vector2.zero);
        RaycastHit2D upleft = Physics2D.Raycast(neighbours.upleft, Vector2.zero);
        RaycastHit2D upright = Physics2D.Raycast(neighbours.upright, Vector2.zero);
        RaycastHit2D downleft = Physics2D.Raycast(neighbours.downleft, Vector2.zero);
        RaycastHit2D downright = Physics2D.Raycast(neighbours.downright, Vector2.zero);
        tempArray[0] = up;
        tempArray[1] = upright;
        tempArray[2] = downright;
        tempArray[3] = down;
        tempArray[4] = downleft;
        tempArray[5] = upleft;
        return tempArray;
    }
    //Replacement will be added. This function just for testing.
    List<Vector2> Explode()
    {
        moveEnded = false;
        moveCounter = 0;
        List<Vector2> linecastList = FillLinecast();
        foreach (var item in explodeList)
        {
            objectPool.Add(item);
            item.transform.position = OUT_OF_CAMERA;
            item.SetActive(false);
        }
        ScoreHandler.instance.UpdateScore(explodeList.Count * SCORE_MULTIPLIER);
        explodeList.Clear();
        foreach (var item in linecastList)
        {
            StartCoroutine(MoveObjects(FindLinecastArray(item)));
        }
        return linecastList;
    }
    public List<Vector2> FillLinecast()
    {
        List<Vector2> tempList = new List<Vector2>();
        foreach (var item in explodeList)
        {
            bool add = true;
            for (int i = 0; i < tempList.Count; i++)
            {
                if (Mathf.Approximately(tempList[i].x, item.transform.position.x))
                    add = false;
            }
            if (add)
                tempList.Add(item.transform.position);
        }
        return tempList;
    }
    //Linecasts positive y direction from given point.
    public RaycastHit2D[] FindLinecastArray(Vector2 pos)
    {
        RaycastHit2D[] foundElements = Physics2D.LinecastAll(pos - new Vector2(0f, VERTICAL_GRID_DISTANCE), pos + new Vector2(0f, 6f));
        return foundElements;
    }
    IEnumerator MoveObjects(RaycastHit2D[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            TweenMove(arr[i].collider.gameObject);
            yield return new WaitForSeconds(WAIT_THRESHOLD);
            arr[i].transform.position = new Vector2((float)System.Math.Round(arr[i].transform.position.x, 2), (float)System.Math.Round(arr[i].transform.position.y, 2));
        }
        moveCounter++;
    }
    public void TweenMove(GameObject gameObj)
    {
        int steps = StepToFall(gameObj);
        if (steps > 0)
        {
            gameObj.transform.DOMoveY(gameObj.transform.position.y - steps * VERTICAL_GRID_DISTANCE, WAIT_THRESHOLD);
        }
    }
    public int StepToFall(GameObject item)
    {
        Vector2 down = new Vector2(item.transform.position.x, item.transform.position.y - VERTICAL_GRID_DISTANCE);
        RaycastHit2D ray;
        int steps = 0;
        while (down.y > -0.36f)
        {
            ray = Physics2D.Raycast(down, Vector2.zero);
            if (ray.collider == null)
            {
                down -= new Vector2(0f, VERTICAL_GRID_DISTANCE);
                steps++;
            }
            else
            {
                break;
            }
        }
        return steps;
    }
    //Call the coroutine for all objects that.
    IEnumerator Rotator(RaycastHit2D[] rayhit)
    {
        rotateDetected = true;
        StartCoroutine(RotateFunction(rayhit[0].collider.gameObject, centerDot));
        StartCoroutine(RotateFunction(rayhit[1].collider.gameObject, centerDot));
        StartCoroutine(RotateFunction(rayhit[2].collider.gameObject, centerDot));
        StartCoroutine(RotateFunction(frame, centerDot, rayhit));
        yield return null;
    }
    //Animating the rotation of hexagons. It is called 4 times for rotation of a group.
    IEnumerator RotateFunction(GameObject hexobject, GameObject circ, RaycastHit2D[] hit = null)
    {
        float rotateThreshold = 0.01f;
        int numberOfIterations = Mathf.RoundToInt(TIME_TO_ROTATE / rotateThreshold);
        float rotateAngle = 120 / (TIME_TO_ROTATE / rotateThreshold);
        Vector3 rotateVector = clockwise ? (new Vector3(0, 0, -120)) : (new Vector3(0, 0, 120));
        for (int i = 0; i < numberOfIterations; ++i)
        {
            hexobject.transform.RotateAround(circ.transform.position, rotateVector, rotateAngle);
            yield return new WaitForSeconds(rotateThreshold);
        }
        if (hexobject.CompareTag("Hexagon"))
        {
            hexobject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        if (hit != null)
        {
            SearchExplosion(hit);
        }
    }
    IEnumerator Filler(List<Vector2> vecList, Vector2 rayPoint) {
        foreach (var pos in vecList)
        {
            float yPos = pos.y;
            RaycastHit2D ray;
            bool first = true;
            Vector2 reference = OUT_OF_CAMERA;
            int emptySpots = 0;
            while (yPos < 5.7f)
            {
                ray = Physics2D.Raycast(new Vector2(pos.x, yPos), Vector2.zero);
                if (ray.collider == null)
                {
                    if (first)
                    {
                        reference = new Vector2(pos.x, yPos);
                        first = false;
                    }
                    emptySpots++;
                    yPos += VERTICAL_GRID_DISTANCE;
                }
                else
                    yPos += VERTICAL_GRID_DISTANCE;
            }
            StartCoroutine(FillColumn(emptySpots, reference));
            yield return new WaitForSeconds(emptySpots * WAIT_THRESHOLD + 0.1f);
        }
        RaycastHit2D[] rayArray = Physics2D.CircleCastAll(rayPoint, 0.15f, Vector2.zero);
        TouchManager.instance.selected = Select(rayArray);
        TouchManager.instance.hit = rayArray;
        yield return null;
    }
    IEnumerator FillColumn(int emptySpots, Vector2 reference)
    {
        GameObject gameObj;  
        for (int i = 0; i < emptySpots; i++)
        {
            refHex.transform.position = reference;
            gameObj = objectPool.Count > 0 ? objectPool[0] : null;
            objectPool.Remove(gameObj);
            gameObj.SetActive(true);
            gameObj.GetComponent<Hexagon>().SetColor(ColorHexTile(refHex.GetComponent<Hexagon>()));
            gameObj.transform.position = reference + new Vector2(0f, 10f);
            gameObj.transform.DOMoveY(reference.y, WAIT_THRESHOLD);
            reference += new Vector2(0f, VERTICAL_GRID_DISTANCE); //Move to the upper spot
            yield return new WaitForSeconds(WAIT_THRESHOLD);
        }
    }
    public IEnumerator Waiter(RaycastHit2D[] hit, Vector2 rayPoint)
    {
        bool flag = false;
        bool noExplosion;
        int columnCount = 0;
        List<Vector2> columnCoordinates = new List<Vector2>();
        frame.SetActive(false);
        centerDot.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(Rotator(hit));
            yield return new WaitForSeconds(TIME_TO_ROTATE + 0.1f);
            ////Normalize the objects to prevent bugs
            hit[0].transform.position = new Vector2((float)System.Math.Round(hit[0].transform.position.x, 2), (float)System.Math.Round(hit[0].transform.position.y, 2));
            hit[1].transform.position = new Vector2((float)System.Math.Round(hit[1].transform.position.x, 2), (float)System.Math.Round(hit[1].transform.position.y, 2));
            hit[2].transform.position = new Vector2((float)System.Math.Round(hit[2].transform.position.x, 2), (float)System.Math.Round(hit[2].transform.position.y, 2));
            if (explodeList.Count > 0)
            {
                ///bomb controller
                columnCoordinates = Explode();
                columnCount = columnCoordinates.Count;
                flag = true;
                break;
            }
        }
        while (flag)
        {
            yield return new WaitForSeconds(0.2f);
            moveEnded = moveCounter == columnCount;
            if (moveEnded)
            {
                noExplosion = true;
                foreach (var item in columnCoordinates)
                {
                    RaycastHit2D[] ray = Physics2D.LinecastAll(item - new Vector2(0f, VERTICAL_GRID_DISTANCE), item + new Vector2(0f, 6f));
                    SearchExplosion(ray);
                    if (explodeList.Count > 0)
                    {
                        noExplosion = false;
                        List<Vector2> tempList = Explode();
                        columnCount = tempList.Count;
                        foreach (var temp in tempList)
                        {
                            columnCoordinates.Add(temp);
                        }
                        columnCoordinates.Remove(item);
                        break;
                    }
                }
                if (noExplosion)
                    break;
            }
        }
        StartCoroutine(Filler(columnCoordinates, rayPoint));
        rotateDetected = false;
    }
}