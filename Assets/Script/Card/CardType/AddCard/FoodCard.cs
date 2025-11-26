using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/FoodCard")]
[System.Serializable]
public class FoodCard : ScriptableObject
{
    [Header("基本信息")]
    [SerializeField] private bool isLevelUp;
    [SerializeField] private float price;

    [Header("升级前的得分相关")]
    [SerializeField] private string objectName;
    [SerializeField] private float refreshTime;
    [SerializeField] private Sprite cardSprite;
    [SerializeField] private float score_Add;
    [SerializeField] private float power_Add;
    [SerializeField] private float power_Mul;

    [Header("升级后的得分相关")]
    [SerializeField] private string objectNameLevelUp;
    [SerializeField] private float refreshTimeLevelUp;
    [SerializeField] private Sprite cardSpriteLevelUp;
    [SerializeField] private float score_AddLevelUp;
    [SerializeField] private float power_AddLevelUp;
    [SerializeField] private float power_MulLevelUp;

    public enum FoodType//默认肉类>蔬菜>水果>面包
    {
        Bread,//面包
        Fruit,//水果
        Meats,//肉类
        Veget,//蔬菜
    }
    public FoodType cardType;
    public float GetPrice() {return price;}
    public bool GetLevelUp(){ return isLevelUp; }
    public Sprite GetCardSprite(){  return isLevelUp? cardSpriteLevelUp : cardSprite; }
    public float GetRefreshTime(){ return isLevelUp? refreshTimeLevelUp : refreshTime; }  
    public float GetScoreAdd() { return isLevelUp? score_AddLevelUp : score_Add; }
    public float GetPowerAdd() { return isLevelUp? power_AddLevelUp : power_Add; }
    public float GetPowerMul() { return isLevelUp? power_MulLevelUp : power_Mul; }
    public string GetObjectName() {return isLevelUp ? objectNameLevelUp :objectName; }

    public int GetSortNum()//默认肉类>蔬菜>水果>面包,升级>默认
    {
        return cardType switch
        {
            FoodType.Meats => isLevelUp ? 8 : 7,
            FoodType.Veget => isLevelUp ? 6 : 5,
            FoodType.Fruit => isLevelUp ? 4 : 3,
            FoodType.Bread => isLevelUp ? 2 : 1,
            _ => 0,
        };
    }
}