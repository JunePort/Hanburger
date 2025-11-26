using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using static FoodCard;
using System.Collections;

public class BookCardDeck : MonoBehaviour
{
    [SerializeField] private Card selectedCard;//ѡ��Ƭ
    [SerializeReference] private Card hoveredCard;//��ͣ��Ƭ

    [SerializeField] private GameObject BookBGPrefab;
    [SerializeField] private GameObject CardSoltPrefab;
    [SerializeField] private GameObject PopWindowPrefab;
    [SerializeField] private GameObject bookCardShowPanel;
    [SerializeField] private GameObject bookCardShowExcil;
    [SerializeField] private GameObject cardshow;

    private readonly List<Card> cards_show = new();
    private readonly List<GameObject> BookCardBGObjs = new();
    private readonly List<BookCard.CardType> uiCardTypes = new();
    private GameObject PopWindow;
    [Header("��������")]
    public List<BookCard> cardtypes;

    private const float tweenCardReturn = 2.0f;//�Ƿ����ÿ�Ƭ��λ�Ķ���ʱ��
    private int cardNameCount = 0;
    [Header("�Ƿ�����ÿ���")]
    public bool isGetTheMostCard;
    public List<BookCard.CardType> mostCardTypes = new();
    private bool twice;

    [Header("��ʳ��")]
    public bool isShowBookCard;
    private static BookCardDeck _instance;
    public static BookCardDeck Instance { get { if (_instance == null) _instance = FindObjectOfType<BookCardDeck>(); return _instance; } }

    #region ��ʼ��
    private async void Start()
    {
        PopWindow = Instantiate(PopWindowPrefab, transform.parent.transform.Find(transform.parent.name + "Show"));
        PopWindow.transform.SetSiblingIndex(0);
        cardtypes = CardList.Instance.GetBookCardTypes();

        for (int i = 0; i < cardtypes.Count; i++) cardtypes[i].Level_1();//��ʼ�����Ƶȼ�      
        for (int i = 0; i < cardtypes.Count; i++)
        {
            BookCardBGObjs.Add(Instantiate(BookBGPrefab, bookCardShowExcil.transform));
            await CreateBookCard(cardtypes[i], cards_show, bookCardShowPanel.transform);
        }
        await SetBookCard(cardtypes, BookCardBGObjs.ToArray());
        for (int i = 0; i < CardList.Instance.BookCardCount; i++)
        {
            await CreateBookCard(cardtypes[i % cardtypes.Count], CardList.Instance.bookcards, transform);//����Ҫ��һ��
        }
        bookCardShowPanel.SetActive(false);
        bookCardShowExcil.SetActive(false);
    }
    public async UniTask CreateBookCard(BookCard cardType, List<Card> cards, Transform parent = null)
    {
        GameObject cardsolt = Instantiate(CardSoltPrefab, parent);
        Card card = cardsolt.transform.GetChild(0).gameObject.GetComponent<Card>();
        card.addCardType = ScriptableObject.CreateInstance<AddCardType>();
        await UniTask.Yield();
        card.addCardType.cardType = AddCardType.CardType.isBookCard;
        card.addCardType.bookCard = cardType;
        await UniTask.Yield();
        card.cardVisual.cardImage.sprite = card.addCardType.bookCard.GetCardSprite();
        cards.Add(card);
        if (parent != transform)
        {
            await Refresh_Card(card.GetComponent<Card>(), bookCardShowPanel.transform);
            card.cardVisual.transform.position = this.transform.position;
            card.cardVisual.transform.localScale = Vector3.zero;
        }
    }
    private async UniTask Refresh_Card(Card card, Transform parent = null)
    {
        await UniTask.Yield();
        if (card == null) return;
        card.transform.parent.transform.SetParent(parent);
        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(BeginDrag);
        card.EndDragEvent.AddListener(EndDrag);
        card.OnSelectedChanged += OnCardSelectionChanged;
        card.name = cardNameCount.ToString();//����ֻ�����ֶ���
        card.Deselect();
        cardNameCount++;
    }

