using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<Enemy> enemies = new();

    [SerializeField]
    private GameObject key;

    private bool isKeyInstantiate;

    void Start()
    {
        enemies = GameObject.FindGameObjectWithTag(Tag.Enemies.ToString()).GetComponentsInChildren<Enemy>().ToList();
    }

    public void RemoveEnemy(Enemy enemy, Vector3 keyPosition)
    {
        enemies.Remove(enemy);
        if (enemies.Count == 0 && !isKeyInstantiate)
        {
            Instantiate(key, keyPosition, Quaternion.identity);
            isKeyInstantiate = true;
        }
    }
}
