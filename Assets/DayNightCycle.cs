using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Material sunriseSkybox;
    public Material daySkybox;
    public Material sunsetSkybox;
    public Material nightSkybox;

    public float cycleDuration = 60f; // Full cycle duration in seconds

    private float cycleTimer = 0f;
    private int currentPhase = 0;
    private Material currentSkybox;
    private Material nextSkybox;
    private Material[] skyboxes;

    void Start()
    {
        skyboxes = new Material[] { sunriseSkybox, daySkybox, sunsetSkybox, nightSkybox };
        currentPhase = 0;
        currentSkybox = new Material(skyboxes[currentPhase]); // Clone material to allow modification
        nextSkybox = skyboxes[(currentPhase + 1) % 4];

        RenderSettings.skybox = currentSkybox;
        DynamicGI.UpdateEnvironment();
    }

    void Update()
    {
        cycleTimer += Time.deltaTime;
        float phaseDuration = cycleDuration / 4;
        float blendFactor = (cycleTimer % phaseDuration) / phaseDuration;

        // Blend between current and next skybox
        currentSkybox.Lerp(skyboxes[currentPhase], nextSkybox, blendFactor);
        DynamicGI.UpdateEnvironment(); // Updates global illumination

        // Change skybox phase when cycle completes
        if (cycleTimer >= phaseDuration)
        {
            cycleTimer = 0f;
            currentPhase = (currentPhase + 1) % 4;
            nextSkybox = skyboxes[(currentPhase + 1) % 4];
        }
    }
}