    #endregion

    #region GoodCard
    public static void GOOD_CARD_MARKET(int LevelUp = 0)
    {

        BookCardDeck instance = Instance; // ��ȡΨһʵ��
        Debug.Log("GoodCardShop GOOD_CARD_Market");
        switch (LevelUp)
        {
            case 0: instance.isGetTheMostCard = false; break;
            case 1: instance.isGetTheMostCard = true; break;
            case 2: GoodCardShop.Instance.isFreeMostBookCard = true; break;
        };
    }
    #endregion

    #region �ⲿ�ӿ�
    public Card GetCard(BookCard bookCard = null)
    {
        if (CardList.Instance.bookcards.Count > 0)
        {
            Card cardToMove = bookCard ? FindCard(CardList.Instance.bookcards, bookCard) : Random(CardList.Instance.bookcards, isGetTheMostCard && twice);
            twice = !twice;
            CardList.Instance.bookcards.Remove(cardToMove);
            return cardToMove;
        }
        return null;
    }
    private Card FindCard(List<Card> cards, BookCard bookCard)//��ʼ���浵�õ�
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].addCardType.bookCard.cardType == bookCard.cardType)
            {
                (cards[^1], cards[i]) = (cards[i], cards[^1]);
                break;
            }
        }
        _ = UpdateBookCardUILevelUp_OR_Down(cards[^1]);

        return cards[^1];
    }


    public void FindMostCardType()
    {
        mostCardTypes.Clear();
        // �ҵ����ʹ�ô���
        int maxUseNum = cardtypes[0].GetUseNum();
        foreach (var card in cardtypes) if (card.GetUseNum() > maxUseNum) maxUseNum = card.GetUseNum();
        foreach (var card in cardtypes) if (card.GetUseNum() == maxUseNum && !mostCardTypes.Contains(card.cardType)) mostCardTypes.Add(card.cardType);
        // ������
        if (mostCardTypes.Count == 1)
        {
            Debug.Log("��ÿ��ƣ�" + mostCardTypes[0]);
        }
        else if (mostCardTypes.Count > 1)
        {
            Debug.Log("��ÿ��ƣ����У���" + string.Join(", ", mostCardTypes));
        }
    }

    public bool IsMostCardType(BookCard.CardType cardType)
    {
        FindMostCardType();
        return mostCardTypes.Contains(cardType);
    }
    private Card Random(List<Card> cards, bool isMost = false)
    {
        if (isMost)
        {
            bool isHaveMostCard = false;
            for (int i = 0; i < cards.Count; i++)
            {
                if (IsMostCardType(cards[i].addCardType.bookCard.cardType))
                {
                    (cards[^1], cards[i]) = (cards[i], cards[^1]);
                    isHaveMostCard = true;
                    break;
                }
            }
            if (!isHaveMostCard) { Debug.Log("û����ÿ���"); }
        }
        else
        {
            if (cards.Count == 0) return null;
            System.Random random = new(CardList.Instance.GetSeed());
            int randomIndex = random.Next(cards.Count);
            (cards[^1], cards[randomIndex]) = (cards[randomIndex], cards[^1]);
        }
        return cards[^1];
    }
    public List<Card> GetListCard(List<Card> cards, out BookCard bookcard)
    {
        BookCard.CardType cardType = GetCardType(cards);
        bookcard = cardtypes[0];
        for (int i = 0; i < cardtypes.Count; i++)
        {
            if (cardtypes[i].cardType == cardType)
            {
                bookcard = cardtypes[i];
                break;
            }
        }
        // ��ԭʼ˳����飬��������ԭʼ˳��
        Dictionary<FoodType, List<Card>> foodCardsByType = new();
        foreach (Card card in cards) // ��ԭʼ˳�����
        {
            if (card.addCardType == null || card.addCardType.cardType != AddCardType.CardType.isFoodCard)
                continue;

            FoodType foodType = card.addCardType.foodCard.cardType;
            if (!foodCardsByType.ContainsKey(foodType))
            {
                foodCardsByType[foodType] = new List<Card>();
            }
            foodCardsByType[foodType].Add(card); // ��ԭʼ˳�����ӵ�����
        }
        // ����FoodType���ȼ���������ɸѡʱ�����ȼ��жϣ����ı�˳��
        var typePriority = new Dictionary<FoodType, int>
    {
        { FoodType.Meats, 4 },
        { FoodType.Veget, 3 },
        { FoodType.Fruit, 2 },
        { FoodType.Bread, 1 }
    };
        // ɸѡ�����������飨��ԭʼ����˳��
        List<KeyValuePair<FoodType, List<Card>>> validGroups;

        switch (cardType)
        {
            case BookCard.CardType.Flush:
                // ͬ����ȡ����>=4���飨��ԭʼ����˳�����ȵ�һ��������������������飩
                validGroups = foodCardsByType.Where(kvp => kvp.Value.Count >= 4).ToList();
                if (validGroups.Any())
                {
                    int maxCount = validGroups.Max(g => g.Value.Count);
                    // �ӷ��������������ҵ�һ������������飨����ԭʼ˳��
                    var targetGroup = validGroups.First(g => g.Value.Count == maxCount);
                    if (validGroups.Count(g => g.Value.Count == maxCount) > 1)
                    {
                        // ������ͬʱ�����ȼ��ҵ�һ�����ϵ��飨����ԭʼ˳��
                        targetGroup = validGroups.Where(g => g.Value.Count == maxCount)
                                                 .OrderByDescending(g => typePriority[g.Key])
                                                 .First();
                    }
                    return targetGroup.Value; // ����˳����ԭʼ˳��
                }
                break;
            case BookCard.CardType.ThreeWithTwo: return cards;
            case BookCard.CardType.ThreeOfAKind:
                // ������ȡ����>=3���飨��ԭʼ˳��
                validGroups = foodCardsByType.Where(kvp => kvp.Value.Count >= 3).ToList();
                if (validGroups.Any())
                {
                    var targetGroup = validGroups.OrderByDescending(g => g.Value.Count)
                                                .ThenByDescending(g => typePriority[g.Key])
                                                .First();
                    return targetGroup.Value;
                }
                break;
            case BookCard.CardType.FullHouse:
                // ���òʣ�ÿ��ʳ������ȡ1�ţ��ϸ񱣳�ԭʼ�б��п��Ƶĳ���˳��
                List<Card> fullHouse = new();
                HashSet<FoodType> collectedTypes = new(); // ��¼���ռ�������

                // ��ԭʼcards�б�˳�������ȷ�����ı�˳��
                foreach (Card card in cards)
                {
                    AddCardType addCardType = card.addCardType;
                    if (addCardType == null || addCardType.cardType != AddCardType.CardType.isFoodCard)
                        continue;

                    FoodType foodType = addCardType.foodCard.cardType;

                    // �����δ�ռ��������ͣ����ӵ���������Ϊ���ռ�
                    if (!collectedTypes.Contains(foodType))
                    {
                        fullHouse.Add(card);
                        collectedTypes.Add(foodType);

                        // �ռ�������4�����ͺ���ǰ�˳�
                        if (collectedTypes.Count == 4)
                            break;
                    }
                }
                return fullHouse;
            case BookCard.CardType.TwoPairs:
                // ���ԣ�ѡ���������2�ŵĿ��ƣ��ϸ񱣳�ԭʼ˳�����ں�������ԭʼ����˳��
                List<Card> twoPairs = new();
                Dictionary<FoodType, List<Card>> tempGroups = new();
                HashSet<FoodType> selectedTypes = new();

                // 1. ��ԭʼ˳��������ƣ���������ԭʼ˳��ķ���
                foreach (Card card in cards)
                {
                    AddCardType addCardType = card.addCardType;
                    if (addCardType == null || addCardType.cardType != AddCardType.CardType.isFoodCard)
                        continue;

                    FoodType foodType = addCardType.foodCard.cardType;
                    if (!tempGroups.ContainsKey(foodType))
                    {
                        tempGroups[foodType] = new List<Card>();
                    }
                    tempGroups[foodType].Add(card); // ���ڿ���˳����ԭʼ�б�һ��
                }

                // 2. ɸѡ�����������飨������2�����������������������ȣ��ٰ����ȼ���
                var validPairs = tempGroups
                    .Where(kvp => kvp.Value.Count >= 2)
                    .OrderByDescending(g => g.Value.Count)
                    .ThenByDescending(g => typePriority[g.Key])
                    .Take(2) // ȡǰ����
                    .ToList();

                if (validPairs.Count < 2)
                    break; // �����������˳�

                // 3. ��ԭʼ�б�������״γ���˳���ռ��������֤���˳�����ԭʼ�߼���
                // ����ԭʼ���ƣ��ռ���ѡ�е������ǰ2�ţ�����ԭʼ˳��
                foreach (Card card in cards)
                {
                    AddCardType addCardType = card.addCardType;
                    if (addCardType == null || addCardType.cardType != AddCardType.CardType.isFoodCard)
                        continue;

                    FoodType foodType = addCardType.foodCard.cardType;

                    // ֻ������ѡ�е�����
                    if (!validPairs.Any(g => g.Key == foodType))
                        continue;

                    // ÿ��ֻȡǰ2��
                    var group = tempGroups[foodType];
                    int indexInGroup = group.IndexOf(card); // ����ԭʼ����
                    if (indexInGroup < 2 && !twoPairs.Contains(card))
                    {
                        twoPairs.Add(card);
                    }

                    // �ռ���4�ţ�2+2������ǰ�˳�
                    if (twoPairs.Count == 4)
                        break;
                }

                return twoPairs;
            case BookCard.CardType.Pair:
                // ���ӣ�ȡ����>=2���飨��ԭʼ˳��
                validGroups = foodCardsByType.Where(kvp => kvp.Value.Count >= 2).ToList();
                if (validGroups.Any())
                {
                    var targetGroup = validGroups.OrderByDescending(g => g.Value.Count)
                                                .ThenByDescending(g => typePriority[g.Key])
                                                .First();
                    return targetGroup.Value;
                }
                break;
            case BookCard.CardType.HighCard:
                // ���ƣ������ȼ��ҵ�һ���п��Ƶ��飬�������ڵ�һ�ţ�ԭʼ˳��
                foreach (var type in new List<FoodType> { FoodType.Meats, FoodType.Veget, FoodType.Fruit, FoodType.Bread })
                {
                    if (foodCardsByType.TryGetValue(type, out var typeCards) && typeCards.Any())
                    {
                        return new List<Card> { typeCards.First() }; // ���ڵ�һ����ԭʼ˳��ĵ�һ��
                    }
                }
                break;
            case BookCard.CardType.WelCome:
                return new List<Card>();
        }
        return new List<Card>();
    }
    #endregion

    #region ShowPanel

    private bool isShowBookCard_TAG;
    public void DeShowBookCard(){ if (isShowBookCard) ShowBookCard(); }
    void DeshowOther() { CookCardDeck.Instance.DeShowCookCard();FoodCardDeck.Instance.DeShowFoodCard(); GoodCardDeck.Instance.DeShowGoodCard(); GoodCardShop.Instance.DeShowCardShop(); }
    public async void ShowBookCard()
    {
        if (isShowBookCard_TAG) return;
        isShowBookCard_TAG = true;
        isShowBookCard = !isShowBookCard;      
        if (isShowBookCard)
        {
            DeshowOther();
            bookCardShowPanel.SetActive(true);
            bookCardShowExcil.SetActive(true);
            DoPopWindowScaleAndActive(PopWindow, true);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));           
            foreach (GameObject BookCardBGObj in BookCardBGObjs) DoCardScaleAndActive(BookCardBGObj, true);
            foreach (Card card in cards_show) DoCardScaleAndActive(card.cardVisual.gameObject, true);
        }
        else
        {
            foreach (Card card in cards_show) DoCardScaleAndActive(card.cardVisual.gameObject, false);
            foreach (GameObject BookCardBGObj in BookCardBGObjs) DoCardScaleAndActive(BookCardBGObj, false);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            DoPopWindowScaleAndActive(PopWindow, false);
            bookCardShowPanel.SetActive(false);
            bookCardShowExcil.SetActive(false);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        isShowBookCard_TAG = false;
    }
    private void DoCardScaleAndActive(GameObject card_, bool isActive)//0.4s
    {
        if (isActive)
        {
            card_.transform.localScale = Vector3.zero;
            card_.SetActive(true);
            card_.transform.DOScale(1.2f, 0.2f).OnComplete(() =>
            {
                card_.transform.DOScale(1f, 0.2f);
            });
        }
        else
        {
            card_.transform.DOScale(1.1f, 0.2f).OnComplete(() =>
            {
                card_.transform.DOScale(0f, 0.2f).OnComplete(() =>
                {
                    card_.SetActive(false);
                });
            });
        }
    }
    private void DoPopWindowScaleAndActive(GameObject popWindow, bool isActive)//0.4s
    {
        if (isActive)
        {
            popWindow.transform.GetComponent<RectTransform>().DOAnchorPos(new Vector2(500, 100), 0.5f).OnComplete(() =>
            {
                popWindow.transform.DOScale(1.1f, 0.2f).OnComplete(() =>
                {
                    popWindow.transform.DOScale(1f, 0.2f);
                });
            });
        }
        else
        {
            popWindow.transform.GetComponent<RectTransform>().DOAnchorPos(new Vector2(2000, 100), 0.5f);
        }
    }
    #endregion

    #region ����¼�
    private void BeginDrag(Card card)
    {
        selectedCard = card;
    }

    private void EndDrag(Card card)
    {
        if (selectedCard == null) return;
        selectedCard.transform.DOLocalMove(selectedCard.Selected ? new Vector3(0, selectedCard.selectionOffset, 0) : Vector3.zero, tweenCardReturn).SetEase(Ease.OutBack);
        selectedCard = null;
    }

    private async void CardPointerEnter(Card card)
    {
        hoveredCard = card;
        //await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        await ShowBookCard(hoveredCard);
    }

    private async void CardPointerExit(Card card)
    {
        hoveredCard = null;
        //await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        await ShowBookCard(hoveredCard);
    }


    private void OnCardSelectionChanged(Card card)
    {
        StartCoroutine(DelayedUpdateScore());

        static IEnumerator DelayedUpdateScore()
        {
            yield return null; // �ȴ���ǰ֡�����¼��������
            var selectedCards = CardList.Instance.bookcards.Where(c => c.Selected).ToList();
            int selectCount = selectedCards.Count;
            foreach (var card in CardList.Instance.bookcards) card.itcanbeSelect = false;
        }
    }
    #endregion

    #region ��Ƭ����
    public async UniTask UseBook_Card(Card bookCardObj, float time = 0.25f)
    {
        if (bookCardObj == null) return;
        Card card = bookCardObj.GetComponentInChildren<Card>();
        card.cardVisual.gameObject.SetActive(true);
        card.cardVisual.GetComponent<RectTransform>().DOShakePosition(time, 25f, 10, 90f, false);
        BookCardScore.Instance.Pre_Score_And_Power(card.addCardType.bookCard.cardType);
        await BookCardScore.Instance.BookCardType_LevelUp_OR_Down(card.addCardType.bookCard);//����
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        await UpdateBookCardUILevelUp_OR_Down(card);
        BookCardScore.Instance.Empty_Score();
        await BookCardGroup.Instance.Refresh_Card(card);
        card.transform.parent.transform.SetParent(transform);
        card.cardVisual.gameObject.SetActive(false);
    }





    // ��ȡ����BookCard��ͨ�÷���
    private BookCard FindBookCard(BookCard.CardType targetCardType)
    {
        return cardtypes.FirstOrDefault(card => card.cardType == targetCardType);
    }

    // ��ȡ����UI������ͨ�÷���
    private int FindUICardIndex(BookCard.CardType targetCardType)
    {
        for (int i = 0; i < BookCardBGObjs.Count && i < uiCardTypes.Count; i++)
        {
            if (uiCardTypes[i] == targetCardType)
            {
                return i;
            }
        }
        return -1;
    }

    private async UniTask SetBookCard(List<BookCard> cardtypes, GameObject[] bookCardObjs)//����ȫ���Ŀ���UI
    {
        uiCardTypes.Clear();
        foreach (var bookCardObj in bookCardObjs) bookCardObj.transform.localScale = Vector3.zero;

        int count = Mathf.Min(cardtypes.Count, bookCardObjs.Length);

        // ʹ�ò��д����������ܣ����UI����������
        var updateTasks = new List<UniTask>();
        for (int i = 0; i < count; i++)
        {
            uiCardTypes.Add(cardtypes[i].cardType);
            updateTasks.Add(UpdateSingleUI(bookCardObjs[i], cardtypes[i]));
        }

        await UniTask.WhenAll(updateTasks);
        await UniTask.Yield();
    }

    public async UniTask UpdateBookCardUILevelUp_OR_Down(Card card, bool isLevelUp = true)//ʹ�ÿ��Ƶȼ���1������UI
    {
        var addCardType = card.addCardType;
        // ����������ã������ظ���ȡ

        if (isLevelUp) { addCardType.bookCard.LevelUp(); } else { addCardType.bookCard.LevelDown(); }

        var dataCard = FindBookCard(addCardType.bookCard.cardType);
        int uiIndex = FindUICardIndex(addCardType.bookCard.cardType);
        if (dataCard == null) return;
        if (uiIndex != -1)
        {
            await UpdateSingleUI(BookCardBGObjs[uiIndex], dataCard);
        }
    }

    public async UniTask UpdateBookCardUIUseUp(BookCard.CardType targetCardType)//��ʾ����ʹ������+1
    {
        var dataCard = FindBookCard(targetCardType);
        if (dataCard == null) return;

        int uiIndex = FindUICardIndex(targetCardType);
        if (uiIndex != -1)
        {
            await UpdateSingleUI(BookCardBGObjs[uiIndex], dataCard, -1);
        }
    }
    #endregion

    #region CardShow�͸���Panel

    public async UniTask ShowBookCard(Card card, bool isCardDeckShow = true, float price_percent = 1.0f, bool isfreecard = false, float time = 0.2f)
    {
        if (isCardDeckShow) if (!isShowBookCard) return;
        if (card)
        {
            var bookCard = card.addCardType.bookCard;
            Transform show1 = cardshow.transform.Find("Show1");
            Transform show2 = cardshow.transform.Find("Show2");
            Transform show3 = cardshow.transform.Find("Show3");
            string text1 = bookCard.GetCardType(bookCard.cardType,false).ToString();
            string text2 = bookCard.GetDescription() + "\n" + "�÷�:" + bookCard.GetScore() + "/����:" + bookCard.GetPower();
            string text3 =
                isfreecard ? "���" : (
                    (price_percent == 1f ? "" : price_percent == 0.75f ? "����" : "���")
                    + "�ۼ�:" + ((int)(bookCard.GetPrice() * price_percent)).ToString()
                );

            int uiIndex = FindUICardIndex(bookCard.cardType);
            BookCardBGObjs[uiIndex].transform.DOScaleX(1.2f, 0.2f).OnComplete(() => { BookCardBGObjs[uiIndex].transform.DOScaleX(1f, 0.2f); });

            var tasks = new List<UniTask> {
            ShowTextWithFadeInOnly(show1.GetComponentsInChildren<TextMeshProUGUI>()[0], text1, time),
            ShowTextWithFadeInOnly(show2.GetComponentsInChildren<TextMeshProUGUI>()[0], text2, time),
            ShowTextWithFadeInOnly(show3.GetComponentsInChildren<TextMeshProUGUI>()[0], text3, time)};
            await UniTask.WhenAll(tasks);
        }

    }

    private async UniTask UpdateSingleUI(GameObject bg, BookCard bookcard, int isSetWay = 0, float time = 0.2f)//1>0>-1
    {
        Transform score_Transform = bg.transform.Find("Score/Score");
        Transform power_Transform = bg.transform.Find("Power/Power");
        Transform bookTyTransform = bg.transform.Find("BookType/BookType");
        Transform useNumTransform = bg.transform.Find("UseNum/UseNum");
        var tasks = new List<UniTask>();
        switch (isSetWay)
        {
            case -1:
                tasks.Add(ShowTextWithFadeInOnly(useNumTransform.GetComponentsInChildren<TextMeshProUGUI>()[0], bookcard.GetUseNum().ToString(), time));
                break;
            case 0:
                tasks.Add(ShowTextWithFadeInOnly(score_Transform.GetComponentsInChildren<TextMeshProUGUI>()[0], bookcard.GetScore().ToString(), time));
                tasks.Add(ShowTextWithFadeInOnly(power_Transform.GetComponentsInChildren<TextMeshProUGUI>()[0], bookcard.GetPower().ToString(), time));
                tasks.Add(ShowTextWithFadeInOnly(bookTyTransform.GetComponentsInChildren<TextMeshProUGUI>()[0], bookcard.GetCardType(bookcard.cardType), time));
                tasks.Add(ShowTextWithFadeInOnly(useNumTransform.GetComponentsInChildren<TextMeshProUGUI>()[0], bookcard.GetUseNum().ToString(), time));
                break;
            case 1:
                bg.transform.DOScale(1.15f, 0.2f).OnComplete(() => { bg.transform.DOScale(1f, 0.2f); });
                tasks.Add(ShowTextWithFadeInOnly(score_Transform.GetComponentsInChildren<TextMeshProUGUI>()[0], bookcard.GetScore().ToString(), time));
                tasks.Add(ShowTextWithFadeInOnly(power_Transform.GetComponentsInChildren<TextMeshProUGUI>()[0], bookcard.GetPower().ToString(), time));
                tasks.Add(ShowTextWithFadeInOnly(bookTyTransform.GetComponentsInChildren<TextMeshProUGUI>()[0], bookcard.GetCardType(bookcard.cardType), time));
                tasks.Add(ShowTextWithFadeInOnly(useNumTransform.GetComponentsInChildren<TextMeshProUGUI>()[0], bookcard.GetUseNum().ToString(), time));
                break;
            default:
                break;

        }
        await UniTask.WhenAll(tasks);
    }

    #endregion

    #region �������߷���

    private async UniTask ShowTextWithFadeInOnly(TMP_Text textComponent, string message, float duration = 0.4f, CancellationToken ct = default)
    {
        if (textComponent == null) return;

        if (!textComponent.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup = textComponent.gameObject.AddComponent<CanvasGroup>();
        }

        // ����ԭʼ��ɫ���ڶ������
        _ = textComponent.color;
        textComponent.text = message;
        canvasGroup.alpha = 0f;

        float fadeTime = Mathf.Clamp(duration, 0.05f, 1f);

        // ����DoFade����
        await DoFade(canvasGroup, 0f, 1f, fadeTime, ct);
    }

    private async UniTask DoFade(CanvasGroup cg, float from, float to, float duration, CancellationToken ct)
    {
        await UniTask.Create(async (token) =>
        {
            // ����͸���ȶ���
            var alphaTween = cg.DOFade(to, duration).From(from).SetEase(Ease.OutQuad);
            // �ȴ��������
            while (alphaTween.IsActive() && !alphaTween.IsComplete())
            {
                token.ThrowIfCancellationRequested();
                await UniTask.Yield(token);
            }
            // ȡ����������ֹ����
            if (token.IsCancellationRequested)
            {
                alphaTween.Kill();
                throw new OperationCanceledException(token);
            }
        }, ct);
    }
    #endregion

    #region �����������

    public float GetScore_Add(BookCard.CardType cardType)
    {
        return GetValueFromBookCardDeck(cardType, book => book.GetScore());
    }

    public float GetPower_Add(BookCard.CardType cardType)
    {
        return GetValueFromBookCardDeck(cardType, book => book.GetPower());
    }

    public string GetCard_Type(BookCard.CardType cardType)
    {
        return GetValueFromBookCardDeck(cardType, book => book.GetCardType(book.cardType, false), "");
    }

    private T GetValueFromBookCardDeck<T>(BookCard.CardType cardType, Func<BookCard, T> valueSelector, T defaultValue = default)
    {
        foreach (BookCard book in cardtypes)
        {
            if (book.cardType == cardType)
            {
                return valueSelector(book);
            }
        }
        return defaultValue;
    }

    public async UniTask BookCardCountUp(List<Card> cardsToRemove)
    {
        foreach (BookCard card in cardtypes)
        {
            if (card.cardType == GetCardType(cardsToRemove))
            {
                card.UseCountAdd();
                await UpdateBookCardUIUseUp(GetCardType(cardsToRemove));
                break;
            }
        }
    }
    #endregion

    #region ���ÿ�������
    /// <summary>�ж���Ƭ��ϵ�ʳ������</summary>
    public BookCard.CardType GetCardType(List<Card> cards)
    {
        if (cards.Count == 0) return BookCard.CardType.WelCome;
        Dictionary<FoodType, int> foodCount = new();
        foreach (Card card in cards)
        {
            AddCardType addCardType = card.addCardType;
            if (addCardType == null || addCardType.cardType != AddCardType.CardType.isFoodCard) continue;
            FoodType foodType = addCardType.foodCard.cardType;
            foodCount[foodType] = foodCount.TryGetValue(foodType, out int count) ? count + 1 : 1;
        }

        List<int> foodQuantities = foodCount.Values.ToList();
        int foodTypeCount = foodCount.Count;

        // 1. ͬ����Flush����ĳ��ʳ�ĳ���4/5�Σ����ȼ���ߣ�
        if (foodQuantities.Contains(4) || foodQuantities.Contains(5))
        {
            return BookCard.CardType.Flush;
        }

        // 2. ��������ThreeWithTwo����ͬʱ���ڡ�3�Ρ��͡�2�Ρ���ʳ�ģ���3+2��ϣ�
        bool hasThree = foodQuantities.Contains(3);
        bool hasTwo = foodQuantities.Contains(2);
        if (hasThree && hasTwo)
        {
            return BookCard.CardType.ThreeWithTwo;
        }

        // 3. ������ThreeOfAKind����ĳ��ʳ�ĳ���3�Σ��Ҳ�ͬʱ������������
        if (hasThree)
        {
            return BookCard.CardType.ThreeOfAKind;
        }

        // 4. ���òʣ�FullHouse��������ȫ��4��ʳ������
        if (foodTypeCount == 4)
        {
            return BookCard.CardType.FullHouse;
        }

        // 5. ���ԣ�TwoPairs��������2��ʳ�ĸ�����2�Σ���2+2��ϣ�
        int twoPairCount = foodQuantities.Count(qty => qty == 2);
        if (twoPairCount >= 2)
        {
            return BookCard.CardType.TwoPairs;
        }

        // 6. ���ӣ�Pair������1��ʳ�ĳ���2�Σ��Ҳ���2�ԣ�
        if (twoPairCount == 1)
        {
            return BookCard.CardType.Pair;
        }

        // 7. ���ƣ�HighCard�����������κ����
        return BookCard.CardType.HighCard;
    }



    #endregion

}