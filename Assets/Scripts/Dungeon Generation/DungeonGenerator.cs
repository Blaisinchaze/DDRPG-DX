
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Dictionary<Vector2Int, TileInfo> floorList = new Dictionary<Vector2Int, TileInfo>();
    public GameObject[] randomItems;
    public GameObject wall, floor, exit, tilePrefab;
    public LayerMask wallMask, floorMask;

    PlayerController player;
    AIController aiController;

    public class TileInfo
    {
        public Vector2Int Location;
        public bool traversable = true;
        public bool exit = false;
        public List<TileInfo> neighbours;
        public TileInfo(Vector2Int _location)
        {
            Location = _location;
        }
        public void SetNeighbours(List<TileInfo> neighbours)
        {
            this.neighbours = neighbours;
        }
    }

    private void Start()
    {
        player = Object.FindObjectOfType<PlayerController>();
        aiController = Object.FindObjectOfType<AIController>();
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
        player.InitialisePlayer();
        for (int i = 0; i < 4; i++)
        {
            aiController.CreateEnemy(floorList.ElementAt(Random.Range(4, floorList.Count)).Key);
        }
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
        Vector2Int currentPos = Vector2Int.zero;
        floorList.Add(currentPos, new TileInfo(currentPos));//add currentpos to list
        //set floor tile at position
        while (floorList.Count < totalFloorCount)
        {
            currentPos += RandomDirection();
            if (!InFloorList(currentPos))
            {
                floorList.Add(currentPos, new TileInfo(currentPos));
            }
        } //set at a new position
        StartCoroutine(DelayProgress());

    }
    void RoomWalker()
    {
        Vector2Int currentPos = Vector2Int.zero;
        floorList.Add(currentPos, new TileInfo(currentPos));//add currentpos to list
        //set floor tile at position
        while (floorList.Count < totalFloorCount)
        {
            currentPos = Vector2Int.RoundToInt(LongWalk(currentPos));
            RandomRoom(currentPos);

        } //set at a new position      
        StartCoroutine(DelayProgress());

    }
    void HallRoomWalker()
    {
        Vector2Int currentPos = Vector2Int.zero;
        floorList.Add(currentPos, new TileInfo(currentPos));//add currentpos to list
        //set floor tile at position
        while (floorList.Count < totalFloorCount)
        {
            currentPos = Vector2Int.RoundToInt(LongWalk(currentPos));
            int roll = Random.Range(1, 100);
            if (roll > hallwayPercentage)
            {
                RandomRoom(currentPos);
            }


        } //set at a new position      
        StartCoroutine(DelayProgress());

    }
    Vector3 LongWalk(Vector2Int myPos)
    {
        Vector2Int walkDir = RandomDirection();
        int walkLength = Random.Range(9, 10);
        if (myPos != Vector2Int.zero) { myPos += RandomDirection(); }
        for (int i = 0; i < walkLength; i++)
        {
            if (!InFloorList(myPos + walkDir))
            {
                floorList.Add(myPos+walkDir,new TileInfo(myPos + walkDir));
            }
            myPos += walkDir;
        }
        return new Vector3(myPos.x, myPos.y);
    }
    void RandomRoom(Vector2Int myPos)
    {
        int width = Random.Range(1, 5);
        int height = Random.Range(1, 5);
        for (int w = -width; w <= width; w++)
        {
            for (int h = -height; h < height; h++)
            {
                Vector2Int offset = new Vector2Int(w, h);
                if (!InFloorList(myPos + offset))
                {
                    floorList.Add(myPos+offset,new TileInfo(myPos + offset));
                }
            }
        }
    }
    bool InFloorList(Vector2Int myPos)
    {
        for (int i = 0; i < floorList.Count; i++)
        {
            if (floorList.ContainsKey(myPos) )
            {
                return true;
            }
        }
        return false;
    }
    Vector2Int RandomDirection()
    {
        switch (Random.Range(1, 4))
        {
            case 1:
                return Vector2Int.up;

            case 2:
                return Vector2Int.right;

            case 3:
                return Vector2Int.down;

            case 4:
                return Vector2Int.left;
        }
        return Vector2Int.up;
    }
    IEnumerator DelayProgress()
    {
        for (int i = 0; i < floorList.Count; i++)
        {
            Vector3 tempLocation = new Vector3(floorList.ElementAt(i).Value.Location.x, floorList.ElementAt(i).Value.Location.y);
            GameObject goTile = Instantiate(tilePrefab, tempLocation, Quaternion.identity) as GameObject;
            goTile.name = tilePrefab.name;
            goTile.transform.SetParent(transform);
        }
        foreach (var tile in floorList)
        {
            List<TileInfo> _tempNeighbours = new List<TileInfo>();
            if (floorList.ContainsKey(tile.Key + Vector2Int.up))
            {
                _tempNeighbours.Add(floorList[tile.Key + Vector2Int.up]);
            }
            if (floorList.ContainsKey(tile.Key + Vector2Int.right))
            {
                _tempNeighbours.Add(floorList[tile.Key + Vector2Int.right]);
            }
            if (floorList.ContainsKey(tile.Key + Vector2Int.down))
            {
                _tempNeighbours.Add(floorList[tile.Key + Vector2Int.down]);
            }
            if (floorList.ContainsKey(tile.Key + Vector2Int.left))
            {
                _tempNeighbours.Add(floorList[tile.Key + Vector2Int.left]);
            }
            //for (int x = -1; x < 2; x+=2)
            //{
            //    for (int y = -1; y < 2; y += 2)
            //    {
            //        if(floorList.ContainsKey(tile.Key + new Vector2Int(x, y)))
            //        {
            //            _tempNeighbours.Add(floorList[tile.Key + new Vector2Int(x, y)]);
            //        }
            //    }
            //}
            tile.Value.SetNeighbours(_tempNeighbours);
        }
        while (FindObjectsOfType<TileSpawner>().Length > 0)
        {
            yield return null;
        }
        ExitDoor();

    }
    void ExitDoor()
    {
        Vector2 doorPos = floorList.ElementAt(floorList.Count - 1).Value.Location;
        GameObject goExit = Instantiate(exit, doorPos, Quaternion.identity) as GameObject;
        goExit.name = tilePrefab.name;
        goExit.transform.SetParent(transform);
        floorList.ElementAt(floorList.Count - 1).Value.exit = true;
        

        SetupForGameplay();
    }

    public bool IsTileTraversable(Vector2Int _targetTile)
    {
        if(floorList.ContainsKey(_targetTile))
        {
            return floorList[_targetTile].traversable;
        }
        Debug.Log("no tile found");
        return false;
    }

}