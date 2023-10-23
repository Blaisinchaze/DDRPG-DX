using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    public int health = 10;

    public GameObject dieUI;
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

    public void Die()
    {
        dieUI.SetActive(true);
        StartCoroutine(DeathScreenTimer());
    }

    IEnumerator DeathScreenTimer()
    {
        yield return new WaitForSeconds(8f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnMove(InputValue _movementValue)
    {
        Debug.Log("move pressed");
        int movementVector = (int)_movementValue.Get<float>();        
        if (movementVector == 0) return;
        Vector2Int _targetTile = currentTile + (directionFacing * movementVector);
        Debug.Log(_targetTile);
        if (dungeon.IsTileTraversable(_targetTile))
        {
            Debug.Log("Move");
            currentTile = _targetTile;
            targetLocation = new Vector3(_targetTile.x, 0, _targetTile.y);
            if(currentTile == dungeon.floorList.ElementAt(dungeon.floorList.Count - 1).Value.Location)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

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

    void OnAttack(InputValue _attackValue)
    {

        int attackVector = (int)_attackValue.Get<float>();
        if (attackVector == 0) return;


        Debug.Log("Attack");
        aiController.CheckAIAttack(currentTile + directionFacing, 5);
        aiController.UpdateAI(currentTile);

    }

    void OnWait(InputValue _waitValue)
    {
        if(_waitValue.isPressed)
        {
            Debug.Log("Wait");
            aiController.UpdateAI(currentTile);
        }
    }


}
