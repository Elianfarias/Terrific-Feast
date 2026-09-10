using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class PatienceUIBuilder
{
    private const string CanvasChildName = "Patience Canvas";

    // Crea (o reutiliza) el Slider de paciencia para cada MonsterCustomer de la escena.
    [MenuItem("Magic/Build Patience UI For All Customers")]
    public static void BuildForAllCustomers()
    {
        MonsterCustomer[] customers = Object.FindObjectsByType<MonsterCustomer>(FindObjectsSortMode.None);
        int created = 0;

        foreach (var customer in customers)
        {
            if (BuildForCustomer(customer))
                created++;
        }

        Debug.Log($"Patience UI: {created} creadas, {customers.Length - created} ya tenían una.");
    }

    private static bool BuildForCustomer(MonsterCustomer customer)
    {
        Transform existing = customer.transform.Find(CanvasChildName);
        if (existing != null)
        {
            AssignSlider(customer, existing.GetComponentInChildren<Slider>());
            return false;
        }

        GameObject canvasGO = new GameObject(CanvasChildName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Patience Canvas");
        canvasGO.transform.SetParent(customer.transform, false);
        canvasGO.transform.localPosition = new Vector3(0f, 1.2f, 0f);

        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(100f, 16f);
        canvasGO.transform.localScale = Vector3.one * 0.01f;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;

        GameObject background = CreateUIImage("Background", canvasGO.transform, new Color(0.1f, 0.1f, 0.1f, 0.85f));
        StretchFull(background.GetComponent<RectTransform>());

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(fillArea, "Create Patience Canvas");
        fillArea.transform.SetParent(canvasGO.transform, false);
        StretchFull(fillArea.GetComponent<RectTransform>());

        GameObject fill = CreateUIImage("Fill", fillArea.transform, new Color(0.3f, 0.85f, 0.4f, 1f));
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        Slider slider = canvasGO.AddComponent<Slider>();
        slider.transition = Selectable.Transition.None;
        slider.interactable = false;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        slider.fillRect = fillRect;
        slider.handleRect = null;
        slider.targetGraphic = null;

        AssignSlider(customer, slider);
        return true;
    }

    private static GameObject CreateUIImage(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(go, "Create Patience Canvas");
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        return go;
    }

    private static void StretchFull(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void AssignSlider(MonsterCustomer customer, Slider slider)
    {
        if (slider == null) return;

        SerializedObject so = new SerializedObject(customer);
        so.FindProperty("patienceBar").objectReferenceValue = slider;
        so.ApplyModifiedProperties();
    }
}
