using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditorInternal;

public class HexagonManager : ProtectedClass
{
    public GameObject hexagonPrefab;
    public GameObject centerDot;
    public GameObject frame;
    public GameObject parentObject;
    public Color[] colors;
    public bool clockwise;
    GameObject[,] tiles;
    GameObject tile;
    private float xPosition;
    private float yPosition;
    private int counter = 0;
    private int rowNum = 0;
    private int colNum = 0;
    private bool valid = false;
    public bool explosionDetected = false;
    private Hexagon tempHexagon;
    public static HexagonManager instance = null;
    List<GameObject> explodeList;
    Vector2 center;
    Vector3 rotateVector;
    Vector3 zeroVector;
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
        tempHexagon.SetColor(ColorHexTile());
        tempHexagon.SetName(rowNum + ", " + colNum);
        tempHexagon.SetParent(this.transform);
        tiles[rowNum, colNum] = tempHexagon.gameObject;
        xPosition += HALF_HORIZONTAL;
        colNum++;
        if (colNum == 8)
            colNum = 0;
        counter++;
    }
    private Color ColorHexTile()
    {
        int total, rand;
        do {
            rand = Random.Range(0, 5);
            total = 0;
            //Check down-left when odd
            if ((rowNum > 0) && (colNum % 2 == 1) && (colors[rand] == tiles[rowNum - 1, colNum - 1].GetComponent<SpriteRenderer>().color))
                total++;
            //Check down-left when even
            else if ((rowNum > 0) && (colNum > 0) && (colNum % 2 == 0) && (colors[rand] == tiles[rowNum - 1, colNum].GetComponent<SpriteRenderer>().color))
                total++;
            //Check down
            if ((rowNum > 0) && (colors[rand] == tiles[rowNum - 1, colNum].GetComponent<SpriteRenderer>().color))
                total++;
            if ((rowNum > 0) && (colNum < 7) && (colNum % 2 == 1))
            {
                //Check down-right when odd
                if (colors[rand] == tiles[rowNum - 1, colNum + 1].GetComponent<SpriteRenderer>().color)
                    total++;
                //Check up-left when odd
                if (colors[rand] == tiles[rowNum, colNum - 1].GetComponent<SpriteRenderer>().color)
                    total++;
            }
        } while (total > 1);
        return colors[rand];
    }
   
    public void Select(RaycastHit2D[] hit)
    {
        center = FindCenter(hit);
        if (valid)
        {
            centerDot.transform.position = center;
            centerDot.SetActive(true);
            SetHexFrame(hit, center);
        }
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
    //Select the group of hexagons.
    //Deselect and reposition frame and circle.
    public void Deselect()
    {
        valid = false;
        centerDot.transform.position = OUT_OF_CAMERA;
        centerDot.SetActive(false);
        frame.transform.position = OUT_OF_CAMERA;
        frame.SetActive(false);
    }
    //Adjust the rotation of frame for the best fit.
    public void SetHexFrame(RaycastHit2D[] rayhit, Vector2 vec)
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
        }
    public void RotateTween(RaycastHit2D[] rayhit)
    {
        parentObject.transform.position = center;
        foreach (var item in rayhit)
        {
            item.transform.parent = parentObject.transform;
        }
        Sequence mySequence = DOTween.Sequence();
        frame.transform.parent = parentObject.transform;
        rotateVector = clockwise ? (new Vector3(0, 0, 120)) : (new Vector3(0, 0, -120));
        mySequence.Append(parentObject.transform.DORotate(rotateVector, 1)
            .OnComplete(()=> {if (explosionDetected)
                    mySequence.Kill();
            }).SetLoops(3));
       
      
        zeroVector = new Vector3(0f, 0f, 0f);
        zeroVector += rotateVector;
        parentObject.transform.DORotate(rotateVector, 1).OnStepComplete(() => SearchExplosion(rayhit)).SetLoops(3);
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
                    explosionDetected = true;
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
                explosionDetected = true;
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
    public void Explode()
    {
        foreach (var item in explodeList)
        {
            Destroy(item);
        }
        explosionDetected = false;
    }
    //Call the coroutine for all objects that.
    public IEnumerator Rotator(RaycastHit2D[] rayhit)
    {
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


}