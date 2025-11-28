// CardHoverEffect.cs
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CardHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Animation Settings")]
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public Vector3 selectedScale = new Vector3(1.15f, 1.15f, 1.15f);
    public float duration = 0.2f;

    private Vector3 originalScale;
    private Coroutine currentTween;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentTween != null) StopCoroutine(currentTween);
        currentTween = StartCoroutine(TweenTo(hoverScale));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentTween != null) StopCoroutine(currentTween);
        currentTween = StartCoroutine(TweenTo(originalScale));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentTween != null) StopCoroutine(currentTween);
        currentTween = StartCoroutine(TweenTo(selectedScale, () =>
        {
            StartCoroutine(TweenTo(originalScale)); // 点击后回弹
        }));
    }

    IEnumerator TweenTo(Vector3 targetScale, System.Action onComplete = null)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
        onComplete?.Invoke();
    }
}