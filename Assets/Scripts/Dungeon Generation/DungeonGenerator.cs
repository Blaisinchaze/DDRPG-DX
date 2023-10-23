
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum DungeonType { Caverns, Rooms, WindingHalls }
public class DungeonGenerator : MonoBehaviour
{
    [Header("Values")]
    [HideInInspector] public float minX, minY, maxX, maxY;
    [Range(50, 1000)] public int totalFloorCount;
    [Range(1, 100)] public int hallwayPercentage;
    public DungeonType dungeonType;

    [Space]
    [Header("Bools")]
    public bool useRoundedEdges;

    [Space]
    [Header("Lists & Arrays")]
    private List<Vector3> floorList = new List<Vector3>();
    public GameObject[] randomItems;
    public GameObject wall, floor, exit, tilePrefab;
    public LayerMask wallMask, floorMask;


    private void Start()
    {
        switch (dungeonType)
        {
            case DungeonType.Caverns: RandomWalker(); break;
            case DungeonType.Rooms: RoomWalker(); break;
            case DungeonType.WindingHalls: HallRoomWalker(); break;
        }

    }

    public void SetupForGameplay()
    {
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        if (Application.isEditor && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    void RandomWalker()
    {
        Vector3 currentPos = Vector3.zero;
        floorList.Add(currentPos);//add currentpos to list
        //set floor tile at position
        while (floorList.Count < totalFloorCount)
        {
            currentPos += RandomDirection();
            if (!InFloorList(currentPos))
            {
                floorList.Add(currentPos);
            }
        } //set at a new position
        StartCoroutine(DelayProgress());

    }
    void RoomWalker()
    {
        Vector3 currentPos = Vector3.zero;
        floorList.Add(currentPos);//add currentpos to list
        //set floor tile at position
        while (floorList.Count < totalFloorCount)
        {
            currentPos = LongWalk(currentPos);
            RandomRoom(currentPos);

        } //set at a new position      
        StartCoroutine(DelayProgress());

    }
    void HallRoomWalker()
    {
        Vector3 currentPos = Vector3.zero;
        floorList.Add(currentPos);//add currentpos to list
        //set floor tile at position
        while (floorList.Count < totalFloorCount)
        {
            currentPos = LongWalk(currentPos);
            int roll = Random.Range(1, 100);
            if (roll > hallwayPercentage)
            {
                RandomRoom(currentPos);
            }


        } //set at a new position      
        StartCoroutine(DelayProgress());

    }
    Vector3 LongWalk(Vector3 myPos)
    {
        Vector3 walkDir = RandomDirection();
        int walkLength = Random.Range(9, 10);
        myPos += RandomDirection();
        for (int i = 0; i < walkLength; i++)
        {
            if (!InFloorList(myPos + walkDir))
            {
                floorList.Add(myPos + walkDir);
            }
            myPos += walkDir;
        }
        return myPos;
    }
    void RandomRoom(Vector3 myPos)
    {
        int width = Random.Range(1, 5);
        int height = Random.Range(1, 5);
        for (int w = -width; w <= width; w++)
        {
            for (int h = -height; h < height; h++)
            {
                Vector3 offset = new Vector3(w, h, 0);
                if (!InFloorList(myPos + offset))
                {
                    floorList.Add(myPos + offset);
                }
            }
        }
    }
    bool InFloorList(Vector3 myPos)
    {
        for (int i = 0; i < floorList.Count; i++)
        {
            if (Vector3.Equals(myPos, floorList[i]))
            {
                return true;
            }
        }
        return false;
    }
    Vector3 RandomDirection()
    {
        switch (Random.Range(1, 5))
        {
            case 1:
                return Vector3.up;

            case 2:
                return Vector3.right;

            case 3:
                return Vector3.down;

            case 4:
                return Vector3.left;
        }
        return Vector3.zero;
    }
    IEnumerator DelayProgress()
    {
        for (int i = 0; i < floorList.Count; i++)
        {
            GameObject goTile = Instantiate(tilePrefab, floorList[i], Quaternion.identity) as GameObject;
            goTile.name = tilePrefab.name;
            goTile.transform.SetParent(transform);
        }
        while (FindObjectsOfType<TileSpawner>().Length > 0)
        {
            yield return null;
        }
        ExitDoor();

    }
    void ExitDoor()
    {
        Vector3 doorPos = floorList[floorList.Count - 1];
        {
            GameObject goExit = Instantiate(exit, doorPos, Quaternion.identity) as GameObject;
            goExit.name = tilePrefab.name;
            goExit.transform.SetParent(transform);
        }

        SetupForGameplay();
    }
}