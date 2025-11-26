using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/CookCard")]
[System.Serializable]
public class CookCard : ScriptableObject
{
    public string objectName;
    [SerializeField] private bool isLevelUp;

    [SerializeField] private Sprite cardSprite;
    [SerializeField] private string description;
    [Header("得分")]
    [SerializeField] private float score_Add;
    [SerializeField] private float power_Add;
    [SerializeField] private float power_Mul;
    [Header("结尾")]
    [SerializeField] private float score_Add_End;
    [SerializeField] private float power_Add_End;
    [SerializeField] private float power_Mul_End;
    public enum CardType
    {
        MeatsCook,//肉类：得分+5-->倍率+2
        BreadCook,//面包：得分+5-->倍率+2
        FruitCook,//果酱：得分+5-->倍率+2
        VegetCook,//蔬菜：得分+5-->倍率+2

        Two__Cook,//两对：倍率+4-->倍率*2
        Full_Cook,//满彩：倍率+4-->倍率*2
        ThreeCook,//三张：倍率+4-->倍率*2
        Four_Cook,//四张：倍率+4-->倍率*2

        Half_Cook,//(精致点心)小于3张,倍率+10
        StaffCook,//(抱团取暖)每拥有一个员工，倍率+3
        Book_Cook,//(精益求精)使用食谱的次数加到倍率中
        GroupCook,//(供不应求)传送带上的食物数量加到倍率中
        Shop_Cook,//(免费购物)商店免费购物一次
        Bank_Cook,//(银行贷款)可以负债-20$
        SmallCook,//(匠心独具)打出不是最常见的食谱，倍率累加0.2,若是最常见的食谱，则倍率减到0
        MouldCook,//(拼命内卷)每一个空的厨师位给予，倍率*1
    }
    public CardType cardType;
    [Header("售价")]
    private bool is_sell;
    [SerializeField] private int price;
    [SerializeField] private int price_sell;

    #region 外部接口
    public bool GetLevelUp() { return isLevelUp; }
    public Sprite GetCardSprite() { return cardSprite; }
    public string GetDescription() { return description+ DesAdd(); }
    public void SetIsSell(bool is_sell) { this.is_sell = is_sell;this.power_Mul_End = 1; }
    public bool GetIsSell() { return is_sell; }
    public int GetPrice(bool is_sell = false) { return is_sell? price_sell : price; }
    public float GetScoreAdd() { return score_Add; }
    public float GetPowerAdd() {  return power_Add; }
    public float GetPowerMul(){ return power_Mul; }
    public float GetScoreAddEnd(){ return score_Add_End; }
    public float GetPowerAddEnd(){ return power_Add_End; }
    public float GetPowerMulEnd(){  return power_Mul_End;}
    #endregion

    #region CookCard逻辑
    public string DesAdd()
    {
        return cardType switch
        {
            CardType.SmallCook => "当前倍率*" + power_Mul_End,
            CardType.MouldCook => "当前倍率*" + power_Mul_End,
            _ => "",
        };
    }
    public bool AddCookCard(FoodCard foodCard)//对单张食材触发效果
    {
        return cardType switch
        {
            CardType.MeatsCook => foodCard.cardType == FoodCard.FoodType.Meats,
            CardType.BreadCook => foodCard.cardType == FoodCard.FoodType.Bread,
            CardType.FruitCook => foodCard.cardType == FoodCard.FoodType.Fruit,
            CardType.VegetCook => foodCard.cardType == FoodCard.FoodType.Veget,
            _ => false,
        };
    }
    public bool AddCookCardScore(BookCard bookcard)//对最后整体触发的效果
    {
        switch (cardType)
        {
            case CardType.Two__Cook: return bookcard.cardType == BookCard.CardType.Pair || bookcard.cardType == BookCard.CardType.TwoPairs;
            case CardType.Full_Cook: return bookcard.cardType == BookCard.CardType.FullHouse;
            case CardType.ThreeCook: return bookcard.cardType == BookCard.CardType.ThreeOfAKind || bookcard.cardType == BookCard.CardType.ThreeWithTwo;
            case CardType.Four_Cook: return bookcard.cardType == BookCard.CardType.Flush;

            case CardType.Half_Cook: return bookcard.cardType == BookCard.CardType.Pair || bookcard.cardType == BookCard.CardType.TwoPairs;
            case CardType.StaffCook: power_Add_End = CardList.Instance.cookGroupcards.Count * 3; return CardList.Instance.cookGroupcards.Count > 0;
            case CardType.Book_Cook: power_Add_End = bookcard.GetUseNum(); return bookcard.GetUseNum() > 0;
            case CardType.GroupCook: power_Add_End = CardList.Instance.foodGroupcards.Count; return CardList.Instance.foodGroupcards.Count > 0;

            case CardType.SmallCook: power_Mul_End += BookCardDeck.Instance.IsMostCardType( bookcard.cardType) ? 0.2f : -power_Mul_End+1; return true;
            case CardType.MouldCook: power_Mul_End = CookCardGroup.Instance.MAXCardCount - CardList.Instance.cookGroupcards.Count; return CardList.Instance.cookGroupcards.Count > 0;
            default: return false;
        }
    }
    public async void UseCookCard(bool is_use = true)//直接使用
    {
        switch (cardType)
        {
            case CardType.Shop_Cook: await GoodCardShop.COOK_CARD_SHOP(is_use); break;
            case CardType.Bank_Cook: GoodCardMoney.COOK_CARD_BANK(is_use); break;
            default: break;
        }
    }

    #endregion

}