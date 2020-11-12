using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform[] targets;
    public Transform EnemiesObjectives;
    public GameObject EnemyPrefab;
    public Vector3 SpawnPoint;
    List<EnemieAgent> enemies;
    public int enemyn = 0;
    int totalenemies;
    bool[] ocupado;
    private void Start()
    {
        enemies = new List<EnemieAgent>();
        ocupado = new bool[targets.Length];
        for (int j = 0; j < ocupado.Length; j++)
        {
            ocupado[j] = false;
        }

        for (int i = 0; i < enemyn; i++)
        {
            GameObject GO = Instantiate(EnemyPrefab, SpawnPoint, Quaternion.identity);
            GO.name = GO.name + i.ToString();
            enemies.Add(GO.GetComponent<EnemieAgent>());
            enemies[i].miManager = this;
            enemies[i].InitializeEnemies(targets);
            enemies[i].ObjectiveAim = EnemiesObjectives;
        }
        totalenemies = enemyn;

    }

    public bool RequestPosition(int _r)
    {
        if (ocupado[_r])
        {
            return false;
        }

        ocupado[_r] = true;
        return true;
    }

    public void LeavePosition(int _l)
    {
        ocupado[_l] = false;
    }

    public void RespawnAgent(EnemieAgent ag)
    {

        enemies.Remove(ag);
        Destroy(ag.gameObject);
        GameObject GO = Instantiate(EnemyPrefab, SpawnPoint, Quaternion.identity);
        GO.name = GO.name + totalenemies.ToString();
        EnemieAgent mienemi = GO.GetComponent<EnemieAgent>();
        enemies.Add(mienemi);
        totalenemies++;

        mienemi.GetComponent<EnemieAgent>().miManager = this;
        mienemi.InitializeEnemies(targets);
        mienemi.ObjectiveAim = EnemiesObjectives;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.8f, 0.5f, 0.9f);
        Gizmos.DrawWireSphere(SpawnPoint, 1);
    }
}