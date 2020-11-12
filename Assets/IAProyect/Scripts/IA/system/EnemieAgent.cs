using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemieAgent : MonoBehaviour, IDamageable
{
    Transform[] targets;

    CapsuleCollider miColi;

    public NavMeshAgent agent;
    [SerializeField]Animator miAnim;
    [SerializeField] Transform shootPoint;
    [HideInInspector]
    public EnemyManager miManager;
    [HideInInspector]
    public Transform ObjectiveAim;
    

    Vector3 currentDestination;
    int current = 0, ntargets, next;
    public float Distance = 2, rotationspeed = 1f, Accuerror = 2.0f, shootTime = 1.5f;
    bool arrived = false, puedeDisparar = true;
    public bool dead = false;
    float healthpoints = 30f, initialheight, initiialY;


    public void InitializeEnemies(Transform[] _t)
    {
        healthpoints = 30f;
        targets = _t;
        dead = false;
        ntargets = targets.Length;
        next = Random.Range(0, ntargets);
        while (!miManager.RequestPosition(next))
        {
            next = Random.Range(0, ntargets);
        }

        miColi = GetComponent<CapsuleCollider>();
        initialheight = miColi.height;
        initiialY = miColi.center.y;

        current = next;
        currentDestination = targets[next].position;
        agent.SetDestination(currentDestination);
        miAnim.SetBool("moving", true);
        toogleColliderCrouch(false);

    }

    private void Update()
    {
        Vector3 direccion = currentDestination - transform.position;
        if (!dead && direccion.sqrMagnitude < Distance && !arrived)
        {
            arrived = true;
            miAnim.SetBool("moving", false);
            toogleColliderCrouch(true);
            StartCoroutine(Arrived(Random.Range(2.0f, 10.0f)));
        }

    }

    public IEnumerator Arrived(float time)
    {
        if (Random.Range(0, 2) == 0)//0 es disparar, 1 es esperar
        {
            //print("pium "+transform.name);
            yield return new WaitForSeconds(1.0f);
            miAnim.SetBool("Shooting", true);
            toogleColliderCrouch(false);
            transform.LookAt(ObjectiveAim.position);

            if (time - 2.0f > 0.0f)
            {
                StartCoroutine(WaitToShoot((time - 2.0f) * 0.4f));
            }

        }
        yield return new WaitForSeconds(time);
        miAnim.SetBool("Shooting", false);
        toogleColliderCrouch(true);
        GoToDestination();
    }

    public void GoToDestination()
    {
        next = Random.Range(0, ntargets);
        while (!miManager.RequestPosition(next))
        {
            next = Random.Range(0, ntargets);
        }
        miManager.LeavePosition(current);
        arrived = false;
        currentDestination = targets[next].position;
        agent.SetDestination(currentDestination);
        miAnim.SetBool("moving", true);
        toogleColliderCrouch(false);
        current = next;
    }


    public void FuckingDieGodDammit()
    {
        SceneVictoryManager.scInstance.addnMuertos();
        SoundFX.miSFX.PlaySFX(2);
        miAnim.SetTrigger("Die");
        StopAllCoroutines();
        agent.SetDestination(transform.position);
        miManager.LeavePosition(current);
        StartCoroutine(RespawnMePlease(5.0f));
    }

    void toogleColliderCrouch(bool _crouch)
    {
        if (_crouch)
        {
            miColi.height = 1.15f;
            miColi.center = new Vector3(0.0f, -0.3f, 0.0f);
        }
        else
        {
            miColi.height = initialheight;
            miColi.center = new Vector3(0.0f, initiialY, 0.0f);
        }
    }

    public IEnumerator RespawnMePlease(float secs)
    {
        yield return new WaitForSeconds(secs);
        miManager.RespawnAgent(this);
    }

    public IEnumerator WaitToShoot(float time)
    {
        yield return new WaitForSeconds(time);
        Disparar();
    }

    void Disparar()
    {
        if (puedeDisparar)
        {
            puedeDisparar = false;
            GameObject bala = ObjPooler.instancia.SpawnFromPool("Bullet", shootPoint.position, Quaternion.identity);
            Rigidbody bulletbody = bala.GetComponent<Rigidbody>();
            bulletbody.velocity = Vector3.zero;
            float randomAmount = UnityEngine.Random.Range(-15, 15) ;
            Vector3 shootDir = Quaternion.Euler(0, randomAmount, 0) * shootPoint.forward;

            bulletbody.AddForce(shootDir * 5, ForceMode.Impulse);

            StartCoroutine(shootTimer());
        }
    }

    IEnumerator shootTimer()
    {
        yield return new WaitForSeconds(shootTime);
        puedeDisparar = true;
    }

    public void Daniar(int _danio)
    {
        healthpoints -= _danio;
        if (healthpoints <= 0 && !dead)
        {
            dead = true;
            FuckingDieGodDammit();
        }
    }
}


