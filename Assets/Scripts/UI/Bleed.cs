using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bleed : MonoBehaviour
{
    public static Bleed instance;

    public Image bleedOverlay; // Bleed effect overlay

    [Header("Settings")]
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] float stayDuration = 0.3f;
    [SerializeField] float alpha = 0.6f;

    private Coroutine fadeCoroutine; // To manage fading effect

    private void Awake()
    {
        if(instance == null) instance = this;
        bleedOverlay = GetComponent<Image>();
        if (bleedOverlay != null) bleedOverlay.color = new Color(bleedOverlay.color.r, bleedOverlay.color.g, bleedOverlay.color.b, 0);
    }

    public void ShowBleed()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(ShowBleedEffect());
    }

    private IEnumerator ShowBleedEffect()
    {
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float newAlpha = Mathf.Lerp(0, alpha, t / fadeDuration);
            bleedOverlay.color = new Color(bleedOverlay.color.r, bleedOverlay.color.g, bleedOverlay.color.b, newAlpha);
            yield return null;
        }
        bleedOverlay.color = new Color(bleedOverlay.color.r, bleedOverlay.color.g, bleedOverlay.color.b, alpha);

        yield return new WaitForSeconds(stayDuration);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float newAlpha = Mathf.Lerp(alpha, 0, t / fadeDuration);
            bleedOverlay.color = new Color(bleedOverlay.color.r, bleedOverlay.color.g, bleedOverlay.color.b, newAlpha);
            yield return null;
        }
        bleedOverlay.color = new Color(bleedOverlay.color.r, bleedOverlay.color.g, bleedOverlay.color.b, 0);
    }
}
