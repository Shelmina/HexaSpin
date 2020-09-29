using System;
using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal;

public class TouchManager : MonoBehaviour
{
    private HexagonManager hexagonManager;
    private Hexagon hexagon;
    public bool selectable = true;
    void Start()
    {
        hexagonManager = GetComponent<HexagonManager>();
        hexagonManager.Deselect();
    }
    void Update()
    {
        if ((Input.touchCount > 0) && Input.touches[0].phase == TouchPhase.Began)
        {
            RaycastHit2D[] hit = Physics2D.CircleCastAll(Camera.main.ScreenToWorldPoint(Input.touches[0].position), 0.3f, Vector2.zero);
            //hexagonManager.Select(hit);
        }
//for testing on editor
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)&&(selectable))
        {
            selectable = false;
            RaycastHit2D nullchecker = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (nullchecker.collider == null)
            {
                hexagonManager.Deselect();
            }
            else
            {
                RaycastHit2D[] hit = Physics2D.CircleCastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f, Vector2.zero);
                if (hexagonManager.IsValidGroup(hit))
                {
                    hexagonManager.Select(hit);
                    for (int i = 0; i < 3; i++)
                    {
                        hexagonManager.Rotator(hit);
                        if (hexagonManager.explosionDetected)
                        {
                            break;
                        }
                    }
                }
            }
            selectable = true;
        }
#endif
    }
}