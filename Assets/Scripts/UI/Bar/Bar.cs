using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Gradient gradient;

    [SerializeField]
    private Image fill;
    public float animationDuration = 0.5f;

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
        slider.value = value;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetValue(float value)
    {
        StartCoroutine(SetValueCoroutine(value));
    }

    public bool IsMaxValue()
    {
        return slider.maxValue == slider.value;
    }

    IEnumerator SetValueCoroutine(float value)
    {
        float elapsedTime = 0f;
        float firstValue = slider.value;
        float lastValue = value;

        while (elapsedTime < animationDuration)
        {
            slider.value = Mathf.Lerp(firstValue, lastValue, elapsedTime / animationDuration);
            fill.color = gradient.Evaluate(slider.normalizedValue);
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }

        slider.value = value;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}