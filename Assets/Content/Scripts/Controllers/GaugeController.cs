using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GaugeController : MonoBehaviour
{
    [Header("UIDocument which contains the UXML")]
    public UIDocument uiDocument;

    [Range(0f, 1f)]
    public float value = 1f;

    public float animateDuration = 0.25f;

    private float fuel = 100f;

    private VisualElement fill;
    private Label valueLabel;
    private float currentValue;

    void Start()
    {
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("Add a UI document to game Object or assign it from editor.");
                return;
            }
        }

        var root = uiDocument.rootVisualElement;

        var sheet = Resources.Load<StyleSheet>("UI/HUD_StyleSheet");
        if (sheet != null) root.styleSheets.Add(sheet);

        fill = root.Q<VisualElement>("GaugeFill");
        valueLabel = root.Q<Label>("ValueLabel");

        currentValue = Mathf.Clamp01(value);
        ApplyValue(currentValue);
    }

    public void ConsumeValue(float newValue)
    {
        fuel -= newValue;
        StopAllCoroutines();
        currentValue = Mathf.Clamp01(fuel/100.0f);
        ApplyValue(currentValue);
    }

    public void ConsumeValueAnimated(float targetValue, float duration = -1f)
    {
        fuel -= targetValue;
        if (duration < 0f) duration = animateDuration;
        StopAllCoroutines();
        StartCoroutine(AnimateTo(Mathf.Clamp01(fuel / 100.0f), duration));
    }

    IEnumerator AnimateTo(float target, float duration)
    {
        if (duration <= 0f)
        {
            ConsumeValue(target);
            yield break;
        }

        float start = currentValue;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            currentValue = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t));
            ApplyValue(currentValue);
            yield return null;
        }
        currentValue = target;
        ApplyValue(currentValue);
    }

    void ApplyValue(float v)
    {
        if (fill != null)
        {
            // Assign width in percent
            fill.style.width = new StyleLength(new Length(v * 100f, LengthUnit.Percent));

            // Change color based on thresholds:
            // > 0.5 => green, > 0.25 => yellow, <= 0.25 => red
            Color col;
            if (v > 0.5f)
            {
                col = Color.green;
            }
            else if (v > 0.25f)
            {
                col = Color.yellow;
            }
            else
            {
                col = Color.red;
            }

            fill.style.backgroundColor = new StyleColor(col);
        }
        if (valueLabel != null)
        {
            valueLabel.text = Mathf.RoundToInt(v * 100f) + "%";
        }
    }
}