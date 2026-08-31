using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
[System.Serializable]
    public class SaveData
    {
         public int activeChar;
         public int turno;
         public bool miniGameResult;
         public bool afectoVamp;
         public bool afectoSapo;
         public bool afectoFire;
         public bool afectoKerita;
         // Nodo de Yarn al que hay que volver al recargar la novela visual
         // después del minijuego (vacío = arrancar normal desde "Start").
         public string resumeNode = "";

    }
public class GameStateProgress : MonoBehaviour
{
    public SaveData Progreso;

    // Calculada al vuelo en vez de cachearla en Awake(): otros scripts (ej.
    // YarnComands) pueden llamar a cargarProgreso() desde su propio Awake(),
    // y Unity no garantiza el orden entre Awakes de distintos componentes.
    private string savePath => Path.Combine(Application.persistentDataPath,"guardado.json");

    private void Awake(){
        if (Progreso == null) Progreso = new SaveData();
    }

    public void guardarProgreso(){
        string json=JsonUtility.ToJson(Progreso,true);
        File.WriteAllText(savePath,json);
        Debug.Log("guardado en:"+ savePath);
    }

    public void cargarProgreso(){

        if (!File.Exists(savePath)){
            Debug.Log("No hay datos guardados");
            return;
        }
        string json=File.ReadAllText(savePath);
        Progreso=JsonUtility.FromJson<SaveData>(json);
    }
}