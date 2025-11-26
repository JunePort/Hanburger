using BehaviorDesigner.Runtime.Tasks.Unity.UnityLight;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/CardBoss")]
public class CardBoss : ScriptableObject
{
    public int moneyAward;
    public float scoreGoal;
    public int PublishCardNum;
    public int AbandonCardNum;

    public bool isfeated;

    private int P_Num;
    private int A_Num;
    public enum CardType
    {
        None,
        //*********非Boss*********
        Shop_Cooks,//商店面试官：打开商店可获得<免费员工>
        Shop_Goods,//商店装修工：打开商店可获得<免费装修>
        Shop_Books,//商店食谱师：打开商店可获得<免费食谱>

        //*********Boss*********
        Capitalistic,//资本家：打出最常见的卡牌时资金清零
        CirticPeople,//批判家：食谱的得分和倍率减半    
        Finickyeater,//挑食者：只能打出一种美食             
        Groupusculer,//小团体：打出每种食材资金-1
        Gastronomeer,//美食家：不能重复打出重复美食      
        Troublemaker,//捣蛋鬼：打出任意美食时食谱等级-1

        Big_star,    //大名星：所需要的得分翻倍
        Drunkman,    //醉汉者：解雇一名员工维持秩序才能得分
        Homeless,    //流浪者：手牌-1食材被流浪者拿走了
        Stranger,    //陌生人：只能制作一次美食,但是得分减半   
        Supplier,    //供应商：供应商面前不能弃牌                                   
        Thiefman,    //偷窃者：升级效果失败 
    }
    public CardType cardType;//在关卡初始化的时候给Boss一个属性(必须),交给外界

    
    public void SetBossStart()////
    {
        switch (cardType)
        {
            case CardType.Big_star: P_Num = PublishCardNum; A_Num = AbandonCardNum; /*GetScoreGoal()*/      break; //大名星：所需要的得分翻倍
            case CardType.Drunkman: P_Num = PublishCardNum; A_Num = AbandonCardNum; IsDrunkman(false,true); break; //醉汉者：解雇一名员工维持秩序才能得分  
            case CardType.Homeless: P_Num = PublishCardNum; A_Num = AbandonCardNum; /*IsHomeless()*/        break; //流浪者：手牌-1因为食材送给流浪者了
            case CardType.Supplier: P_Num = PublishCardNum; A_Num = AbandonCardNum; /*IsSupplier()*/        break; //供应商：供应商面前不能弃牌
            case CardType.Stranger: P_Num = 1;              A_Num = AbandonCardNum; /*GetScoreGoal()*/      break; //陌生人：只能出一次牌,但是得分减半 
            case CardType.Thiefman: P_Num = PublishCardNum; A_Num = AbandonCardNum; /*IsThiefman()*/        break; //偷窃者：升级效果失败  
            default:                P_Num = PublishCardNum; A_Num = AbandonCardNum;                         break;
        }
    } 
    public bool GetIsFeated() { return isfeated; }
    public float GetScoreGoal(){ return cardType switch{ CardType.Big_star => scoreGoal * 2,  CardType.Stranger => scoreGoal / 2,  _ => scoreGoal, }; }
    public int GetMoneyAward() { return moneyAward; }   
    public void UsePublishCard() { P_Num--; }
    public void UseAbandonCard() { A_Num--; }
    public bool CanUsePublishCard() { return P_Num > 0; }
    public bool CanUseAbandonCard() { return A_Num > 0; }
    public int GetPublishCardNum() { return P_Num; }
    public int GetAbandonCardNum() { return A_Num; }

    public void Reset_()
    {
        isDrunk = false;
        bookcardTypeList.Clear();
    }

    private bool isDrunk;
    public List<BookCard.CardType> bookcardTypeList = new();
    public bool IsDrunkman(bool ismaintain = false, bool isDrunkPre = false) { return cardType == CardType.Drunkman && (isDrunkPre ? isDrunk = true : (ismaintain ? isDrunk = false : isDrunk)); }
    public int IsHomeless() { return cardType == CardType.Homeless ? 1 : 0; }
    public bool IsThiefman() { return cardType == CardType.Thiefman; }
    public bool IsSupplier() { return cardType == CardType.Supplier; }
    public float IsCirticPeople() { return cardType == CardType.CirticPeople ? 0.5f : 1; }
    public bool IsShopBoss() { return cardType == CardType.Shop_Books || cardType == CardType.Shop_Goods || cardType == CardType.Shop_Cooks; }
    public bool IsFreeBookCard() { return cardType == CardType.Shop_Books; }
    public bool IsFreeCookCard() { return cardType == CardType.Shop_Cooks; }
    public bool IsFreeGoodCard() { return cardType == CardType.Shop_Goods; }
    
