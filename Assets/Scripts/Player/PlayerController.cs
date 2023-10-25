using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    BPMController bpmController;

    public int health = 10;

    public GameObject dieUI;
    public Slider healthBar;
    // Start is called before the first frame update
    void Start()
    {
        dungeon = UnityEngine.Object.FindObjectOfType<DungeonGenerator>();
        aiController = UnityEngine.Object.FindObjectOfType<AIController>();
        bpmController = UnityEngine.Object.FindObjectOfType<BPMController>();
        targetDirection = Quaternion.identity;
        healthBar.maxValue = health;
        healthBar.value = health;
    }
    // Update is called once per frame
    void Update()
    {

        DDRPadInputCheck();
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                Debug.Log("KeyCode down: " + kcode);
        }
        if (Input.anyKeyDown)
        {

            foreach (var item in InputSystem.devices)
            {
                Debug.Log(item.displayName);
            }
            //Debug.Log(InputSystem.devices[1].); 

        }
        if (transform.rotation != targetDirection)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetDirection, Time.deltaTime * rotationSpeed);
        }

        if(transform.position != targetLocation)
        {
            transform.position = Vector3.Lerp(transform.position, targetLocation, Time.deltaTime * movementSpeed);
        }
    }

    private void DDRPadInputCheck()
    {
        //Left Pad Left Arrow
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            OnTurn(-1);
        }
        //Left Pad Right Arrow
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            OnTurn(1);
        }
        //Left Pad Up Arrow
        if (Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            OnMove(1);
        }

        //Left Pad Back Arrow
        if (Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            OnMove(-1);
        }
        //Left Pad Start Button
        if (Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
        }
        //Left Pad Select Button
        if (Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
        }

        //Right Pad Left Arrow
        if (Input.GetKeyDown(KeyCode.Joystick1Button6))
        {
            OnAttack(-1);
        }
        //Right Pad Right Arrow
        if (Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            OnAttack(1);
        }
        //Right Pad Up Arrow
        if (Input.GetKeyDown(KeyCode.Joystick1Button8))
        {
            //Interact Code
        }

        //Right Pad Back Arrow
        if (Input.GetKeyDown(KeyCode.Joystick1Button9))
        {
            OnWait();
        }
        //Right Pad Start Button
        if (Input.GetKeyDown(KeyCode.Joystick1Button10))
        {
        }
        //Right Pad Select Button
        if (Input.GetKeyDown(KeyCode.Joystick1Button11))
        {
        }
    }

    public void InitialisePlayer()
    {
        currentTile = Vector2Int.zero;
        directionFacing = Vector2Int.up;
    }

    public void TakeDamage(int _damage)
    {
        health -= _damage;
        healthBar.value = health;
        if(health <= 0)
        {
            Die();
        }
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

        OnMove(movementVector);
    }

    void OnTurn(InputValue _turnValue)
    {
        int movementVector = (int)_turnValue.Get<float>();
        if (movementVector == 0) return;

        OnTurn(movementVector);
    }

    void OnAttack(InputValue _attackValue)
    {

        int attackVector = (int)_attackValue.Get<float>();
        if (attackVector == 0) return;

        OnAttack(attackVector);

    }

    void OnWait(InputValue _waitValue)
    {
        if(_waitValue.isPressed)
        {
            Debug.Log("Wait");
            aiController.UpdateAI(currentTile);
        }
    }

    public void OnTurn(int _turnValue)
    {
        Quaternion rot = Quaternion.Euler(0, 0, _turnValue > 0 ? -90 : 90);
        Vector2 _tempVector = directionFacing;
        _tempVector = rot * _tempVector;
        directionFacing = Vector2Int.RoundToInt(_tempVector);
        rot = Quaternion.Euler(0, _turnValue > 0 ? 90 : -90, 0);
        targetDirection = targetDirection * rot;
    }

    public void OnMove(int _moveValue)
    {
        Vector2Int _targetTile = currentTile + (directionFacing * _moveValue);
        Debug.Log(_targetTile);
        if (dungeon.IsTileTraversable(_targetTile))
        {
            Debug.Log("Move");
            currentTile = _targetTile;
            targetLocation = new Vector3(_targetTile.x, 0, _targetTile.y);
            if (currentTile == dungeon.floorList.ElementAt(dungeon.floorList.Count - 1).Value.Location)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            aiController.UpdateAI(currentTile);
        }
    }

    public void OnAttack(int _attackValue)
    {
        Debug.Log("Attack");
        aiController.CheckAIAttack(currentTile + directionFacing, 5);
        aiController.UpdateAI(currentTile);
    }

    public void OnWait()
    {
        Debug.Log("Wait");
        aiController.UpdateAI(currentTile);
    }
}
