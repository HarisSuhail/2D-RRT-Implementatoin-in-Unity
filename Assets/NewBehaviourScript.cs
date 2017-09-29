using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    // Use this for initialization
    public float xLow, xHigh, yLow, yHigh, zLevel, OpenAreaAroundDestination;
    public int numberOfObstacles;
    public GameObject obstacle;
    public GameObject destination;
    // Use this for initialization
    void Awake()
    {

        for (int i = 0; i < numberOfObstacles; i++)
        {
            Vector3 h = new Vector3(Random.Range(xLow, xHigh), Random.Range(yLow, yHigh), zLevel);
            Instantiate(obstacle, h, Quaternion.identity);
        }

        bool done = false;
        int count = 0;
        while (!done)
        {
            count++;
            if (count > 100)
            {
                Debug.Log("Could not place destination");
                return;
            }
            Vector3 h = new Vector3(Random.Range(xLow, xHigh), Random.Range(yLow, yHigh), zLevel);
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(new Vector2(h.x, h.y), OpenAreaAroundDestination);
            if (hitColliders.Length > 0)
            {

            }
            else
            {
                done = true;
                Instantiate(destination, h, Quaternion.identity);

            }
        }
    }

}