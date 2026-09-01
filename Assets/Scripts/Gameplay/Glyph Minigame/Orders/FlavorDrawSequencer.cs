using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlavorDrawSequencer : MonoBehaviour
{
    [SerializeField] private FlavorSelectionController selection;
    [SerializeField] private GlyphCastController caster;
    [SerializeField] private SoulCreatureView creature;
    [SerializeField] private SlideDownIntro tablaIntro;
    [SerializeField] private MonsterCustomer targetCustomer;
    [SerializeField] private List<DrinkRecipe> recipeCatalog = new List<DrinkRecipe>();

    [Header("Ayudas (se ocultan mientras se dibuja)")]
    [SerializeField] private GameObject hintButton;
    [SerializeField] private GameObject tutorialButton;

    private readonly List<DrawnFlavorResult> tragoResults = new List<DrawnFlavorResult>();
    private bool drawingGlyph;

    // Se dispara cuando el trago queda servido y bloqueado. Punto de enganche
    // para, más adelante, avanzar a la escena de la novela visual.
    public event System.Action OnTragoServed;

    private void Start()
    {
        if (tablaIntro != null)
            tablaIntro.OnComplete += HandleTablaIntroComplete;
        else
            creature.Revive();
    }

    private void HandleTablaIntroComplete()
    {
        tablaIntro.OnComplete -= HandleTablaIntroComplete;
        creature.Revive();
    }

    private void OnEnable()
    {
        selection.OnFlavorChosen += HandleFlavorChosen;
        caster.OnInvocationResolved += HandleInvocationResolved;
    }

    private void OnDisable()
    {
        selection.OnFlavorChosen -= HandleFlavorChosen;
        caster.OnInvocationResolved -= HandleInvocationResolved;
    }

    // Clickear un sabor ya lo confirma: mata la libélula y arranca el glifo.
    private void HandleFlavorChosen(DrawPattern flavor)
    {
        if (drawingGlyph) return;

        DrinkRecipe recipe = recipeCatalog.FirstOrDefault(r => r.glyph == flavor);
        if (recipe == null)
        {
            Debug.LogWarning($"FlavorDrawSequencer: no hay DrinkRecipe para el glifo {flavor.glyphName}.");
            return;
        }

        drawingGlyph = true;
        selection.gameObject.SetActive(false);
        SetHelpButtonsVisible(false);
        caster.SetRecipe(recipe);
        caster.ReleaseSoul();
    }

    private void SetHelpButtonsVisible(bool visible)
    {
        if (hintButton != null) hintButton.SetActive(visible);
        if (tutorialButton != null) tutorialButton.SetActive(visible);
    }

    // Registra el resultado y sigue con la ronda siguiente. Al llegar al
    // máximo de sabores ya no se sirve solo: se bloquea la selección y se
    // espera a que el jugador aprete "Servir".
    private void HandleInvocationResolved(GameObject result, DrinkRecipe usedRecipe, float accuracy)
    {
        if (usedRecipe != null)
            tragoResults.Add(new DrawnFlavorResult(usedRecipe.glyph, accuracy));

        drawingGlyph = false;
        caster.SetRecipe(null);
        selection.gameObject.SetActive(true);
        SetHelpButtonsVisible(true);

        if (selection.HasReachedMax)
            selection.LockAll();
        else
            creature.Revive();
    }

    // Cierra el trago con lo dibujado hasta ahora, sin esperar a los 3 glifos.
    // Servir sin ningún sabor es válido (hay clientes que lo piden así).
    public void RequestServe()
    {
        EventSystem.current?.SetSelectedGameObject(null);

        if (drawingGlyph) return;

        CloseTrago();
    }

    // Cierra el trago y deja todo bloqueado: el reseteo de slots ya no es
    // automático, queda para cuando arranque un cliente/trago realmente nuevo.
    private void CloseTrago()
    {
        selection.gameObject.SetActive(true);
        selection.LockAll();

        if (targetCustomer != null)
        {
            PreferenceTier reaction = targetCustomer.EvaluateTrago(tragoResults, WandMinigameSession.SelectedLiquid);
            targetCustomer.ReactTo(reaction);
        }

        tragoResults.Clear();
        OnTragoServed?.Invoke();
    }
}
