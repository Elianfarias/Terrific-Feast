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
        caster.OnEnterRequested += HandleEnterRequested;
        caster.OnInvocationResolved += HandleInvocationResolved;
    }

    private void OnDisable()
    {
        caster.OnEnterRequested -= HandleEnterRequested;
        caster.OnInvocationResolved -= HandleInvocationResolved;
    }

    // Confirma el sabor seleccionado y arranca esa ronda de dibujo.
    private void HandleEnterRequested()
    {
        if (drawingGlyph) return;
        if (!selection.TryConfirmSelection(out DrawPattern flavor)) return;

        DrinkRecipe recipe = recipeCatalog.FirstOrDefault(r => r.glyph == flavor);
        if (recipe == null)
        {
            Debug.LogWarning($"FlavorDrawSequencer: no hay DrinkRecipe para el glifo {flavor.glyphName}.");
            return;
        }

        drawingGlyph = true;
        selection.gameObject.SetActive(false);
        caster.SetRecipe(recipe);
        caster.ReleaseSoul();
    }

    private void HandleInvocationResolved(GameObject result, DrinkRecipe usedRecipe, float accuracy)
    {
        if (usedRecipe != null)
            tragoResults.Add(new DrawnFlavorResult(usedRecipe.glyph, accuracy));
    }

    // Sigue con la ronda siguiente, o cierra el trago si ya llegó al máximo.
    private void HandleBannerDismissed()
    {
        drawingGlyph = false;
        caster.SetRecipe(null);

        if (selection.HasReachedMax)
            CloseTrago();
        else
        {
            selection.gameObject.SetActive(true);
            creature.Revive();
        }
    }

    // Cierra el trago con lo dibujado hasta ahora, sin esperar a los 3 glifos.
    public void RequestServe()
    {
        EventSystem.current?.SetSelectedGameObject(null);

        if (drawingGlyph) return;
        if (tragoResults.Count == 0) return;

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
            PreferenceTier reaction = targetCustomer.EvaluateTrago(tragoResults);
            targetCustomer.ReactTo(reaction);
        }

        tragoResults.Clear();
        OnTragoServed?.Invoke();
    }
}
