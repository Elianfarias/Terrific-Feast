using System.Data.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class YarnComands : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    //info para guardar 
    [SerializeField] private GameStateProgress data;
    //imagenes de personajes
    [SerializeField] private RawImage osvaldo;
    [SerializeField] private RawImage Tartu_Idle;
    [SerializeField] private RawImage Tartu_Enojado;
    [SerializeField] private RawImage Kerita_Idle;
    [SerializeField] private RawImage Kerita_Enojado;
    [SerializeField] private RawImage Fue_Idle;
    [SerializeField] private RawImage Fue_Enojado;
    [SerializeField] private RawImage Naima_Idle;
    [SerializeField] private RawImage Naima_Enojado;
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

        // Hay que cargar el progreso ACA (antes de que el DialogueRunner
        // arranque solo con autoStart) para poder decirle en qué nodo
        // continuar si venimos de vuelta del minijuego.
        data.cargarProgreso();
        if (!string.IsNullOrEmpty(data.Progreso.resumeNode))
        {
            dialogueRunner.startNode = data.Progreso.resumeNode;
            data.Progreso.resumeNode = "";
            data.guardarProgreso();
        }
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

    [YarnCommand("tartuAparece")]
    public void TartuAparece()
    {
        Tartu_Idle.enabled = true;
    }

    [YarnCommand("tartuDesaparece")]
    public void TartuDesaparece()
    {
        Tartu_Idle.enabled = false;
    }

    [YarnCommand("tartuEAparece")]
    public void TartuEAparece()
    {
        Tartu_Enojado.enabled = true;
    }

    [YarnCommand("tartuEDesaparece")]
    public void TartuEDesaparece()
    {
        Tartu_Enojado.enabled = false;
    }

    [YarnCommand("keritaAparece")]
    public void KeritaAparece()
    {
        Kerita_Idle.enabled = true;
    }

    [YarnCommand("keritaDesaparece")]
    public void KeritaDesaparece()
    {
        Kerita_Idle.enabled = false;
    }

    [YarnCommand("keritaEAparece")]
    public void KeritaEAparece()
    {
        Kerita_Enojado.enabled = true;
    }

    [YarnCommand("keritaEDesaparece")]
    public void KeritaEDesaparece()
    {
        Kerita_Enojado.enabled = false;
    }

    [YarnCommand("fueAparece")]
    public void FueAparece()
    {
        Fue_Idle.enabled = true;
    }

    [YarnCommand("fueDesaparece")]
    public void FueDesaparece()
    {
        Fue_Idle.enabled = false;
    }

    [YarnCommand("fueEAparece")]
    public void FueEAparece()
    {
        Fue_Enojado.enabled = true;
    }

    [YarnCommand("fueEDesaparece")]
    public void FueEDesaparece()
    {
        Fue_Enojado.enabled = false;
    }

    [YarnCommand("naimaAparece")]
    public void NaimaAparece()
    {
        Naima_Idle.enabled = true;
    }

    [YarnCommand("naimaDesaparece")]
    public void NaimaDesaparece()
    {
        Naima_Idle.enabled = false;
    }

    [YarnCommand("naimaEAparece")]
    public void NaimaEAparece()
    {
        Naima_Enojado.enabled = true;
    }

    [YarnCommand("naimaEDesaparece")]
    public void NaimaEDesaparece()
    {
        Naima_Enojado.enabled = false;
    }

    // nodoRegreso: nodo de Yarn donde hay que continuar la novela visual
    // cuando se vuelva del minijuego (ej: "tartu_reaccion").
    [YarnCommand("irAlMinijuego")]
    public void IrAlMinijuego(string nodoRegreso)
    {
        data.Progreso.resumeNode = nodoRegreso;
        data.guardarProgreso();
        SceneManager.LoadScene("Glyph Minigame");
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
