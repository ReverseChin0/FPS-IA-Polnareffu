using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    ObjPooler pool = default;
    float lifeTime = 3;
    bool canDmg = false;
    public LayerMask explodable;
    private void Awake()
    {
        pool = ObjPooler.instancia;
    }
    private void OnEnable()
    {
        canDmg = true;
        StartCoroutine(lifetimeRoutine());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canDmg)
        {
            canDmg = false;

            foreach (var c in FindObjectsOfType<IA_Enemigo>())
            {
                c.IniciarHuidaTemporal(transform.position);
            }
        }
    }

    IEnumerator lifetimeRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        Collider[] explodables = Physics.OverlapSphere(transform.position, 5, explodable);
        foreach (var item in explodables)
        {
            item.GetComponent<IDamageable>().Daniar(30);
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce((item.transform.position - transform.position).normalized * 10.0f + Vector3.up *5.0f);
            }
        }
        SoundFX.miSFX.PlaySFX(3);
        GameObject part = pool.SpawnFromPool("grenadeparticles", transform.position, Quaternion.identity);
        part.GetComponent<ParticleSystem>().Play();
        gameObject.SetActive(false);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 5);
    }

}
