using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneVictoryManager : MonoBehaviour
{
    public static SceneVictoryManager scInstance;
    int scenesNumber = 5;
    public int nMuertos = 0;
    [SerializeField]int minimoMuertos = 6;
    [SerializeField] TextMeshProUGUI muertostmpro = default;
    private void Awake()
    {
        scInstance = this;
    }
    public void goToScene(string nombre)
   {
        SceneManager.LoadScene(nombre, LoadSceneMode.Single);
   }

    void nextScene()
    {
        int currentindex = SceneManager.GetActiveScene().buildIndex;
        currentindex++;
        if (currentindex > scenesNumber-1)
            currentindex = 0;

            SceneManager.LoadScene(currentindex);
    }

    public void CerrarAplicacion()
    {
        Application.Quit();
    }

    public void addnMuertos()
    {
        nMuertos++;
        if (nMuertos >= minimoMuertos)
            nextScene();

        if(muertostmpro!=null)
        muertostmpro.text = "Enemigos asesinados: " + nMuertos;
    }
}
