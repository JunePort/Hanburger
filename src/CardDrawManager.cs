// CardDrawManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardDrawManager : MonoBehaviour
{
    public GameObject cardPrefab;           // 卡牌预制体（含 CardHoverEffect 组件）
    public Transform drawTargetPosition;    // 卡牌最终停留的位置（例如手牌区）
    public float drawDuration = 0.6f;
    public float bounceIntensity = 0.1f;

    public void DrawCard()
    {
        if (cardPrefab == null || drawTargetPosition == null) return;

        GameObject newCard = Instantiate(cardPrefab);
        newCard.transform.SetParent(transform.root); // 临时放在根层级，避免缩放继承问题
        newCard.transform.SetAsLastSibling();        // 确保在最上层

        // 初始位置：屏幕下方外
        Vector3 startPos = new Vector3(drawTargetPosition.position.x, -1000f, drawTargetPosition.position.z);
        newCard.transform.position = startPos;
        newCard.SetActive(true);

        // 执行弹入动画
        StartCoroutine(AnimateCardDraw(newCard, startPos, drawTargetPosition.position));
    }

    IEnumerator AnimateCardDraw(GameObject card, Vector3 start, Vector3 end)
    {
        float elapsed = 0f;
        Vector3 originalScale = card.transform.localScale;

        while (elapsed < drawDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / drawDuration;

            // 缓动函数：EaseOutBack（带回弹效果）
            float back = bounceIntensity;
            float overshoot = 1f + back;
            float easedT = 1f - Mathf.Pow(1f - t, 2) * (1f + back * (1f - t));

            card.transform.position = Vector3.Lerp(start, end, easedT);

            // 轻微缩放弹跳
            float scaleT = 1f + back * Mathf.Sin(Mathf.PI * t * 2f) * (1f - t);
            card.transform.localScale = originalScale * scaleT;

            yield return null;
        }

        card.transform.position = end;
        card.transform.localScale = originalScale;

        // 可选：播放粒子特效（如果存在）
        ParticleSystem ps = card.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }
    }
}