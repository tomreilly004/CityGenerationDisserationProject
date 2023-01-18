using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellID : MonoBehaviour
{
    public GameObject road, residential, industrial, commerical, empty;

    public Grid.State ID = 0;

    private bool updated = false;

    // Update is called once per frame

    void Update()
    {
        if (ID == Grid.State.ROAD)
        {
            if (!updated)
            {
                GameObject clone = Instantiate(road, transform.position, transform.rotation);
                clone.transform.SetParent(this.transform);
                updated = true;
            }
        }

        if (ID == Grid.State.EMPTY)
        {
            if (!updated)
            {
                GameObject clone = Instantiate(empty, transform.position, transform.rotation);
                clone.transform.SetParent(this.transform);
                updated = true;
            }
        }

        if (ID == Grid.State.COMMERCIAL)
        {
            if (!updated)
            {
                //int largeHeight = Random.Range(7, 15);

                //for (int a = 0; a <= largeHeight; a++)
                //{
                    GameObject clone = Instantiate(commerical, transform.position + new Vector3(0, 0, 0), transform.rotation);
                clone.transform.localScale = new Vector3(0.1f, 0.5f, 0.1f);
                clone.transform.SetParent(this.transform);
                //}
                updated = true;
            }
        }

        if (ID == Grid.State.RESIDENTIAL)
        {
            if (!updated)
            {
               // int smallHeight = Random.Range(0, 3);

               // for (int a = 0; a <= smallHeight; a++)
               // {
                    GameObject clone = Instantiate(residential, transform.position + new Vector3(0, 0, 0), transform.rotation);
                clone.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);
                clone.transform.SetParent(this.transform);
               // }
                updated = true;
            }
        }

        if (ID == Grid.State.INDUSTRIAL)
        {
            if (!updated)
            {
               // int mediumHeight = Random.Range(2, 6);

                //for (int a = 0; a <= mediumHeight; a++)
                //{
                    GameObject clone = Instantiate(industrial, transform.position + new Vector3(0, 0, 0), transform.rotation);
                    clone.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);

                    clone.transform.SetParent(this.transform);
                //}
                updated = true;
            }
        }
    }
}
