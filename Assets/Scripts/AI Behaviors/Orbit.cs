using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour {

    public Transform planetToOrbit;
    public Axes xyOrzAxis;
    public float orbitalSpeed;

    public bool randomAxis, swimming;

    Vector3 chosenAxis;

    public enum Axes
    {
        X, Y, Z,
    }

    //set chosen axis at start
    void Start()
    {
        RandomizeOrbitAxis();
        swimming = true;
    }

    void Update () {
        if(swimming)
            transform.RotateAround(planetToOrbit.position, chosenAxis, orbitalSpeed * Time.deltaTime);
	}

    public void RandomizeOrbitAxis()
    {
        //randomize Axis
        if (randomAxis)
        {
            float randomAxes = Random.Range(0, 100);
            if (randomAxes < 33)
            {
                xyOrzAxis = Axes.X;
            }
            else if (randomAxes > 33 && randomAxes < 66)
            {
                xyOrzAxis = Axes.Y;
            }
            else if (randomAxes > 66 && randomAxes < 100)
            {
                xyOrzAxis = Axes.Z;
            }
        }

        if (xyOrzAxis == Axes.X)
        {
            chosenAxis = transform.right;
        }
        else if (xyOrzAxis == Axes.Y)
        {
            chosenAxis = transform.up;
        }
        else if (xyOrzAxis == Axes.Z)
        {
            chosenAxis = transform.forward;
        }
    }
}
