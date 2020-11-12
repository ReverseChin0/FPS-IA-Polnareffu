using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSpawner : MonoBehaviour
{
    float respawnTime = 5f;
    bool disponible = true;
    [SerializeField] GameObject healthModel = default;
    [SerializeField] int health = 30;

    private void OnTriggerEnter(Collider other)
    {
        if (disponible && other.CompareTag("Player"))
        {
            disponible = false;
            healthModel.SetActive(false);
            other.GetComponent<fpsShooting>().AddHealth(health);
            StartCoroutine(respawn());
        }
    }

    public IEnumerator respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        disponible = true;
        healthModel.SetActive(true);
    }
}
