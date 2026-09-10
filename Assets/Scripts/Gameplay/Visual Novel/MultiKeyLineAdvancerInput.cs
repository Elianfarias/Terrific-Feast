using UnityEngine;
using Yarn.Unity;

// El LineAdvancerInput.KeyCodes de Yarn Spinner solo admite una tecla por
// acción. Este componente permite click izquierdo, Enter y Espacio, todos
// disparando lo mismo que "hurry up / avanzar línea".
[RequireComponent(typeof(LineAdvancer))]
public class MultiKeyLineAdvancerInput : MonoBehaviour, ILineAdvancerInput
{
    [SerializeField] private LineAdvancer lineAdvancer;
    [SerializeField] private bool advanceOnLeftClick = true;
    [SerializeField] private KeyCode[] advanceKeys = { KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Space };

    public LineAdvancer LineAdvancer { get => lineAdvancer; set => lineAdvancer = value; }

    private void Update()
    {
        if (lineAdvancer == null) return;

        if (advanceOnLeftClick && Input.GetMouseButtonDown(0))
        {
            lineAdvancer.OnInputHurryUpLines();
            return;
        }

        foreach (KeyCode key in advanceKeys)
        {
            if (Input.GetKeyDown(key))
            {
                lineAdvancer.OnInputHurryUpLines();
                return;
            }
        }
    }

    public void OnDialogueStarted() { }
    public void OnDialogueComplete() { }
}
