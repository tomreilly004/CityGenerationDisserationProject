using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public Grid grid;

    public RoadVisualiser roadVisualiser;

    public bool enableRandom;

    public bool enablePerlin;

    public bool enableLSystem;

    // Start is called before the first frame update
    void Start()
    {
        if (enableRandom || enablePerlin)
        {
            //grid.StartGrid(enablePerlin, enableRandom);
        }
        else if (enableLSystem)
        {
            //roadVisualiser.StartLsystem();
        }
    }

}
