using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private HexagonManager hexagonManager;
    void Start()
    {
        hexagonManager = GetComponent<HexagonManager>();
        hexagonManager.ConstructBoardTiles(); 
    }
    void Update()
    {
       
    }
}