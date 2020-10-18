using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : ProtectedClass
{
    public struct NearbyTiles
    {
        public Vector2 up;
        public Vector2 down;
        public Vector2 upleft;
        public Vector2 upright;
        public Vector2 downleft;
        public Vector2 downright;
    }
    //Returns a struct for possible neighbours' coordinates
    public NearbyTiles GetNearbies()
    {
        NearbyTiles nearbies;
        float xPos = transform.position.x;
        float yPos = transform.position.y;
        nearbies.up = new Vector2(xPos, yPos + VERTICAL_GRID_DISTANCE);
        nearbies.down = new Vector2(xPos, yPos - VERTICAL_GRID_DISTANCE);
        nearbies.upleft = new Vector2(xPos - HALF_HORIZONTAL, yPos + HALF_VERTICAL);
        nearbies.upright = new Vector2(xPos + HALF_HORIZONTAL, yPos + HALF_VERTICAL);
        nearbies.downleft = new Vector2(xPos - HALF_HORIZONTAL, yPos - HALF_VERTICAL);
        nearbies.downright = new Vector2(xPos + HALF_HORIZONTAL, yPos - HALF_VERTICAL);
        return nearbies;
    }
    //Checks if given coordinates are valid for possible hexagon positions
    public bool IsValidHexagon(Vector2 vec)
    {
        if((vec.x < 0f) || (vec.y < 0f) || (vec.x > 4.4f) || (vec.y > 6f))
        {
            return false;
        }
        return true;
    }
    public Color GetColor()
    {
        return this.GetComponent<SpriteRenderer>().color;
    }
    public void SetColor(Color col)
    {
        this.GetComponent<SpriteRenderer>().color = col;
    }
    public void SetParent(Transform trans)
    {
        this.transform.parent = trans;
    }
}