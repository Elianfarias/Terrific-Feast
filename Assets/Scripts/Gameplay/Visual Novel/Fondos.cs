using System.Runtime.CompilerServices;
using UnityEngine;

public class Fondos : MonoBehaviour
{
    public void activateBackground(int turno)
    {
        if (transform.childCount-1 > turno || turno<0)
        {
            return;
        }

        transform.GetChild(turno).gameObject.SetActive(true);
    }
}

