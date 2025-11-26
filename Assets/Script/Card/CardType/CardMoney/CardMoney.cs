using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/CardMoney")]
public class CardMoney : ScriptableObject
{
    [SerializeField] private int GoldCoin;
    [SerializeField] private int Interest;

    public void Reset_()
    {
        GoldCoin = 25;
        Interest = 25;
    }
    public void Copy(out int goldcoin, out int interest)
    {
        goldcoin = GoldCoin;
        interest = Interest;
    }
    public void Init(ref int goldcoin, ref int interest)
    {
        GoldCoin = goldcoin;
        Interest = interest;
    }
    public void AddGoldCoin(int changeamount) { GoldCoin += changeamount; }
    public int GetGoldCoin() { return GoldCoin; }
    public int GetGoldCoinInterest() { return GoldCoin > Interest ? Interest / 5 : (GoldCoin > 0 ?  GoldCoin / 5: 0); }
    public void SetGoldCoinInterest(int interest) { Interest = interest; }
}
