using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/GoodCard")]
[System.Serializable]
public class GoodCard : ScriptableObject
{
    public string objectName;
    [SerializeField] private bool isLevelUp;

    [Header("升级前的属性")]
    [SerializeField] private string cardName;
    [SerializeField] private string description;
    [SerializeField] private Sprite cardSprite;

    [Header("升级后的属性")]
    [SerializeField] private string cardNameLevelUp;
    [SerializeField] private string descriptionLevelUp;
    [SerializeField] private Sprite cardSpriteLevelUp;
    public enum CardType
    {
        Buying,
        Client,
        Market,
        Staffs,
        InCome,
        Public,
        Groups,
        Update,
    }
    public CardType cardType;

    [Header("售价")]
    [SerializeField] private int price;
    #region Get
    public int GetPrice() { return price; }
    public Sprite GetCardSprite() { return isLevelUp ? cardSpriteLevelUp : cardSprite; }
    public string GetCardName() { return isLevelUp ? cardNameLevelUp : cardName; }
    public string GetDescription() { return isLevelUp ? descriptionLevelUp : description; }
    public bool GetLevelUp() { return isLevelUp; }
    #endregion

    #region GoodCard
    public async UniTask UseGoodCard()
    {
        switch (cardType)
        {
            case CardType.Buying:       GoodCardShop .GOOD_CARD_BUYING(isLevelUp ? 2 : 1); break;
            case CardType.Client:       FoodCardGroup.GOOD_CARD_CLIENT(isLevelUp ? 2 : 1); break;
            case CardType.Market:       BookCardDeck .GOOD_CARD_MARKET(isLevelUp ? 2 : 1); break;
            case CardType.Staffs:       CookCardGroup.GOOD_CARD_STAFFS(isLevelUp ? 2 : 1); break;
            case CardType.InCome:       GoodCardMoney.GOOD_CARD_INCOME(isLevelUp ? 2 : 1); break;
            case CardType.Public:       BookCardScore.GOOD_CARD_PUBLIC(isLevelUp ? 2 : 1); break;
            case CardType.Groups:       FoodCardGroup.GOOD_CARD_GROUPS(isLevelUp ? 2 : 1); break;
            case CardType.Update: await GoodCardShop .GOOD_CARD_UPDATE(isLevelUp ? 2 : 1); break;
        }
    }
    #endregion
}