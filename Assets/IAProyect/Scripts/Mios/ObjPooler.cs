using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPooler : MonoBehaviour
{
    //242023
    [System.Serializable]
    public class Pool
    {
        public string etiqueta;
        public GameObject prefab;
        public int tamanio;
    }

    #region Singleton
    //==============Singleton=========
    public static ObjPooler instancia;

    private void Awake()
    {
        instancia = this;
    }
    //=================================
    #endregion

    public List<Pool> ConjuntoPools;
    public Dictionary<string, List<GameObject>> poolDiccionario;

    //Al principio
    void Start()
    {
        //Hace nuevo apartado en el diccionario
        poolDiccionario = new Dictionary<string, List<GameObject>>();

        //para cada pool en la lista de pools
        foreach (Pool pool in ConjuntoPools)
        {
            //crea un Queue (fila)
            List<GameObject> objectPool = new List<GameObject>();

            //itera n veces y spawnea y desactiva los objetos
            for (int i = 0; i < pool.tamanio; i++)
            {
                GameObject objetillo = Instantiate(pool.prefab);
                //objetillo.SetActive(false);
                
                objetillo.SetActive(false);
                
                objectPool.Add(objetillo);
            }
            //Añade este pool al diccionario
            poolDiccionario.Add(pool.etiqueta, objectPool);
        }
    }

    IEnumerator DesactivarObjeto(GameObject _go, float _tiempo)
    {
        yield return new WaitForSeconds(_tiempo);
        _go.SetActive(false);
    }

    public GameObject SpawnFromPool(string tag, Vector3 posicion, Quaternion rotacion)
    {
        //Pa que no existan errores por pasar tag invalido
        if (!poolDiccionario.ContainsKey(tag))
        {
            Debug.LogWarning("El Pool con tag " + tag + " no existe");
            return null;
        }
        //=========================================================\\

        //GameObject ObjetoaSpawnear = poolDiccionario[tag].Dequeue();
        int _i = 0;
        bool HayDesactivada = false;
        for (_i = 0; _i < poolDiccionario[tag].Count; _i++)
        {
            if (!poolDiccionario[tag][_i].activeSelf)
            {
                HayDesactivada = true;
                break;
            }
        }

        GameObject ObjetoaSpawnear;

        if (HayDesactivada)
        {
            ObjetoaSpawnear = poolDiccionario[tag][_i];
            //Activamos al chico
            ObjetoaSpawnear.SetActive(true);
            ObjetoaSpawnear.transform.position = posicion;
            ObjetoaSpawnear.transform.rotation = rotacion;
        }
        else 
        {
            //Creamos a un nuevo chico
            ObjetoaSpawnear = Instantiate(poolDiccionario[tag][_i - 1]);
            ObjetoaSpawnear.SetActive(true);
            ObjetoaSpawnear.transform.position = posicion;
            ObjetoaSpawnear.transform.rotation = rotacion;
            poolDiccionario[tag].Add(ObjetoaSpawnear);
        }

        if (tag == "bulletparticles" || tag == "grenadeparticles")
            StartCoroutine(DesactivarObjeto(ObjetoaSpawnear, 1));
        return ObjetoaSpawnear;
    }
}
