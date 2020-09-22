using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal;

public class TouchManager : MonoBehaviour
{
    private HexagonManager hexagonManager;
    public Text testing;
    void Start()
    {
        hexagonManager = GetComponent<HexagonManager>();
    }
    void Update()
    {/*
        if ((Input.touchCount > 0) && Input.touches[0].phase == TouchPhase.Began)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.touches[0].position), Vector2.zero);
            if (hit.collider != null)
            {
                testing.text = hit.transform.name;
            }
        }*/
//for testing on editor
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] hit = Physics2D.CircleCastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.2f, Vector2.zero);
            if (hit.Length == 3)
            {
                hexagonManager.SelectTrio(hit);
                //Debug.Log("koordinat: " + hexagonManager.FindCenter(hit).ToString());
                foreach (var item in hit)
                {
                    if (item.collider.CompareTag("Hexagon"))
                    {
                        testing.text += " /";
                        testing.text += item.transform.name;
                    }
                }
            }
        }
#endif
    }
 
    /*
    public GameObject SelectedHexagon(RaycastHit2D hit)
    {
        
        return hit.collider.gameObject;
    }

    public float CalculateAngle(RaycastHit2D hit, GameObject hextile)
    {
        return (float)Math.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - hit.transform.position.y,
                Camera.main.ScreenToWorldPoint(Input.mousePosition).x - hit.transform.position.x) * 180 / Mathf.PI;
    }*/

}
