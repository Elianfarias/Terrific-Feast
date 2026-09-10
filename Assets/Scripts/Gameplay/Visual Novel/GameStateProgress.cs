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
         // Último checkpoint guardado (inicio del encuentro con cada
         // personaje). A diferencia de resumeNode, este no se borra solo:
         // es lo que usa "Continuar" del Main Menu.
         public string checkpointNode = "";

    }
public class GameStateProgress : MonoBehaviour
{
    public SaveData Progreso;

    // Misma ruta para todo el proyecto (ej: UIMainMenu la usa para saber si
    // mostrar el botón "Continuar", sin duplicar el Path.Combine).
    public static string SavePath => Path.Combine(Application.persistentDataPath, "guardado.json");

    // Calculada al vuelo en vez de cachearla en Awake(): otros scripts (ej.
    // YarnComands) pueden llamar a cargarProgreso() desde su propio Awake(),
    // y Unity no garantiza el orden entre Awakes de distintos componentes.
    private string savePath => SavePath;

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