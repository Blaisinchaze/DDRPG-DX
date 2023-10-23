using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Vector2Int currentTile;
    public Vector3 targetLocation;
    public Vector2Int directionFacing;
    public Quaternion targetDirection = new Quaternion();

    public float movementSpeed;
    public float rotationSpeed;

    DungeonGenerator dungeon;
    PlayerController playerController;

    public List<Vector2Int> path = new List<Vector2Int>();
    private void Start()
    {
        dungeon = Object.FindObjectOfType<DungeonGenerator>();
        playerController = Object.FindObjectOfType<PlayerController>();
    }

    public void Init(Vector2Int currentTile)
    {
        this.currentTile = currentTile;
        this.targetLocation = new Vector3(currentTile.x,0,currentTile.y);
        this.directionFacing = Vector2Int.up;
        this.targetDirection = Quaternion.identity;
        this.movementSpeed = 10f;
        this.rotationSpeed = 10f;
    }
    void Update()
    {

        transform.LookAt(playerController.transform);

        if (transform.position != targetLocation)
        {
            transform.position = Vector3.Lerp(transform.position, targetLocation, Time.deltaTime * movementSpeed);
        }
    }

    public void MoveAndAttack()
    {
        if (path.Count == 1)
        {
            //attack
        }
        else if (path.Count > 1)
        {
            dungeon.floorList[currentTile].traversable = true;
            currentTile = path[0];
            targetLocation = new Vector3(path[0].x, 0, path[0].y);
            dungeon.floorList[currentTile].traversable = false;
        }

    }

    public void FindPath(Vector2Int _targetLocation)
    {
        path.Clear();

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();

        Node endPoint = new Node(_targetLocation,99999,0);

        open.Add(new Node( currentTile,0,(int)Vector2Int.Distance(currentTile, _targetLocation)));
        Node currentNode = null;
        Node neighbourNode = null;


        while (open.Count > 0)
        {
            currentNode = neighbourNode = null;
            //find the node with the lowest F (switch to a for loop to reduce memory usage)
            foreach (Node node in open)
            {
                if (currentNode == null || node.f < currentNode.f)
                {
                    currentNode = node;
                }
            }

            open.Remove(currentNode);

            //if we've been to this node before we don't want to go there again (shouldn't be a problem really)
            if (closed.Contains(currentNode)) continue;
            else closed.Add(currentNode);

            //We've reached our target (EXIT HERE)
            if (closed.Contains(endPoint)) break;

            //Go through the neighbours (switch to a for loop to reduce memory usage)
            for (int i = 0; i < dungeon.floorList[currentNode.tile].neighbours.Count; i++)
            {
                if(dungeon.floorList[currentNode.tile].neighbours[i].traversable == false) { continue; }
                neighbourNode = closed.Find(x => x.tile == dungeon.floorList[currentNode.tile].neighbours[i].Location);
                if (neighbourNode != null)
                {

                    continue;
                }
                neighbourNode = new Node(dungeon.floorList[currentNode.tile].neighbours[i].Location, currentNode.g +1, (int)Vector2Int.Distance(dungeon.floorList[currentNode.tile].neighbours[i].Location, _targetLocation));
                //if we know that the neighbours will contain the endPoint we can just go for that or search for the node in open.
                if (neighbourNode.tile == endPoint.tile)
                {
                    neighbourNode = endPoint;
                    open.Add(neighbourNode);
                }
                else neighbourNode = open.Find(x => x.tile == dungeon.floorList[currentNode.tile].neighbours[i].Location);

                if (neighbourNode == null)
                {
                    neighbourNode = new Node(dungeon.floorList[currentNode.tile].neighbours[i].Location, currentNode.g + 1, (int)Vector2Int.Distance(dungeon.floorList[currentNode.tile].neighbours[i].Location, _targetLocation));

                    //Any pathfinding Node calculations can be done here
                    open.Add(neighbourNode);
                }
                else if (neighbourNode.f > currentNode.g + 1 + neighbourNode.h)
                {
                    neighbourNode.g = currentNode.g + 1;
                }
            }
        }
        if (closed.Contains(endPoint))
        {
            path.Add(endPoint.tile);
            Node lastNode = endPoint;
            for (int i = lastNode.g - 1; i >= 0; i--)
            {
                lastNode = closed.Find(x => x.g == i && dungeon.floorList[lastNode.tile].neighbours.Contains(dungeon.floorList[x.tile]));

                if (lastNode != null) path.Add(lastNode.tile);
            }

            path.Reverse();
            path.RemoveAt(0);
        }
    }
}
