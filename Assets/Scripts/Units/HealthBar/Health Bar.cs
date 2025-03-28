using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Health unit;
    [SerializeField] private Image bar;
    [SerializeField] private Image fill;
    private RectTransform rectTransform;

    [SerializeField] private float minScale = 0.5f; // Smallest size of the health bar
    [SerializeField] private float maxScale = 1.2f; // Largest size of the health bar
    [SerializeField] private float maxDistance = 20f; // Distance at which scaling stops
    [SerializeField] private float fadeSpeed = 5f;

    public void Initialize(Health un)
    {
        this.unit = un;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (unit == null) return;

        // Get the current active camera
        Camera activeCamera = GameStateManager.Instance.currentCamera;
        if (activeCamera == null) return; // Safety check

        Vector3 worldPosition = unit.transform.position + Vector3.up * 2;
        Vector3 screenPosition = activeCamera.WorldToScreenPoint(worldPosition);

        // Check if the enemy is in front of the camera
        bool isVisible = screenPosition.z > 0 && screenPosition.x > 0 && screenPosition.x < Screen.width && screenPosition.y > 0 && screenPosition.y < Screen.height;

        if (isVisible)
        {
            transform.position = screenPosition;

            // Distance-based scaling
            float distance = Vector3.Distance(activeCamera.transform.position, unit.transform.position);
            float scale = Mathf.Lerp(maxScale, minScale, distance / maxDistance);
            rectTransform.localScale = new Vector3(scale, scale, scale);

            // Smoothly fade in
            LerpBarAlpha(1f);
        }
        else
        {
            // Smoothly fade out
            LerpBarAlpha(0f);
        }
    }

    private void LerpBarAlpha(float towards)
    {
        Color barColor = bar.color;
        Color fillColor = fill.color;
        barColor.a = Mathf.MoveTowards(barColor.a, towards, Time.deltaTime * fadeSpeed);
        fillColor.a = barColor.a;
        bar.color = barColor;
        fill.color = fillColor;
    }

    public void UpdateFill(float current, float max)
    {
        if (fill != null)
        {
            fill.rectTransform.localScale = new Vector3(current / max, 1, 1);
        }
    }
}
