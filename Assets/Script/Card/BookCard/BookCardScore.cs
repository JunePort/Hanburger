using System;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using System.Collections.Generic;

public class BookCardScore : MonoBehaviour
{
    public float socre_percent = 1;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI Boss_Goal_Text;
    [SerializeField] private TextMeshProUGUI BossAward_Text;
    [SerializeField] private TextMeshProUGUI SPAll_____Text;
    [SerializeField] private TextMeshProUGUI SPAll_Add_Text;
    [SerializeField] private TextMeshProUGUI Card_Type_Text;
    [SerializeField] private TextMeshProUGUI score_____Text;
    [SerializeField] private TextMeshProUGUI score_Add_Text;
    [SerializeField] private TextMeshProUGUI power_____Text;
    [SerializeField] private TextMeshProUGUI power_Add_Text;
    [SerializeField] private TextMeshProUGUI power_Mul_Text;
    private static BookCardScore _instance;
    public static BookCardScore Instance { get { if (_instance == null) _instance = FindObjectOfType<BookCardScore>(); return _instance; } }

    void Start()
    {
        Empty_Score();
        SetScoreGoal();
    }

    public void Empty_Score()
    {
        score_____Text.text = "0";
        score_Add_Text.text = "";
        power_____Text.text = "0";
        power_Add_Text.text = "";
        power_Mul_Text.text = "";

        SPAll_____Text.text = "0";
        SPAll_Add_Text.text = "";
        Card_Type_Text.text = "";
        BossAward_Text.text = new string('$', Mathf.Max(0, CardList.Instance.cardBoss.GetMoneyAward()));
    }

    public static void GOOD_CARD_PUBLIC(int LevelUp = 0)
    {
        BookCardScore instance = Instance; // 获取唯一实例
        Debug.Log("BookCardScore GOOD_CARD_PUBLIC");
        switch (LevelUp)
        {
            case 0: instance.socre_percent = 1; break;
            case 1: instance.socre_percent = 0.9f; break;
            case 2: instance.socre_percent = 0.8f; break;
        };
        instance.SetScoreGoal();
    }

    

    public void SetScoreGoal()
    {
        Boss_Goal_Text.text = "目标" + (socre_percent == 1 ? " " : (socre_percent == 0.9f ? "(90%)" : "(80%)")) + ":" + ((int)(CardList.Instance.cardBoss.GetScoreGoal() * socre_percent)).ToString();
    }

    public async UniTask CarryScoreGoal(float currentscore)
    {
        if(currentscore >= (int)(CardList.Instance.cardBoss.GetScoreGoal() * socre_percent))
        {
            Boss_Goal_Text.text = "营业顺利";
            await AnimateDollarRemoval(CardList.Instance.cardBoss.GetMoneyAward(), delayPerCoin: 0.1f);
            if (!GoodCardShop.Instance.isOpenedShop)
            {
                await GoodCardMoney.Instance.Money_Add(CardList.Instance.cardBoss.GetMoneyAward());
                BossAward_Text.text = "已领取";
            }
            await GoodCardMoney.Instance.Money_Interest();
            //打开商店
            FoodCardGroup.Instance.isStopGame = true;
            GoodCardShop.Instance.isSuccessfulDay = true;
            CardList.Instance.CleanFoodSaveCard();
            GoodCardShop.Instance.ShowCardShop();
        }
        else if (!CardList.Instance.cardBoss.CanUsePublishCard())
        {
            Boss_Goal_Text.text = "营业失败";
            await AnimateDollarRemoval(CardList.Instance.cardBoss.GetMoneyAward(), delayPerCoin: 0.1f);
            BossAward_Text.text = "下次再来";
            //打开商店
            FoodCardGroup.Instance.isStopGame = true;
            GoodCardShop.Instance.isSuccessfulDay = false;
            CardList.Instance.CleanFoodSaveCard();
            GoodCardShop.Instance.ShowCardShop();
        }
        else
        {
            SetScoreGoal();
        }
    }

