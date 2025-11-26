using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(menuName = "Card/BookCard")]
[System.Serializable]
public class BookCard : ScriptableObject
{
    public string objectName;
    [SerializeField] private string description;
    [SerializeField] private Sprite cardSprite;
    [Header("基础数值不可以改变")]
    [SerializeField] private int score_Base;
    [SerializeField] private int power_Base;
    [Header("升级次数/使用次数")]
    [SerializeField] private int Level_Num;
    [SerializeField] private int Use_Num;
    [Header("得分相关")]
    [SerializeField] private int score_Add;
    [SerializeField] private int power_Add;
    [Header("升级得分")]
    [SerializeField] private int up_score;
    [SerializeField] private int up_power;

    public enum CardType
    {
        WelCome,        // 什么有没有
        HighCard,       // 高牌（基础汉堡）0.39
        Pair,           // 对子（双倍风味堡）5.47
        TwoPairs,       // 两对（田园肉食堡）35.16
        FullHouse,      // 满堂彩（经典汉堡）23.44
        ThreeOfAKind,   // 三条（特色风味堡）23.44
        ThreeWithTwo,    // 三带二（明星汉堡）11.72
        Flush,          // 同花（巨无霸）11.0
    }
    public CardType cardType;

    [Header("售价")]
    [SerializeField] private int price;

    #region public

    public string GetDescription() { return description; }
    public Sprite GetCardSprite() { return cardSprite; }
    public void Level_1() { Level_Num = 1; score_Add = 0; power_Add = 0; }
    public void LevelUp() { Level_Num++; score_Add += up_score; power_Add += up_power; }
    public void LevelDown() { if (Level_Num > 1) { Level_Num--; score_Add -= up_score; power_Add -= up_power; } }
    public void UseCountReset(bool isStart) { if(isStart)Use_Num = 0; }
    public void UseCountAdd() { Use_Num++; }
    public int GetLevelNum() { return Level_Num; }
    public int GetUseNum() { return Use_Num; }
    public void InitUseNum(int num) { Use_Num = num; }
    public int GetScore() { return score_Base + score_Add; }
    public int GetPower() { return power_Base + power_Add; }
    public int GetScore_LevelUp() { return up_score; }
    public int GetPower_LevelUp() { return up_power; }
    public int GetPrice() { return price; }
    public string GetCardType(CardType cardType,bool isShowLevel = true)
    {
        string cardTypeStr = cardType switch
        {
            CardType.WelCome => "欢迎光临",
            CardType.HighCard => "基础汉堡",
            CardType.Pair => "双倍风味堡",
            CardType.TwoPairs => "田园肉食堡",
            CardType.FullHouse => " 满堂彩",
            CardType.ThreeOfAKind => "特色风味堡",
            CardType.ThreeWithTwo => "明星汉堡",
            CardType.Flush => "巨无霸",
            _ => "未知类型",
        };
        return cardTypeStr+(isShowLevel? (" Lv."+GetLevelNum()):"");
    }

    
    #endregion
}