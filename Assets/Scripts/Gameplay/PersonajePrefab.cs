using UnityEngine;
using UnityEngine.UI;

public class Personaje : MonoBehaviour
{
/*
------------------ID FIJOS--------------------------------- 
    0.Tartu
    1.Kerita
    2.Fue
    3.Naima
*/
    public int id;
    [SerializeField] private RawImage Idle;
    [SerializeField] private RawImage Enojado;

    public void startCharacter()
    {
        Idle.enabled=true;
        Enojado.enabled=false;
    }

    public void madCharacter()
    {
        Enojado.enabled=true;
        Idle.enabled=false;
    }
}
