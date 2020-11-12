using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Enemigo : MonoBehaviour, IDamageable
{
    [SerializeField] Rigidbody rb = default;
    [SerializeField] Transform tr = default;
    [SerializeField] Transform pltr = default;
    [SerializeField] Transform shootPoint = default;
    [SerializeField] Transform[] wayPoints = default;
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float shootTime = 1.5f;
    [SerializeField] int salud = 100;
    [SerializeField] bool patrullarPuntos = true;
    [SerializeField] bool huye = true;
    [SerializeField] Animator anim = default;
    public bool isStronk = true;
    [SerializeField] myEnums.Estado edoActual = myEnums.Estado.patrullar;
    [SerializeField] Vector2 angleShoot = default;
    int waypointIndex = 0;
    int waypointLength = 0;
    bool puedeDisparar = true, puedeMoverse = true;
    ObjPooler pool = default;
    Vector3 direccion = default;
    Vector3 ultimaDireccion = default;
    Vector3 puntoInteres = default;
    public float distanciaPersecusion = 10.0f, distanciaMinima = 1.0f;
    public bool muerto = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tr = transform;
        GameObject[] GOwayPoints = GameObject.FindGameObjectsWithTag("wayPoints");
        waypointLength = GOwayPoints.Length;
        wayPoints = new Transform[waypointLength];
        for (int i = 0; i < waypointLength; i++)
        {
            wayPoints[i] = GOwayPoints[i].transform;
        }
    }

    private void Start()
    {
        pool = ObjPooler.instancia;
    }

    private void Update()
    {
        if (!muerto)
        {
            switch (edoActual)
            {
                case myEnums.Estado.patrullar:
                    Patrullar(); break;
                case myEnums.Estado.atacar:
                    Perseguir(); break;
                case myEnums.Estado.investigar:
                    Investigar(); break;
                case myEnums.Estado.huir:
                    if (huye)
                    {
                        Huir();
                    }
                    break;
                default: break;
            }
        }

        if (direccion != Vector3.zero)
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(direccion), Time.deltaTime * 15f);
    }

    private void Patrullar()
    {
        if (patrullarPuntos)
        {
            direccion = wayPoints[waypointIndex].position - tr.position;
            float distanciaPuntoSqr = direccion.sqrMagnitude;
            if (distanciaPuntoSqr > distanciaMinima*distanciaMinima && puedeMoverse)
            {//caminar
                anim.SetInteger("state", 1);
                direccion.Normalize();
                direccion.y = 0;
                ultimaDireccion = direccion;
                //tr.rotation = Quaternion.LookRotation(direccion, Vector3.up);
            }
            else
            {//parar
                if (puedeMoverse)
                {
                    anim.SetInteger("state", 0);
                    puedeMoverse = false;
                    StartCoroutine(moveAgainTimer());
                    waypointIndex++;
                    if (waypointIndex >= waypointLength)
                        waypointIndex = 0;
                }
                direccion = Vector3.zero;
                ultimaDireccion = direccion;
            }

        }
        else
        {
            if (puedeMoverse)
            {
                puedeMoverse = false;
                StartCoroutine(MovimientoRandom());
            }
        }

        trySpotPlayer();
    }

    void trySpotPlayer()
    {
        Vector3 playerdirection = tr.position - pltr.position;
        if (playerdirection.sqrMagnitude < distanciaPersecusion*distanciaPersecusion)
        {//si esta en rango de persecucion
            //si esta en angulo de vision de su cono de 90°
            if (Vector3.SignedAngle(tr.forward, pltr.position,Vector3.up) < 45.0f)
            {//DETECTADO
                StopAllCoroutines();
                puedeMoverse = true;
                edoActual = myEnums.Estado.atacar;
            }
        }
    }
    

    void Perseguir()
    {
        direccion = pltr.position - tr.position;
        
        if (direccion.sqrMagnitude > distanciaMinima * distanciaMinima)
        {//seguir
            anim.SetInteger("state", 3); 
            direccion.Normalize();
            direccion.y = 0;
            ultimaDireccion = direccion;
            //tr.rotation = Quaternion.LookRotation(direccion, Vector3.up);
            Disparar(1);
        }
        else
        {//mirar
            anim.SetInteger("state", 2);
            direccion.y = 0;
            ultimaDireccion = direccion;
            //tr.rotation = Quaternion.LookRotation(direccion, Vector3.up);
            direccion = Vector3.zero;
            Disparar(0);
        }
    }

    public void IniciarInvestigacion(Vector3 puntoaInvestigar)
    {
        if (!muerto)
        {
            Vector3 distance = tr.position - puntoaInvestigar;
            if (distance.sqrMagnitude < distanciaPersecusion * distanciaPersecusion && edoActual != myEnums.Estado.atacar)
            {
                edoActual = myEnums.Estado.investigar;
                puntoInteres = puntoaInvestigar;
                puedeMoverse = true;
            }
        }
        
    }

    void Investigar()
    {
        direccion = puntoInteres - tr.position;
        float distanciaPuntoSqr = direccion.sqrMagnitude;
        if (distanciaPuntoSqr > 6 && puedeMoverse)
        {
            direccion.Normalize();
            direccion.y = 0;
            ultimaDireccion = direccion;
            //tr.rotation = Quaternion.LookRotation(direccion, Vector3.up);
        }
        else
        {
            if (puedeMoverse)
            {
                puedeMoverse = false;
                StartCoroutine(changeState(myEnums.Estado.patrullar, 2.0f));
            }
            direccion = Vector3.zero;
        }

        trySpotPlayer();
    }

    public void IniciarHuidaTemporal(Vector3 puntoAHuir)
    {
        if (!muerto)
        {
            direccion = tr.position - puntoAHuir;
            if(direccion.sqrMagnitude < distanciaPersecusion * distanciaPersecusion)
            {

                anim.SetInteger("state", 1);
                edoActual = myEnums.Estado.indefinido;
                puntoInteres = puntoAHuir;
                direccion.y = 0;
                direccion.Normalize();
                //tr.rotation = Quaternion.LookRotation(direccion, Vector3.up);
                StartCoroutine(changeState(myEnums.Estado.patrullar, 2.0f));
            }
            
        }
    }

    void Huir()
    {
        Vector3 Actualdireccion = pltr.position - tr.position;
        float distanciaSqr = Actualdireccion.sqrMagnitude;
        if (distanciaSqr < distanciaPersecusion * distanciaPersecusion)
        {
            direccion = Actualdireccion * -1f;
            direccion.y = 0;
            direccion.Normalize();
            ultimaDireccion = direccion;
            //tr.rotation = Quaternion.LookRotation(direccion, Vector3.up);
        }
        else
        {
            direccion = Vector3.zero;
        }
    }

    void Disparar(int _estado)
    {
        if (puedeDisparar)
        {
            puedeDisparar = false;
            GameObject bala = pool.SpawnFromPool("Bullet", shootPoint.position, Quaternion.identity);
            Rigidbody bulletbody = bala.GetComponent<Rigidbody>();
            bulletbody.velocity = Vector3.zero;
            float randomAmount = _estado == 0 ? UnityEngine.Random.Range(-angleShoot.x, angleShoot.x) : UnityEngine.Random.Range(-angleShoot.y, angleShoot.y);
            Vector3 shootDir = Quaternion.Euler(0, randomAmount, 0) * shootPoint.forward;

            bulletbody.AddForce(shootDir * 5, ForceMode.Impulse);

            StartCoroutine(shootTimer());
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(tr.position + (direccion * moveSpeed * Time.fixedDeltaTime));
    }


    IEnumerator moveAgainTimer()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(2, 5));
        puedeMoverse = true;
    }

    IEnumerator MovimientoRandom()
    {
        direccion = (tr.position + UnityEngine.Random.insideUnitSphere.normalized) - tr.position;
        direccion.y = 0;
        ultimaDireccion = direccion;
       // tr.rotation = Quaternion.LookRotation(direccion, Vector3.up);
        yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));
        direccion = Vector3.zero;
        ultimaDireccion = direccion;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));
        puedeMoverse = true;
    }

    public IEnumerator changeState(myEnums.Estado _edo, float _timetochange)
    {
        yield return new WaitForSeconds(2f);
        edoActual = _edo;
        puedeMoverse = true;
    }

    IEnumerator shootTimer()
    {
        yield return new WaitForSeconds(shootTime);
        puedeDisparar = true;
    }

    public void Daniar(int _danio)
    {
        if(edoActual!=myEnums.Estado.indefinido)
            edoActual = myEnums.Estado.atacar;

        if (salud > 0)
        {
            salud -= _danio;
            if(salud<25 && huye)
            {
                edoActual = myEnums.Estado.huir;
            }
        }
        else
        {
            if (isStronk) 
            {
                foreach (var IA in FindObjectsOfType<IA_Enemigo>())
                {
                    if (!IA.isStronk)
                    {
                        IA.IniciarHuidaTemporal(pltr.position);
                    }
                }
            }
            puntoInteres = tr.position;
            edoActual = myEnums.Estado.muerto;
            anim.SetBool("alive", false);
            direccion = Vector3.zero;
            muerto = true;
            GetComponent<Collider>().enabled = false;
            rb.isKinematic = true;
            SoundFX.miSFX.PlaySFX(2);
            SceneVictoryManager.scInstance.addnMuertos();

            //gameObject.SetActive(false);
        }
            
    }
}
