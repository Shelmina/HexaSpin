using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal;

public class TouchManager : MonoBehaviour
{
    public Text testing;
    void Start()
    {
        
    }
    void Update()
    {
        if ((Input.touchCount > 0) && Input.touches[0].phase == TouchPhase.Began)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.touches[0].position), Vector2.zero);
            if (hit.collider != null)
            {
                testing.text = hit.transform.name;
            }
        }
//for testing on editor
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D rayhit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if ((rayhit.collider != null)&&(rayhit.collider.tag == "Hexagon"))
            {
                testing.text = SelectedHexagon(rayhit).transform.name + " angle: ";
                testing.text += CalculateAngle(rayhit, SelectedHexagon(rayhit));
            }
        }
#endif
    }

    public GameObject SelectedHexagon(RaycastHit2D hit)
    {
        
        return hit.collider.gameObject;
    }

    public float CalculateAngle(RaycastHit2D hit, GameObject hextile)
    {
        return (float)Math.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - hit.transform.position.y,
                Camera.main.ScreenToWorldPoint(Input.mousePosition).x - hit.transform.position.x) * 180 / Mathf.PI;
    }

}
