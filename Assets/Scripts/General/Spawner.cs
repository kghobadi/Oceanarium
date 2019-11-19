using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    [Header("Spawner Objects & Type")]
    public ObjectPooler objectPooler;
    public GameObject[] generatedObjs;
    public GenerationType generationType;
    public enum GenerationType
    {
        RANDOM, SQUARE, PAIRS, GROUPS,
    }

    public ObjectType objectType;
    public enum ObjectType
    {
        PLONT, CROPSEED, SHROOM, 
    }
    public bool overlapsWithSaveSystem;
    public bool plantOnStart = true;
    public int ageMin=0, ageMax=3;

    [Header("Collision Avoidance")]
    public LayerMask collidableObjects;
    public LayerMask grounded ;
    public float heightAdjust;
    public float checkDist;

    //for RANDOM
    [Header("RANDOM")]
    public int generationAmount;
    public float generationRadius;

    //for SQUARE
    [Header("SQUARE")]
    public int gridSizeX;
    public int gridSizeY;
    public float distBetweenX, distBetweenY;

    void Start () {
        Random.InitState(System.DateTime.Now.Millisecond);
        generatedObjs = new GameObject[generationAmount];

        gameObject.name = generationType.ToString() + " " + objectPooler.name + " Spawner";
    }

    //switch statement tells us which way to generate
    public void GenerateObjects()
    {
        switch (generationType)
        {
            case GenerationType.RANDOM:
                GenerateRandom();
                break;
            case GenerationType.SQUARE:
                GenerateSquare();
                break;
            case GenerationType.PAIRS:
                GeneratePairs();
                break;
            case GenerationType.GROUPS:
                GenerateGroups();
                break;
        }
    }


    bool CheckForSpawnCollisions(Vector3 point)
    {
        //check in radius of planting point if its too close to others
        Collider[] hitColliders = Physics.OverlapSphere(point, checkDist, collidableObjects);
        if(hitColliders.Length > 0)
        {
            //COLLIDED, RUN AGAIN
            return true;
        }
        else
        {
            //NO COLLISIONS, WE'RE CLEAR 
            return false;
        }
    }

    //generate objects in a random unit circle 
    void GenerateRandom()
    {
        for(int i = 0; i < generationAmount;i++)
        {
            Vector2 xz = Random.insideUnitCircle * generationRadius;
            
            Vector3 spawnPos = transform.position + new Vector3(xz.x, 0, xz.y);

            //this loop ends if obj is not colliding with things 
            while (CheckForSpawnCollisions(spawnPos) == true)
            {
                xz = Random.insideUnitCircle * generationRadius;

                spawnPos = transform.position + new Vector3(xz.x, 0, xz.y);
            }

            generatedObjs[i] = objectPooler.GrabObject();

            generatedObjs[i].transform.position = spawnPos;
        }
    }

    //generate objects in a square grid pattern 
    void GenerateSquare()
    {
        //set to size of the grid we will be making 
        generatedObjs = new GameObject[(gridSizeX  + 1) * (gridSizeY  + 1)];

        for (int i = 0, y = 0; y <= gridSizeY; y++)
        {
            for (int x = 0; x <= gridSizeX; x++, i++)
            {
                generatedObjs[i] = objectPooler.GrabObject();

                Vector3 spawnPos = new Vector3(x * distBetweenX, transform.position.y, y * distBetweenY) + transform.position;

                generatedObjs[i].transform.position = spawnPos;
            }
        }
    }

    //generate objects in semi-random pairs 
    void GeneratePairs()
    {

    }

    //generate objects in semi-random groups 
    void GenerateGroups()
    {

    }

    //called when player enters zone 
    public void RegenerateObjects()
    {
        int inactive = 0;
        for(int i = 0; i < generatedObjs.Length; i++)
        {
            //if null or inactive
            if(generatedObjs[i] == null || generatedObjs[i].activeSelf == false)
            {
                inactive++;
            }
        }

        //inactive count is greater than half of original list  && not cropseed plantOnStart
        if(inactive >= (generatedObjs.Length / 2))
        {
            generatedObjs = new GameObject[generationAmount];

            GenerateObjects();
        }
    }

    //adjust height of generated obj 
    void AdjustHeight(GameObject obj)
    {
        RaycastHit hit;

        if (Physics.Raycast(obj.transform.position, Vector3.down, out hit, Mathf.Infinity, grounded))
        {
            if (hit.transform.gameObject.tag == "Ground")
            {
                obj.transform.position = new Vector3(obj.transform.position.x, hit.point.y + heightAdjust, obj.transform.position.z);
            }

            Debug.DrawLine(obj.transform.position, hit.point);
        }
        
    }
}
