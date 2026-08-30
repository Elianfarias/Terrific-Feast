using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class YarnComands : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    //info para guardar 
    [SerializeField]private GameState data;
    //imagenes de personajes
    [SerializeField] private RawImage osvaldo;
    [SerializeField] private Personajes personajes;
    //fondos
    [SerializeField] private Fondos fondoActivo;
    //LinePresenter es la clase que representa el sistema que genera el texto en pantalla
    [SerializeField] private LinePresenter linePresenter;
    //------------------------------------Lista de comandos-------------------------------------
    public void Awake()
    {
        dialogueRunner.AddFunction("personajeActual",GetActiveChar);
        dialogueRunner.AddFunction("resultadoMiniJuego",GetMiniGameResult);  
        dialogueRunner.AddFunction("afectoKerita",afectoKerita); 
        dialogueRunner.AddFunction("afectoFire",afectoFire); 
        dialogueRunner.AddFunction("afectoSapo",afectosSapo);
        dialogueRunner.AddFunction("afectoVamp",afectoVamp);  
    }
    //Modificacion de variables
    private int GetTurno()
    {
        return data.Progreso.turno;
    }

    public int GetActiveChar(){
        return data.Progreso.activeChar;
    }

    public bool GetMiniGameResult(){
        return data.Progreso.miniGameResult;
    }

        public bool afectoKerita(){
        return data.Progreso.afectoKerita;
    }
        public bool afectoFire(){
        return data.Progreso.afectoFire;
    }

        public bool afectosSapo(){
        return data.Progreso.afectoSapo;
    }
        public bool afectoVamp(){
        return data.Progreso.afectoVamp;
    }

    [YarnCommand("cambiarFondo")]
    public void CambiarFondo()
    {
        fondoActivo.activateBackground(data.Progreso.turno);
    }
    [YarnCommand("CambiarPersonaje")]
    public void cambiarPersonaje(){
        data.Progreso.activeChar++;
    }
    [YarnCommand("AumentarAfectoKerita")]
    public void aumentarAfectoKerita()
    {
        data.Progreso.afectoKerita=true;
    }
    [YarnCommand("AumentarAfectoSapo")]
    public void aumentarAfectoSapo()
    {
        data.Progreso.afectoSapo=true;
    }

    [YarnCommand("AumentarAfectoFire")]
    public void aumentarAfectoFire()
    {
        data.Progreso.afectoFire=true;
    }

    [YarnCommand("AumentarAfectoVamp")]
    public void aumentarAfectoVamp()
    {
        data.Progreso.afectoVamp=true;
    }

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
        osvaldo.enabled=true;
    }
    
    [YarnCommand("osvaldoDesaparece")]
    public void OsvaldoDesaparece()
    {
        osvaldo.enabled=false;
    }


    [YarnCommand("GuardarProgreso")]
    public void guardarProgreso()
    {
        data.guardarProgreso();
    }

    [YarnCommand("CargarProgreso")]
    public void cargarProgreso()
    {
        data.cargarProgreso();
    }
}
