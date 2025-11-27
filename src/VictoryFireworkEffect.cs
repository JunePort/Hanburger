using UnityEngine;

public class VictoryFireworkEffect : MonoBehaviour
{
    public ParticleSystem fireworkPS;
    public int maxParticles = 500;
    public Camera mainCamera;
    public float shakeDuration = 0.3f;
    public float shakeIntensity = 0.1f;

    private Coroutine shakeRoutine;
    private bool isPlaying = false;

    void Start()
    {
        var main = fireworkPS.main;
        main.maxParticles = maxParticles;

        var emission = fireworkPS.emission;
        emission.enabled = false;

        var trails = fireworkPS.trails;
        trails.enabled = false;
    }

    public void PlayEffect()
    {
        if (isPlaying) return;
        isPlaying = true;

        fireworkPS.Play();

        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(CameraShakeImmediate());

        UIManager.Instance?.TriggerVictoryFeedback();
    }

    System.Collections.IEnumerator CameraShakeImmediate()
    {
        float elapsed = 0f;
        Vector3 originalPos = mainCamera.transform.localPosition;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / shakeDuration;
            float magnitude = Mathf.Lerp(shakeIntensity, 0f, progress);

            mainCamera.transform.localPosition = originalPos + Random.insideUnitSphere * magnitude;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPos;
        isPlaying = false;
    }
}