    public async UniTask MeetWithBoss(List<Card> cardsToRemove)
    {
        switch (cardType)
        {
            case CardType.Capitalistic: if (BookCardDeck.Instance.IsMostCardType(BookCardDeck.Instance.GetCardType(cardsToRemove))) await GoodCardMoney.Instance.Money_Use(-CardList.Instance.cardMoney.GetGoldCoin()); break;//资本家：打出最常见的卡牌时资金清零
            case CardType.Groupusculer: await GoodCardMoney.Instance.Money_Use(-cardsToRemove.Count); break;//小团体：每次打出一张牌资金-1
            case CardType.Troublemaker: await BookCardGroup.Instance.DelBook_Card(BookCardDeck.Instance.GetCardType(cardsToRemove)); break;//捣蛋鬼：打出任意牌型时等级-1 

            default: break;
        }
    }
    public bool BossCanEatFood(BookCard.CardType bookcardType)
    {
        switch (cardType)
        {
            case CardType.Drunkman: return IsDrunkman(); //醉汉者：解雇一名员工维持秩序才能得分  //
            case CardType.Finickyeater: { if (bookcardTypeList.Count == 0) return false; } return !bookcardTypeList.Contains(bookcardType);
            case CardType.Gastronomeer: { if (bookcardTypeList.Count == 0) return false; } return  bookcardTypeList.Contains(bookcardType);
            default: return false;
        }
    }
    public bool BossEatFood(List<Card> cardsToRemove)
    {
        switch (cardType)
        {
            case CardType.Drunkman: return !IsDrunkman(); //醉汉者：解雇一名员工维持秩序才能得分  
            case CardType.Finickyeater:
                if (bookcardTypeList.Count == 0) { bookcardTypeList.Add(BookCardDeck.Instance.GetCardType(cardsToRemove)); return true; }
                if (!bookcardTypeList.Contains(BookCardDeck.Instance.GetCardType(cardsToRemove))) { return false; }
                return true;//挑食者：只能打出一种牌型
            case CardType.Gastronomeer:
                if (bookcardTypeList.Count == 0) { bookcardTypeList.Add(BookCardDeck.Instance.GetCardType(cardsToRemove)); return true; }
                if ( bookcardTypeList.Contains(BookCardDeck.Instance.GetCardType(cardsToRemove))) { return false; }
                else { bookcardTypeList.Add(BookCardDeck.Instance.GetCardType(cardsToRemove)); }
                return true;//美食家：不能重复打出同一个牌型
            default: return true;
        }
    }
    /// <summary>
    /// 获取卡牌类型的中文名称
    /// 用法：boss.GetCardType()
    /// </summary>
    public string GetCardType()
    {
        return cardType switch
        {
            CardType.Shop_Books => "商店食谱师",
            CardType.Shop_Cooks => "商店面试官",
            CardType.Shop_Goods => "商店装修工",
            CardType.Capitalistic => "资本家",
            CardType.CirticPeople => "批判家",
            CardType.Finickyeater => "挑食者",
            CardType.Groupusculer => "小团体",
            CardType.Gastronomeer => "美食家",
            CardType.Troublemaker => "捣蛋鬼",
            CardType.Big_star => "大名星",
            CardType.Drunkman => "醉汉者",
            CardType.Homeless => "流浪者",
            CardType.Supplier => "供应商",
            CardType.Stranger => "陌生人",
            CardType.Thiefman => "偷窃者",
            _ => "未知类型",
        };
    }

    /// <summary>
    /// 获取卡牌类型的中文描述
    /// 用法：boss.GetCardTypeDesc()
    /// </summary>
    public string GetCardTypeDesc()
    {
        return cardType switch
        {
            CardType.Shop_Books => "打开商店可获得<免费食谱>",
            CardType.Shop_Cooks => "打开商店可获得<免费员工>",
            CardType.Shop_Goods => "打开商店可获得<免费装修>",
            CardType.Capitalistic => "打出最常见的卡牌时资金清零",
            CardType.CirticPeople => "食谱的得分和倍率减半",
            CardType.Finickyeater => "只能打出一种美食",
            CardType.Groupusculer => "打出每种食材资金-1",
            CardType.Gastronomeer => "不能重复打出重复美食",
            CardType.Troublemaker => "打出任意美食时食谱等级-1",
            CardType.Big_star => "所需要的得分翻倍",
            CardType.Drunkman => "解雇一名员工维持秩序才能得分",
            CardType.Homeless => "手牌-1 食材被流浪者拿走了",
            CardType.Supplier => "供应商面前不能弃牌",
            CardType.Stranger => "只能制作一次美食 但得分减半",
            CardType.Thiefman => "升级效果失败",
            _ => "未知类型",
        };
    }

}
