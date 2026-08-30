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

    }
public class GameState : MonoBehaviour
{
    public SaveData Progreso;
    private string savePath;

    private void Awake(){
        savePath=Path.Combine(Application.persistentDataPath,"guardado.json");
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