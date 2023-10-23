using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public GameObject enemyPrefab;
    public List<EnemyAI> enemies = new List<EnemyAI>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateEnemy(Vector2Int _enemySpawn)
    {
        GameObject _temp = Instantiate(enemyPrefab);
        _temp.GetComponent<EnemyAI>().Init(_enemySpawn);
        enemies.Add(_temp.GetComponent<EnemyAI>());
    }

    public void UpdateAI(Vector2Int _playerPosition)
    {
        foreach (EnemyAI enemy in enemies)
        {
            enemy.FindPath(_playerPosition);
            enemy.MoveAndAttack();
        }
    }

}

public class Node : IComparable<Node>
{
    public Node(Vector2Int _tile, int _g, int _h)
    {
        tile = _tile;
        g = _g;
        h = _h;
    }

    public Vector2Int tile;
    public int g;
    public int h;
    public int f => g + h;

    public int CompareTo(Node other)
    {
        if (other == null) return -1;

        if (other.f != f) return f - other.f;
        if (other.h != h) return h - other.h;
        //if (other.g != g) return other.g - g;

        return 0;
    }
}