using UnityEngine;
public class TouchManager : MonoBehaviour
{
    private HexagonManager hexagonManager;
    public bool selected = false;
    public GameObject centerDot;
    Vector2 initialPosition;
    Vector2 currentPosition;
    public RaycastHit2D[] hit;
    public static TouchManager instance = null;
    void Start()
    {
        instance = GetComponent<TouchManager>();
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

        if ((hexagonManager.touchAvailable)&&(Input.GetMouseButtonUp(0)))
        {
            currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (selected && DetectSwipe(currentPosition))
            {
                hexagonManager.touchAvailable = false;
                StartCoroutine(hexagonManager.Waiter(hit, centerDot.transform.position));
            }
            else {
                RaycastHit2D nullchecker = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (nullchecker.collider == null)
                {
                    selected = hexagonManager.Deselect();
                }
                else
                {
                    hit = Physics2D.CircleCastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f, Vector2.zero);
                    if (hexagonManager.IsValidGroup(hit))
                    {
                        selected = hexagonManager.Select(hit);
                    }
                }
                hexagonManager.touchAvailable = true;
            }
        }
#endif
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