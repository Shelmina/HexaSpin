using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This is the class that constant values preserved
public class ProtectedClass : MonoBehaviour
{
    protected float HORIZONTAL_GRID_DISTANCE = 1.2f;
    protected float VERTICAL_GRID_DISTANCE = 0.7f;
    protected float HALF_HORIZONTAL = 0.6f;
    protected float HALF_VERTICAL = 0.35f;
    protected float TIME_TO_ROTATE = 0.25f;
    protected int TOTAL_TILE = 72;
    protected int ROW = 9;
    protected int COLUMN = 8;
    protected readonly Vector2 OUT_OF_CAMERA = new Vector2(-10f, 0f); //For dot and frame to reset their positions.
    protected readonly Vector2 GRID_START_POSITION = new Vector2(0f, 0f);
}
