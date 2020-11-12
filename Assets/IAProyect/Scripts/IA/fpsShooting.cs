using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class fpsShooting : MonoBehaviour, IDamageable
{ 
    ObjPooler pool = default;
    SoundFX sounfx = default;
    bool puedeDisparar = true,puedelanzar = true;
    [SerializeField] float cadenciaDisparo = 0.8f;
    [SerializeField] float shootForce = 5.0f;
    [SerializeField] int salud = 100;
    [SerializeField] Transform shootPoint = default, guntr = default;
    [SerializeField] TextMeshProUGUI ammoText = default, grenadeText = default;
    [SerializeField] Image lifeImg = default;
    [SerializeField] GameObject[] weaponModels = default;
    int currentweapon = 0;
    int nBalas = 15;
    int nCargador = 9, nGran = 3;
    int magazineSize = 9;
    private void Start()
    {
        pool = ObjPooler.instancia;
        sounfx = SoundFX.miSFX;
        updateAmmoCount();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
            Disparar();

        if (Input.GetMouseButton(1))
            LanzarGranada();

        Vector2 scrollamnt = Input.mouseScrollDelta;
        if (scrollamnt.y != 0)
        {
            weaponModels[currentweapon].SetActive(false);
            currentweapon += scrollamnt.y>0?1:-1;
            if (currentweapon > 1) currentweapon = 0;
            else if (currentweapon < 0) currentweapon = 1;
            weaponModels[currentweapon].SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.R) && puedeDisparar)
        {
            Reload();
        }
    }

    private void LanzarGranada()
    {
        if (puedelanzar && nGran > 0)
        {
            puedelanzar = false;
            nGran--;
            grenadeText.text = nGran.ToString();
            GameObject bala = pool.SpawnFromPool("Grenade", shootPoint.position, Quaternion.identity);
            Rigidbody bulletbody = bala.GetComponent<Rigidbody>();
            bulletbody.velocity = Vector3.zero;
            bulletbody.AddForce(shootPoint.forward * shootForce*0.5f, ForceMode.Impulse);
            updateAmmoCount();
            StartCoroutine(launchTimer(1.5f));
        }
    }

    void Disparar()
    {
        if (puedeDisparar && nCargador>0)
        {
            puedeDisparar = false;
            //guntr.DOShakePosition(cadenciaDisparo, -guntr.forward*0.5f, 1, 0, false, true);
            guntr.DOShakeScale(cadenciaDisparo, 0.5f, 1, 90, true);
            nCargador--;
            GameObject bala = pool.SpawnFromPool("Bullet", shootPoint.position, Quaternion.identity);
            Rigidbody bulletbody =bala.GetComponent<Rigidbody>();
            bulletbody.velocity = Vector3.zero;
            bulletbody.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);
            sounfx.PlaySFX(0);
            updateAmmoCount();
            StartCoroutine(shootTimer(cadenciaDisparo));
        }
        else if (nCargador == 0)
        {
            Reload();
        }
    }

    IEnumerator shootTimer(float waitToShoot) 
    {
        yield return new WaitForSeconds(waitToShoot);
        puedeDisparar = true;
    }

    IEnumerator launchTimer(float waitToShoot)
    {
        yield return new WaitForSeconds(waitToShoot);
        puedelanzar = true;
    }

    public void Daniar(int _danio)
    {
        if (salud > 0)
        {
            salud -= _danio;
            float saludporcentaje = salud / 100.0f;
            lifeImg.DOFillAmount(saludporcentaje, 0.2f);
        }
        else
        {
            print("morir");
        }
    }

    public void AddHealth(int healt)
    {
        salud += healt;
        if (salud > 100)
        {
            salud = 100;
        }
        float saludporcentaje = salud / 100.0f;
        lifeImg.DOFillAmount(saludporcentaje, 0.2f);
    }

    void Reload()
    {
        int requiredBullets = magazineSize - nCargador;
        StopAllCoroutines();
        if(requiredBullets>0 && requiredBullets <= nBalas)
        {
            sounfx.PlaySFX(1);
            puedeDisparar = false;
            StartCoroutine(shootTimer(2.0f));
            nCargador = magazineSize;
            nBalas -= requiredBullets;
            updateAmmoCount();
        }
        else if(requiredBullets>nBalas)
        {
            sounfx.PlaySFX(1);
            puedeDisparar = false;
            StartCoroutine(shootTimer(2.0f));
            nCargador = nCargador + nBalas;
            nBalas -= requiredBullets;
            if (nBalas < 0)
                nBalas = 0;
            updateAmmoCount();
        }
    }

    void updateAmmoCount()
    {
        ammoText.text = nBalas + " | " + nCargador;
    }

    public void AddAmmo(int newAmmo)
    {
        nBalas += newAmmo;
        StopAllCoroutines();
        puedeDisparar = true;
        updateAmmoCount();
    }

    public void AddNades(int newAmmo)
    {
        nGran += newAmmo;
        StopAllCoroutines();
        puedelanzar = true;
        grenadeText.text = nGran.ToString();

    }
}
