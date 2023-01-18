using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public enum State
    {
        UNASSIGNED, // 0
        ROAD, // 1
        RESIDENTIAL, // 2
        INDUSTRIAL, // 3
        COMMERCIAL, // 4
        EMPTY // 5
    };

    //public State[] cellStates = new State[9];
    public CellID[,] cells = new CellID[100,100];

    public int gridX;
    public int gridZ;
    public GameObject prefabToSpawn;
    public Vector3 gridOrigin = Vector3.zero;
    public float gridOffset = 1f;
    public float roadThreshold = 0.1f;


    public bool EnableRandom;
    public bool EnablePerlin;

    void Awake()
    {

    }
    public void Start()
    {
        var watch = new System.Diagnostics.Stopwatch();

        if (EnableRandom == true)
        {
            watch.Start();
            Generate();
            GenerateRandomCity();
            watch.Stop();
            Debug.Log(watch.ElapsedMilliseconds + "ms");
        }
        else if(EnablePerlin == true)
        {
            watch.Start();
            Generate();
            GeneratePerlinCity();
            watch.Stop();
            Debug.Log(watch.ElapsedMilliseconds + "ms");
        }
    }

    private void GeneratePerlinCity()
    {
        GenerateRandomRoads();
        GeneratePerlinDistricts();
    }

    private void GeneratePerlinDistricts()
    {
        for(int x = 0; x < gridX; x++)
        {
            for(int z = 0; z < gridZ; z++)
            {
                if (cells[x, z].GetComponent<CellID>().ID == State.ROAD)
                {
                    continue;
                }

                float sampledValue = GetComponent<PerlinNoiseCreator>().PerlinSteppedPosition(cells[x, z].GetComponent<CellID>().gameObject.transform.position);
                
                if (sampledValue < 0.4)
                {
                    cells[x, z].GetComponent<CellID>().ID = State.RESIDENTIAL;
                }
                else if (sampledValue > 0.4 && sampledValue < 0.75)
                {
                    cells[x, z].GetComponent<CellID>().ID = State.INDUSTRIAL;
                }
                else if (sampledValue > 0.74)
                {
                    cells[x, z].GetComponent<CellID>().ID = State.COMMERCIAL;
                }
            }
        }
    }

    private void GenerateRandomCity()
    {
        GenerateRandomRoads();
        GenerateRandomDistricts();
        FillEmptyCells();
    }

    private void FillEmptyCells()
    {
        for(int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                if (cells[x, z].GetComponent<CellID>().ID == State.UNASSIGNED)
                {
                    cells[x, z].GetComponent<CellID>().ID = State.EMPTY;
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate()
    {
        SpawnGrid();
    }

    void SpawnGrid()
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                GameObject clone = Instantiate(prefabToSpawn, transform.position + gridOrigin + new Vector3(gridOffset * x, 0, gridOffset * z), transform.rotation);
                cells[x, z] = clone.GetComponent<CellID>();
                cells[x, z].GetComponent<CellID>().ID = State.UNASSIGNED;
                clone.transform.SetParent(this.transform);
            }
        }
    }
    private void GenerateRandomDistricts()
    {
        int lastGridX;
        int lastGridZ;

        for (lastGridX = 0; lastGridX <= 99; lastGridX++)
        {
            for (lastGridZ = 0; lastGridZ <= 99; lastGridZ++)
            {
                if (cells[lastGridX, lastGridZ].GetComponent<CellID>().ID != State.ROAD)
                {
                    int buildingType = Random.Range(0, 10);

                    if (buildingType <= 5)
                    {
                        cells[lastGridX, lastGridZ].GetComponent<CellID>().ID = State.RESIDENTIAL;
                    }
                    else if (buildingType <= 8)
                    {
                        cells[lastGridX, lastGridZ].GetComponent<CellID>().ID = State.INDUSTRIAL;
                    }
                    else if (buildingType <= 10)
                    {
                        cells[lastGridX, lastGridZ].GetComponent<CellID>().ID = State.COMMERCIAL;
                    }
                }
            }
        }
    }

    private void GenerateRandomRoads()
    {
        bool mainRoadsComplete = false;
        bool smallRoadsComplete = false;
        int lastRoadX;
        int lastRoadZ;

        while (!mainRoadsComplete)
        {
            for (lastRoadX = 0; lastRoadX < gridX; lastRoadX++)
            {
                int roadGap = Random.Range(10, 14);
                lastRoadX += roadGap;

                if(lastRoadX >= gridX)
                {
                    break;
                }
                
                for(int z = 0; z < gridZ; z++)
                {
                    cells[lastRoadX, z].GetComponent<CellID>().ID = State.ROAD;
                    if (lastRoadX + 1 < gridX)
                    {
                        cells[lastRoadX + 1, z].GetComponent<CellID>().ID = State.ROAD;
                    }
                }
            }
            mainRoadsComplete = true;
        }

        while (!smallRoadsComplete)
        {
            for (lastRoadZ = 0; lastRoadZ < gridZ; lastRoadZ++)
            {
                int roadGap = Random.Range(5, 10);
                lastRoadZ += roadGap;

                if (lastRoadZ >= gridZ)
                {
                    break;
                }

                for (int x = 0; x < gridX; x++)
                {
                    cells[x, lastRoadZ].GetComponent<CellID>().ID = State.ROAD;

                }
            }
            smallRoadsComplete = true;
        }

    }
}
