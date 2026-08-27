using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
    
public class YarnComands : MonoBehaviour
{
    [SerializeField] private RawImage imagen;
    //LinePresenter es la clase que representa el sistema que genera el texto en pantalla
    [SerializeField] private LinePresenter linePresenter;

//------------------------------------Lista de comandos-------------------------------------

//Modo para pasar lineas de texto automaticamente 
    [YarnCommand("activarAuto")]
    public void ActivarAuto()
    {
        linePresenter.autoAdvance = true;
    }

    [YarnCommand("desactivarAuto")]
    public void DesactivarAuto()
    {
        linePresenter.autoAdvance = false;
    }
//Comandos para modificar posicion y apriciones de personajes
   [YarnCommand("osvaldoAparece")]
    public void OsvaldoAparece()
    {
        imagen.enabled=true;

    }
    
    [YarnCommand("osvaldoDesaparece")]
    public void OsvaldoDesaparece()
    {
        imagen.enabled=false;

    }
}
