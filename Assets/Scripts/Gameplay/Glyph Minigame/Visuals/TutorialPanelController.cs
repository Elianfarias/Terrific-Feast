using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controla el panel de tutorial: se abre automáticamente la primera vez que
// se entra al minijuego y después queda disponible desde el botón de ayuda.
public class TutorialPanelController : MonoBehaviour
{
    private const string TutorialSeenKey = "GlyphMinigame.TutorialPanel.Seen";

    [Serializable]
    private class TutorialPage
    {
        public string title;
        [TextArea(3, 8)] public string body;
    }

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Text titleText;
    [SerializeField] private Text bodyText;
    [SerializeField] private GameObject nextButtonRoot;
    [SerializeField] private bool showAutomaticallyOnFirstEntry = true;

    [SerializeField]
    private List<TutorialPage> pages = new List<TutorialPage>
    {
        new TutorialPage
        {
            title = "¿Qué hay que hacer?",
            body = "Cada cliente tiene sabores que le gustan y otros que no le gustan nada. " +
                   "Fijate en sus preferencias antes de preparar el trago: podés combinar hasta " +
                   "3 sabores, o servirlo sin nada si eso es lo que pide."
        },
        new TutorialPage
        {
            title = "Invocar el sabor",
            body = "Elegí un sabor y presioná Enter para invocarlo. Eso mata a la libélula, " +
                   "libera su alma, y te deja trazar su glifo para impregnarlo en la bebida."
        },
        new TutorialPage
        {
            title = "Dibujar el glifo",
            body = "El glifo siempre empieza desde el punto marcado al inicio: ese es tu punto " +
                   "de partida. El orden en que toques los siguientes puntos también importa — " +
                   "seguí la secuencia para que el hechizo salga bien. Cuando termines (hasta 3 " +
                   "sabores, o antes si preferís), tocá \"Servir\"."
        }
    };

    private int pageIndex;

    private void Awake()
    {
        if (!showAutomaticallyOnFirstEntry)
        {
            Hide();
            return;
        }

        if (PlayerPrefs.GetInt(TutorialSeenKey, 0) == 0)
        {
            Show();
            PlayerPrefs.SetInt(TutorialSeenKey, 1);
            PlayerPrefs.Save();
        }
        else
        {
            Hide();
        }
    }

    public void Show()
    {
        panelRoot.SetActive(true);
        pageIndex = 0;
        RenderPage();
    }

    public void Hide() => panelRoot.SetActive(false);

    // Avanza a la siguiente página. El botón Next ya se oculta solo cuando
    // no queda ninguna página más (ver RenderPage).
    public void Next()
    {
        if (pageIndex >= pages.Count - 1) return;

        pageIndex++;
        RenderPage();
    }

    private void RenderPage()
    {
        if (pages.Count == 0) return;

        TutorialPage page = pages[pageIndex];
        titleText.text = page.title;
        bodyText.text = page.body;

        if(nextButtonRoot != null)
            nextButtonRoot.SetActive(pageIndex < pages.Count - 1);
    }
}
