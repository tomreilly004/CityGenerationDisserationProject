using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadVisualiser : MonoBehaviour
{
    public LSystemCreator lsystem;
    List<Vector3> positions = new List<Vector3>();

    public GameObject roadPrefab;

    public GameObject cellPrefab;

    Dictionary<Vector3Int, GameObject> roadDictionary = new Dictionary<Vector3Int, GameObject>();

    Dictionary<Vector3Int, GameObject> buildingDictionary = new Dictionary<Vector3Int, GameObject>();

    public int length = 8;

    public float angle = 42;

    public bool enableLsystem = false;

    [Range(0, 30)]
    public int smallRange = 0;
    [Range(20, 60)]
    public int largeRange = 0;

    public int Length
    {
        get
        {
            if (length > 0)
            {
                return length;
            }
            else
            {
                return 1;
            }
        }
        set => length = value;
    }

    private void Start()
    {
        if (enableLsystem)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var sequence = lsystem.GenerateSentence();
            VisualiseSequence(sequence);
            PlaceBuildings();
            watch.Stop();
            Debug.Log(watch.ElapsedMilliseconds + "ms");
        }
    }

    public enum EncodingLetters
    {
        unknown = '1',
        save = '[',
        load = ']',
        draw = 'F',
        turnRight = '+',
        turnLeft = '-'
    }

    private void VisualiseSequence(string sequence)
    {
        Stack<Agent> savePoints = new Stack<Agent>();
        var currentPosition = Vector3.zero;

        Vector3 direction = Vector3.forward;
        Vector3 tempPosition = Vector3.zero;

        positions.Add(currentPosition);

        foreach (var letter in sequence)
        {
            EncodingLetters encoding = (EncodingLetters)letter;
            switch (encoding)
            {
                case EncodingLetters.save:
                    savePoints.Push(new Agent
                    {
                        position = currentPosition,
                        direction = direction,
                        length = Length
                    });
                    break;
                case EncodingLetters.load:
                    if (savePoints.Count > 0)
                    {
                        var agentParameter = savePoints.Pop();
                        currentPosition = agentParameter.position;
                        direction = agentParameter.direction;
                        Length = agentParameter.length;
                    }
                    else
                    {
                        throw new System.Exception("Dont have saved point in our stack");
                    }
                    break;
                case EncodingLetters.draw:
                    tempPosition = currentPosition;
                    currentPosition += direction * Length;
                    //Draw Road
                    PlaceRoad(tempPosition, Vector3Int.RoundToInt(direction), length);
                    Length -= 2;
                    positions.Add(currentPosition);
                    break;
                case EncodingLetters.turnRight:
                    direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                    break;
                case EncodingLetters.turnLeft:
                    direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                    break;
                default:
                    break;
            }
        }
    }

    public void PlaceRoad(Vector3 startPostion, Vector3Int direction, int currLength)
    {
        var rotation = Quaternion.identity;

        for(int i = 0; i < currLength; i++)
        {
            var position = Vector3Int.RoundToInt(startPostion + (direction * i));
            if (roadDictionary.ContainsKey(position))
            {
                continue;
            }

            var road = Instantiate(roadPrefab, position, rotation, transform);
            roadDictionary.Add(position, road);
            
        }
    }

    public void PlaceBuildings()
    {
        var rotation = Quaternion.identity;

        foreach ( GameObject gameObject in roadDictionary.Values)
        {
            Vector3 left = new Vector3(gameObject.transform.position.x - 1, gameObject.transform.position.y, gameObject.transform.position.z);
            Vector3 right = new Vector3(gameObject.transform.position.x + 1, gameObject.transform.position.y, gameObject.transform.position.z);
            Vector3 front = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 1);
            Vector3 back = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 1);
            Vector3 frontLeft = new Vector3(gameObject.transform.position.x - 1, gameObject.transform.position.y, gameObject.transform.position.z + 1);
            Vector3 frontRight = new Vector3(gameObject.transform.position.x + 1, gameObject.transform.position.y, gameObject.transform.position.z + 1);
            Vector3 backLeft = new Vector3(gameObject.transform.position.x - 1, gameObject.transform.position.y, gameObject.transform.position.z - 1);
            Vector3 backRight = new Vector3(gameObject.transform.position.x - 1, gameObject.transform.position.y, gameObject.transform.position.z - 1);

            float sampledValueLeft = GetComponent<PerlinNoiseCreator>().PerlinSteppedPosition(left);
            float sampledValueRight = GetComponent<PerlinNoiseCreator>().PerlinSteppedPosition(right);
            float sampledValueFront = GetComponent<PerlinNoiseCreator>().PerlinSteppedPosition(front);
            float sampledValueBack = GetComponent<PerlinNoiseCreator>().PerlinSteppedPosition(back);
            float sampledValueFrontLeft = GetComponent<PerlinNoiseCreator>().PerlinSteppedPosition(frontLeft);
            float sampledValueFrontRight = GetComponent<PerlinNoiseCreator>().PerlinSteppedPosition(frontRight);
            float sampledValueBackLeft = GetComponent<PerlinNoiseCreator>().PerlinSteppedPosition(backLeft);
            float sampledValueBackRight = GetComponent<PerlinNoiseCreator>().PerlinSteppedPosition(backRight);

            if (!roadDictionary.ContainsKey(Vector3Int.RoundToInt(left)) && !buildingDictionary.ContainsKey(Vector3Int.RoundToInt(left)))
            {
                Instantiate(cellPrefab, left, rotation, gameObject.transform);
                buildingDictionary.Add(Vector3Int.RoundToInt(left), cellPrefab);
                if (sampledValueLeft < 0.4)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.RESIDENTIAL;
                }
                else if (sampledValueLeft > 0.4 && sampledValueLeft < 0.75)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.INDUSTRIAL;
                }
                else if (sampledValueLeft > 0.74)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.COMMERCIAL;
                }
            }
            if (!roadDictionary.ContainsKey(Vector3Int.RoundToInt(right)) && !buildingDictionary.ContainsKey(Vector3Int.RoundToInt(right)))
            {
                Instantiate(cellPrefab, right, rotation, gameObject.transform);
                buildingDictionary.Add(Vector3Int.RoundToInt(right), cellPrefab);
                if (sampledValueRight < 0.4)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.RESIDENTIAL;
                }
                else if (sampledValueRight > 0.4 && sampledValueRight < 0.75)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.INDUSTRIAL;
                }
                else if (sampledValueRight > 0.74)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.COMMERCIAL;
                }
            }
            if (!roadDictionary.ContainsKey(Vector3Int.RoundToInt(front)) && !buildingDictionary.ContainsKey(Vector3Int.RoundToInt(front)))
            {
                Instantiate(cellPrefab, front, rotation, gameObject.transform);
                buildingDictionary.Add(Vector3Int.RoundToInt(front), cellPrefab);
                if (sampledValueFront < 0.4)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.RESIDENTIAL;
                }
                else if (sampledValueFront > 0.4 && sampledValueFront < 0.75)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.INDUSTRIAL;
                }
                else if (sampledValueFront > 0.74)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.COMMERCIAL;
                }
            }
            if (!roadDictionary.ContainsKey(Vector3Int.RoundToInt(back)) && !buildingDictionary.ContainsKey(Vector3Int.RoundToInt(back)))
            {
                Instantiate(cellPrefab, back, rotation, gameObject.transform);
                buildingDictionary.Add(Vector3Int.RoundToInt(back), cellPrefab);
                if (sampledValueBack < 0.4)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.RESIDENTIAL;
                }
                else if (sampledValueBack > 0.4 && sampledValueBack < 0.75)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.INDUSTRIAL;
                }
                else if (sampledValueBack > 0.74)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.COMMERCIAL;
                }
            }
            if (!roadDictionary.ContainsKey(Vector3Int.RoundToInt(frontLeft)) && !buildingDictionary.ContainsKey(Vector3Int.RoundToInt(frontLeft)))
            {
                Instantiate(cellPrefab, frontLeft, rotation, gameObject.transform);
                buildingDictionary.Add(Vector3Int.RoundToInt(frontLeft), cellPrefab);
                if (sampledValueFrontLeft < 0.4)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.RESIDENTIAL;
                }
                else if (sampledValueFrontLeft > 0.4 && sampledValueFrontLeft < 0.75)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.INDUSTRIAL;
                }
                else if (sampledValueFrontLeft > 0.74)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.COMMERCIAL;
                }
            }
            if (!roadDictionary.ContainsKey(Vector3Int.RoundToInt(frontRight)) && !buildingDictionary.ContainsKey(Vector3Int.RoundToInt(frontRight)))
            {
                Instantiate(cellPrefab, frontRight, rotation, gameObject.transform);
                buildingDictionary.Add(Vector3Int.RoundToInt(frontRight), cellPrefab);
                if (sampledValueFrontRight < 0.4)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.RESIDENTIAL;
                }
                else if (sampledValueFrontRight > 0.4 && sampledValueFrontRight < 0.75)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.INDUSTRIAL;
                }
                else if (sampledValueFrontRight > 0.74)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.COMMERCIAL;
                }
            }
            if (!roadDictionary.ContainsKey(Vector3Int.RoundToInt(backLeft)) && !buildingDictionary.ContainsKey(Vector3Int.RoundToInt(backLeft)))
            {
                Instantiate(cellPrefab, backLeft, rotation, gameObject.transform);
                buildingDictionary.Add(Vector3Int.RoundToInt(backLeft), cellPrefab);
                if (sampledValueBackLeft < 0.4)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.RESIDENTIAL;
                }
                else if (sampledValueBackLeft > 0.4 && sampledValueBackLeft < 0.75)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.INDUSTRIAL;
                }
                else if (sampledValueBackLeft > 0.74)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.COMMERCIAL;
                }
            }
            if (!roadDictionary.ContainsKey(Vector3Int.RoundToInt(backRight)) && !buildingDictionary.ContainsKey(Vector3Int.RoundToInt(backRight)))
            {
                Instantiate(cellPrefab, backRight, rotation, gameObject.transform);
                buildingDictionary.Add(Vector3Int.RoundToInt(backRight), cellPrefab);
                if (sampledValueBackRight < 0.4)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.RESIDENTIAL;
                }
                else if (sampledValueBackRight > 0.4 && sampledValueBackRight < 0.75)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.INDUSTRIAL;
                }
                else if (sampledValueBackRight > 0.74)
                {
                    cellPrefab.GetComponent<CellID>().ID = Grid.State.COMMERCIAL;
                }
            }
        }
    }

}
