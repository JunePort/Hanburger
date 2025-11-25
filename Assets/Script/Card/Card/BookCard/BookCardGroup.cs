using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using System.Linq;
using System.Collections;
using UnityEngine.InputSystem;

public class BookCardGroup : MonoBehaviour
{
    [SerializeField] private Card selectedCard;//选择卡片
    [SerializeReference] private Card hoveredCard;//悬停卡片


    private const float tweenCardReturn = 2.0f;//是否启用卡片归位的动画时长
    private int cardNameCount = 0;
    private RectTransform rect;//刷新布局

    private static BookCardGroup _instance;
    public static BookCardGroup Instance { get { if (_instance == null) _instance = FindObjectOfType<BookCardGroup>(); return _instance; } }
    private  void Start()
    {
        rect = this.GetComponent<RectTransform>();
    }

   

    public async UniTask Refresh_Card(Card cardsolt, float time = 0.1f)//由其他函数调用和自己
    {
        if (cardsolt == null) return;
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        Card card = cardsolt.GetComponentInChildren<Card>();
        card.transform.parent.transform.SetParent(transform);
        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(BeginDrag);
        card.EndDragEvent.AddListener(EndDrag);
        card.OnSelectedChanged += OnCardSelectionChanged;
        card.name = cardNameCount.ToString();//数字只是区分而已
        cardNameCount++;
        await UniTask.Yield();
        card.Deselect();
        card.cardVisual.transform.position = transform.position;
        CardList.Instance.bookGroupcards.Add(card);
        OnCardSelectionChanged(card);
        {
            rect.sizeDelta += Vector2.right;
            rect.sizeDelta -= Vector2.right;
        }
    }

    public async UniTask DelBook_Card(BookCard.CardType cardType, float time = 0.25f)
    {
        for (int i = 0; i < CardList.Instance.bookGroupcards.Count; i++)
        {
            if (CardList.Instance.bookGroupcards[i].addCardType.bookCard.cardType == cardType)
            {
                Card card = CardList.Instance.bookGroupcards[i];
                CardList.Instance.bookGroupcards.Remove(CardList.Instance.bookGroupcards[i]);//移除了升级卡，并且将等级-1
                CardList.Instance.SaveCard();
                BookCardScore.Instance.Pre_Score_And_Power(card.addCardType.bookCard.cardType);
                await BookCardScore.Instance.BookCardType_LevelUp_OR_Down(card.addCardType.bookCard, false);//动画
                await UniTask.Delay(TimeSpan.FromSeconds(time));
                await BookCardDeck.Instance.UpdateBookCardUILevelUp_OR_Down(card, false);
                return;
            }
        }        
    }
  
  
    #region 鼠标事件
    private void BeginDrag(Card card)
    {
        selectedCard = card;
    }

    private void EndDrag(Card card)
    {
        if (selectedCard == null) return;
        selectedCard.transform.DOLocalMove(selectedCard.Selected ? new Vector3(0, selectedCard.selectionOffset, 0) : Vector3.zero, tweenCardReturn).SetEase(Ease.OutBack);
        {
            rect.sizeDelta += Vector2.right;
            rect.sizeDelta -= Vector2.right;
        }
        selectedCard = null;
    }

    private async void CardPointerEnter(Card card)
    {
        hoveredCard = card;
        await BookCardDeck.Instance.ShowBookCard(hoveredCard,false);
    }

    private async void CardPointerExit(Card card)
    {
        hoveredCard = null;
        await BookCardDeck.Instance.ShowBookCard(hoveredCard,false);
    }

    private void OnCardSelectionChanged(Card card)
    {
        foreach (var card_ in CardList.Instance.bookGroupcards) card_.itcanbeSelect = false;
    }
    #endregion

   
}