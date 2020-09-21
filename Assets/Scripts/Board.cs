using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    private HexTile hexTile;
    public Text testing;
    void Start()
    {
        hexTile = GetComponent<HexTile>();
        hexTile.ConstructBoardTiles(); 
    }
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
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.touches[0].position), Vector2.zero);
            if (hit.collider != null)
            {
                testing.text = hit.transform.name;
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
}


