using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardType : ScriptableObject
{
    public enum CardType
    {
        isFoodCard,
        isCookCard,
        isBookCard,
        isGoodCard,
    }
    public CardType cardType;

    public CookCard cookCard;
    public BookCard bookCard;
    public GoodCard goodCard;
    public FoodCard foodCard;
}
