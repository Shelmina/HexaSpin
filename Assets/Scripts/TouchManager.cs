using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngineInternal;

public class TouchManager : MonoBehaviour
{
    private HexagonManager hexagonManager;
    bool selected = false;
    Vector2 initialPosition;
    Vector2 currentPosition;
    RaycastHit2D[] hit;
    void Start()
    {
        hexagonManager = GetComponent<HexagonManager>();
        hexagonManager.Deselect();
    }
    void Update()
    {
        /* if (((Input.touchCount > 0) && Input.touches[0].phase == TouchPhase.Began)&&(selectable))
         {
             selectable = false;
             initialPosition = Input.GetTouch(0).position;
             RaycastHit2D nullchecker = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
             if (nullchecker.collider == null)
             {
                // hexagonManager.Deselect();
             }
             else
             {
                 RaycastHit2D[] hit = Physics2D.CircleCastAll(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), 0.15f, Vector2.zero);
                 if (hexagonManager.IsValidGroup(hit))
                 {
                     hexagonManager.Select(hit);
                     DetectSwipe();
                     //if (swipeDetected)
                     {
                         StartCoroutine(Waiter(hit));
                     }
                 }
             }
             selectable = true;
         }*/

        //for testing on editor
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            initialPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if ((!hexagonManager.rotateDetected)&&(Input.GetMouseButtonUp(0)))
        {
            currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (selected && DetectSwipe(currentPosition))
            {
                StartCoroutine(Waiter(hit, hexagonManager.FindCenter(hit)));
            }
            else { 
                RaycastHit2D nullchecker = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (nullchecker.collider == null && false)
                {
                    hexagonManager.Deselect();
                    selected = false;
                }
                else
                {
                    hit = Physics2D.CircleCastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f, Vector2.zero);
                    if (hexagonManager.IsValidGroup(hit))
                    {
                        hexagonManager.Select(hit);
                        selected = true;
                    }
                }
            }
        }
#endif
    }
    IEnumerator Waiter(RaycastHit2D[] hit, Vector2 rayPoint)
    {
        bool flag = false;
        int columnCount = 0;
        hexagonManager.frame.SetActive(false);
        hexagonManager.centerDot.SetActive(false);
        List<Vector2> columnCoordinates = new List<Vector2>(); ;
        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(hexagonManager.Rotator(hit));
            yield return new WaitForSeconds(1.0f);
            if (hexagonManager.explodeList.Count > 0)
            {
                columnCoordinates = hexagonManager.Explode();
                columnCount = columnCoordinates.Count;
                flag = true;
                break;
            }
        }
        while (flag)
        {
            yield return new WaitForSeconds(0.2f);
            hexagonManager.moveEnded = hexagonManager.moveCounter == columnCount;
            if (hexagonManager.moveEnded)
            {
                hexagonManager.SearchExplosion(Physics2D.RaycastAll(rayPoint, Vector2.zero));
                if (hexagonManager.explodeList.Count > 0)
                {
                    List<Vector2> tempList = hexagonManager.Explode();
                    columnCount = tempList.Count;
                    foreach (var item in tempList)
                    {
                        columnCoordinates.Add(item);
                    }
                }
                else
                {
                    StartCoroutine(hexagonManager.Filler(columnCoordinates));
                    break;
                }
                    
            }
        }
        hexagonManager.rotateDetected = false;
        hexagonManager.frame.SetActive(true);
        hexagonManager.centerDot.SetActive(true);
    }
    public bool DetectSwipe(Vector2 currentPosition) {
        //if (Input.GetTouch(0).phase == TouchPhase.Moved)
        //Debug.Log("initial: " + initialPosition.ToString() + " current:" + currentPosition.ToString());
        if (IsValidSwipe(initialPosition, currentPosition))
        {
            float yDistance = currentPosition.y - initialPosition.y;
            float xDistance = currentPosition.x - initialPosition.x;
            bool positionBool = (Mathf.Abs(xDistance) > Mathf.Abs(yDistance)) ? (((currentPosition.y - hexagonManager.centerDot.transform.position.y) > 0) ? true : false)
                : (((hexagonManager.centerDot.transform.position.x - currentPosition.x) > 0) ? true : false);
            bool directionBool = (Mathf.Abs(xDistance) > Mathf.Abs(yDistance)) ? (xDistance > 0 ? true : false) : (yDistance > 0 ? true : false);
            if (!directionBool)
            {
                positionBool = !positionBool;
            }
            hexagonManager.clockwise = positionBool;
            return true;
        }
        else
            return false;
    }
    //Checks if swipe distance is valid.
    public bool IsValidSwipe(Vector2 initial, Vector2 current)
    {
        if((Mathf.Abs(current.x - initial.x) > 1.5f)||(Mathf.Abs(current.y - initial.y) > 1.5f))
            return true;
        return false;
    }
}