using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{
    float respawnTime = 5f;
    bool disponible = true;
    [SerializeField]bool nades = false;
    [SerializeField] GameObject ammoModel = default;
    [SerializeField] int ammo = 30;


    private void OnTriggerEnter(Collider other)
    {
        if (disponible && other.CompareTag("Player"))
        {
            disponible = false;
            ammoModel.SetActive(false);
            if(!nades)
            other.GetComponent<fpsShooting>().AddAmmo(ammo);
            else
                other.GetComponent<fpsShooting>().AddNades(ammo);
            StartCoroutine(respawn());
        }
    }

    public IEnumerator respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        disponible = true;
        ammoModel.SetActive(true);
    }
}
