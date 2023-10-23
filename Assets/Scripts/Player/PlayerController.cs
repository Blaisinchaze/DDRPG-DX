using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public Vector2Int currentTile;
    public Vector3 targetLocation;
    public Vector2Int directionFacing;
    public Quaternion targetDirection = new Quaternion();

    public float movementSpeed;
    public float rotationSpeed;

    DungeonGenerator dungeon;
    AIController aiController;
    // Start is called before the first frame update
    void Start()
    {
        dungeon = Object.FindObjectOfType<DungeonGenerator>();
        aiController = Object.FindObjectOfType<AIController>();
        targetDirection = Quaternion.identity;
    }
    // Update is called once per frame
    void Update()
    {
        if(transform.rotation != targetDirection)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetDirection, Time.deltaTime * rotationSpeed);
        }

        if(transform.position != targetLocation)
        {
            transform.position = Vector3.Lerp(transform.position, targetLocation, Time.deltaTime * movementSpeed);
        }
    }

    public void InitialisePlayer()
    {
        currentTile = Vector2Int.zero;
        directionFacing = Vector2Int.up;
    }


    void OnMove(InputValue _movementValue)
    {

        int movementVector = (int)_movementValue.Get<float>();        
        if (movementVector == 0) return;
        Vector2Int _targetTile = currentTile + (directionFacing * movementVector);
        Debug.Log(_targetTile);
        if (dungeon.IsTileTraversable(_targetTile))
        {
            Debug.Log("Move");
            currentTile = _targetTile;
            targetLocation = new Vector3(_targetTile.x, 0, _targetTile.y);


            aiController.UpdateAI(currentTile);
        }
    }

    void OnTurn(InputValue _turnValue)
    {
        int movementVector = (int)_turnValue.Get<float>();
        if (movementVector == 0) return;

        Quaternion rot = Quaternion.Euler(0, 0, movementVector > 0 ? -90 : 90);
        Vector2 _tempVector = directionFacing;
        _tempVector = rot * _tempVector;
        directionFacing = Vector2Int.RoundToInt(_tempVector);
        rot = Quaternion.Euler(0, movementVector > 0 ? 90 : -90,0 );
        targetDirection = targetDirection * rot;
    }


}
