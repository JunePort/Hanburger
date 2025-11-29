using UnityEngine;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("动效参数")]
    public float hoverScale = 1.2f;
    public float hoverDuration = 0.2f;
    public Vector3 hoverOffset = new Vector3(0, 20, 0);

    private RectTransform rect;
    private Vector3 originalLocalPos;
    private Vector3 originalLocalScale;
    private bool isHovering = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalLocalPos = rect.localPosition;
        originalLocalScale = rect.localScale;
    }

    public void Initialize(int cardId)
    {
        // 示例：根据 cardId 设置卡面图片、名称等
        // GetComponent<Image>().sprite = CardDatabase.GetSprite(cardId);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovering) return; // 防止快速进出导致协程堆积
        isHovering = true;
        StopAllCoroutines();
        StartCoroutine(HoverIn());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        StopAllCoroutines();
        StartCoroutine(HoverOut());
    }

    IEnumerator HoverIn()
    {
        float elapsed = 0f;
        Vector3 startScale = rect.localScale;
        Vector3 startPos = rect.localPosition;
        Vector3 targetScale = originalLocalScale * hoverScale;
        Vector3 targetPos = originalLocalPos + hoverOffset;

        while (elapsed < hoverDuration)
        {
            rect.localScale = Vector3.Lerp(startScale, targetScale, elapsed / hoverDuration);
            rect.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / hoverDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rect.localScale = targetScale;
        rect.localPosition = targetPos;
    }

    IEnumerator HoverOut()
    {
        float elapsed = 0f;
        Vector3 startScale = rect.localScale;
        Vector3 startPos = rect.localPosition;

        while (elapsed < hoverDuration)
        {
            rect.localScale = Vector3.Lerp(startScale, originalLocalScale, elapsed / hoverDuration);
            rect.localPosition = Vector3.Lerp(startPos, originalLocalPos, elapsed / hoverDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rect.localScale = originalLocalScale;
        rect.localPosition = originalLocalPos;
    }
}