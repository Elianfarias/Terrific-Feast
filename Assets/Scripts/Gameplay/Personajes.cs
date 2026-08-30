using UnityEngine;

public class Personajes : MonoBehaviour
{
    public void activarPersonaje(int id)
    {
        foreach(Transform pj in transform)
        {
            Personaje pjAct= pj.GetComponent<Personaje>();
            
            if(pjAct != null)
            {
                pj.gameObject.SetActive(pjAct.id==id);
            }
        }
    }
}