    private async UniTask AnimateDollarRemoval(int total, float delayPerCoin, CancellationToken ct = default)
    {
        if (BossAward_Text == null || total <= 0) return;
        BossAward_Text.text = new string('$', total);
        for (int i = total; i > 0; i--)
        {
            if (i < total)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delayPerCoin), cancellationToken: ct);
            }
            BossAward_Text.text = new string('$', i - 1);
        }
    }

    // ==================== 公共方法 ====================

    public void Pre_Score_And_Power(BookCard.CardType cardType, CardBoss bossCard = null)
    {
        if (cardType == BookCard.CardType.WelCome)
        {
            score_____Text.text = "0";
            power_____Text.text = "0";
            Card_Type_Text.text = "";
            return;
        }
        if (bossCard != null)
        {
            if (bossCard.BossCanEatFood(cardType))
            {
                score_____Text.text = "0";
                power_____Text.text = "0";
                Card_Type_Text.text = bossCard.GetCardTypeDesc();
                return;
            }
        }
        score_____Text.text = (BookCardDeck.Instance.GetScore_Add(cardType) * CardList.Instance.cardBoss.IsCirticPeople()).ToString();
        power_____Text.text = (BookCardDeck.Instance.GetPower_Add(cardType) * CardList.Instance.cardBoss.IsCirticPeople()).ToString();
        Card_Type_Text.text = BookCardDeck.Instance.GetCard_Type(cardType);
    }


    public async UniTask BookCardType_LevelUp_OR_Down(BookCard card, bool isLevelUp = true, float time = 1.0f, CancellationToken ct = default)
    {
        //请确保时在大于等级1的情况下调用，这个只是画面显示
        await ShowTextWithFade(score_Add_Text, (isLevelUp ? "+" : "-") + Mathf.Round(card.GetScore_LevelUp()).ToString(), time, ct);
        float current1 = float.TryParse(score_____Text.text, out var p) ? p : 0;
        score_____Text.text = (current1 + card.GetScore_LevelUp() * (isLevelUp ? 1 : -1)).ToString();

        await ShowTextWithFade(power_Add_Text, (isLevelUp ? "+" : "-") + Mathf.Round(card.GetPower_LevelUp()).ToString(), time, ct);
        float current2 = float.TryParse(power_____Text.text, out var q) ? q : 0;
        power_____Text.text = (current2 + card.GetPower_LevelUp() * (isLevelUp ? 1 : -1)).ToString();
    }





    public async UniTask Score_(float score_add, float time = 1.0f, CancellationToken ct = default)
    {
        if (score_add != 0)
        {
            await ShowTextWithFade(score_Add_Text, "+" + Mathf.Round(score_add).ToString(), time, ct);
        }
        else
        {
            await ShowTextWithFade(score_Add_Text, "", time, ct);
        }
        float current = float.TryParse(score_____Text.text, out var p) ? p : 0;
        score_____Text.text = (current + score_add).ToString();
    }

    public async UniTask Power_(float power_add, float power_mul, float time = 1.0f, CancellationToken ct = default)
    {
        bool hasAdd = power_add != 0;
        bool hasMul = power_mul != 0 && power_mul != 1;

        if (!hasAdd && !hasMul)
        {
            await ShowTextWithFade(power_Add_Text, "", time/2f, ct);
            await ShowTextWithFade(power_Mul_Text, "", time/2f, ct);
            return;
        }
        if (hasAdd)
        {
            string text = "+" + Mathf.Round(power_add).ToString();
            await ShowTextWithFade(power_Add_Text, text, hasMul ? time / 2f : time, ct);
            float current = float.TryParse(power_____Text.text, out var p) ? p : 0;
            power_____Text.text = (current + power_add).ToString();
        }

        if (hasMul)
        {
            string text = "x" + power_mul.ToString("0.##");
            await ShowTextWithFade(power_Mul_Text, text, hasAdd ? time / 2f : time, ct);
            float current = float.TryParse(power_____Text.text, out var p) ? p : 0;
            power_____Text.text = (current * power_mul).ToString();
        }
    }

    public async UniTask ScoreAll_Add(float totalTime = 1.0f, CancellationToken ct = default)
    {
        float scoreValue = float.TryParse(score_____Text.text, out var s) ? s : 0;
        float powerValue = float.TryParse(power_____Text.text, out var p) ? p : 0;
        float addValue = scoreValue * powerValue;

        string addText = "+" + Mathf.Round(addValue).ToString();
        await ShowTextWithFade(SPAll_Add_Text, addText, totalTime, ct);

        float currentTotal = float.TryParse(SPAll_____Text.text, out var total) ? total : 0f;
        float targetTotal = currentTotal + addValue;
        await AnimateTextValue(SPAll_____Text, currentTotal, targetTotal, totalTime * 0.5f, ct);

        score_____Text.text = "0";
        power_____Text.text = "0";
        Card_Type_Text.text = "";
        await CarryScoreGoal(targetTotal);
    }

    // ==================== 工具方法 ====================

    private async UniTask AnimateTextValue(
        TMP_Text textComponent,
        float startValue,
        float targetValue,
        float duration,
        CancellationToken ct)
    {
        if (textComponent == null) return;

        await UniTask.Create(async (token) =>
        {
            var tween = DOTween.To(
                    () => startValue,
                    x => textComponent.text = Mathf.Round(x).ToString(),
                    targetValue,
                    duration
                )
                .SetEase(Ease.OutQuad);

            while (tween.IsActive() && !tween.IsComplete())
            {
                token.ThrowIfCancellationRequested();
                await UniTask.Yield(token);
            }

            if (token.IsCancellationRequested)
            {
                tween.Kill();
                throw new OperationCanceledException(token);
            }

            textComponent.text = Mathf.Round(targetValue).ToString();
        }, ct);
    }

    private async UniTask ShowTextWithFade(TMP_Text tmpText, string message, float duration, CancellationToken ct)
    {
        if (tmpText == null) return;

        if (!tmpText.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup = tmpText.gameObject.AddComponent<CanvasGroup>();
        }

        tmpText.text = message;
        canvasGroup.alpha = 0f;

        float fadeTime = Mathf.Clamp(duration * 0.2f, 0.05f, 0.2f);
        float holdTime = Mathf.Max(0, duration - 2 * fadeTime);

        await DoFade(canvasGroup, 0f, 1f, fadeTime, ct);
        if (holdTime > 0)
            await UniTask.Delay(TimeSpan.FromSeconds(holdTime), cancellationToken: ct);
        await DoFade(canvasGroup, 1f, 0f, fadeTime, ct);

        tmpText.text = "";
        canvasGroup.alpha = 1f;
    }

    private async UniTask DoFade(CanvasGroup cg, float from, float to, float duration, CancellationToken ct)
    {
        await UniTask.Create(async (token) =>
        {
            var tween = cg.DOFade(to, duration).From(from).SetEase(Ease.Linear);
            while (tween.IsActive() && !tween.IsComplete())
            {
                token.ThrowIfCancellationRequested();
                await UniTask.Yield(token);
            }
            if (token.IsCancellationRequested)
            {
                tween.Kill();
                throw new OperationCanceledException(token);
            }
        }, ct);
    }
}