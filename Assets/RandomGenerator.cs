using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator : MonoBehaviour {
    public float xLow, xHigh, yLow, yHigh;
    public int numberOfObstacles;
    public GameObject obstacle;
    public GameObject destination;
	// Use this for initialization
	void Awake() {

        for (int i = 0; i < numberOfObstacles; i++)
        {
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
