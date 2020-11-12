using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    ObjPooler pool = default;
    float lifeTime = 3;
    bool canDmg = false;

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
            IDamageable dmg = collision.collider.GetComponent<IDamageable>();
            if (dmg != null)
            {
                //print("damn");
                dmg.Daniar(5);
            }
            else
            {
                foreach (var c in FindObjectsOfType<IA_Enemigo>())
                {
                    c.IniciarInvestigacion(transform.position);
                    StopAllCoroutines();
                }
            }       
        }
        gameObject.SetActive(false);
        GameObject part = pool.SpawnFromPool("bulletparticles", transform.position, Quaternion.identity);
        part.GetComponent<ParticleSystem>().Play();
    }

    IEnumerator lifetimeRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        GameObject part = pool.SpawnFromPool("bulletparticles", transform.position, Quaternion.identity);
        part.GetComponent<ParticleSystem>().Play();
        gameObject.SetActive(false);
        
    }

}
