using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardPackController : MonoBehaviour
{
    [Header("卡牌相关")]
    public GameObject cardPrefab;                     // 卡牌预制体
    public Transform cardParent;                      // 卡牌父对象（用于管理层级）
    public int cardCount = 5;                         // 每次开包出卡数量

    [Header("动画参数")]
    public float openDuration = 1.5f;                 // 整体开启动画时长
    public float spreadInterval = 0.1f;               // 卡牌展开间隔
    public Vector3 spreadOffset = new Vector3(80, 0, 0); // 横向偏移

    [Header("特效与算法")]
    public CardDrawEffect cardDrawEffect;             // 抽卡特效组件引用
    private System.Random rng = new System.Random();

    private bool isOpening = false;

    public void OpenPack()
    {
        if (isOpening) return;
        isOpening = true;
        StartCoroutine(OpenPackCoroutine());
    }

    IEnumerator OpenPackCoroutine()
    {
        // 1. 随机抽卡（可对接你的卡池逻辑）
        var drawnCards = new int[cardCount];
        for (int i = 0; i < cardCount; i++)
        {
            drawnCards[i] = rng.Next(1, 101); // 示例：抽卡ID（实际应调用卡池系统）
        }

        // 2. 生成卡牌 GameObject（初始堆叠）
        var cardObjects = new GameObject[cardCount];
        for (int i = 0; i < cardCount; i++)
        {
            var cardObj = Instantiate(cardPrefab, cardParent);
            cardObj.SetActive(false); // 先隐藏，避免闪现
            cardObjects[i] = cardObj;

            // 绑定抽卡数据（例如设置Sprite、ID等）
            var cardComp = cardObj.GetComponent<CardUI>();
            if (cardComp != null)
                cardComp.Initialize(drawnCards[i]);
        }

        // 3. 播放开启动画：逐个展开
        for (int i = 0; i < cardCount; i++)
        {
            cardObjects[i].SetActive(true);
            StartCoroutine(ExpandCard(cardObjects[i].transform, i));
            yield return new WaitForSeconds(spreadInterval);
        }

        // 4. 触发抽卡特效（初步对接）
        if (cardDrawEffect != null)
            cardDrawEffect.TriggerEffect(drawnCards);

        isOpening = false;
    }

    IEnumerator ExpandCard(Transform card, int index)
    {
        var startPos = transform.position; // 卡包中心位置
        var targetPos = startPos + new Vector3(index * spreadOffset.x, index * spreadOffset.y, index * spreadOffset.z);

        float elapsed = 0f;
        while (elapsed < openDuration / cardCount)
        {
            card.position = Vector3.Lerp(startPos, targetPos, elapsed / (openDuration / cardCount));
            elapsed += Time.deltaTime;
            yield return null;
        }
        card.position = targetPos;
    }
}