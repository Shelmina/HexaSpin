using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombHexagon : MonoBehaviour
{
    private int clock;
    // Start is called before the first frame update
    void Start()
    {
        clock = 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClockTick()
    {
        clock--;
        if(clock == 0)
        {
            //game over
        }
    }
}
