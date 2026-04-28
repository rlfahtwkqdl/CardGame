using UnityEngine;
using UnityEngine.UI; // [추가] Sprite를 사용하기 위해 필요
using System.Collections;
using System.Collections.Generic;

public class CardGame : MonoBehaviour
{
    [Header("Game Settings")]
    public int pairCount = 10; // 인스펙터에서 수정 가능 (10이면 20장 생성)
    public GameObject cardPrefab; // 카드 프리팹 연결
    public Transform cardParent; // 카드가 생성될 부모 (Grid Layout Group이 있는 오브젝트)

    public List<Sprite> cardSprites;

    private List<Card> cards = new List<Card>(); // 생성된 카드들을 담을 리스트
    private Card firstCard = null;
    private Card secondCard = null;

    // 기본값을 true로 두어 시작 연출 전엔 클릭 차단
    public bool isChecking = true;

    void Start()
    {
        SetupCards(); 
        StartGame(); 
    }

    private void SetupCards()
    {
        // 그림 개수가 충분한지 확인
        if (cardSprites == null || cardSprites.Count < pairCount)
        {
            Debug.LogError($"Error: 게임이 비명을 지르고 있는 것 같다.");
            return;
        }

        int totalCards = pairCount * 2;

        for (int i = 0; i < totalCards; i++)
        {
            GameObject newCardObj = Instantiate(cardPrefab, cardParent);
            Card cardScript = newCardObj.GetComponent<Card>();

            if (cardScript != null)
            {
                cards.Add(cardScript);
                cardScript.cardGame = this;
            }
        }
    }

    private void StartGame()
    {
        // 셔플된 숫자 리스트 맹긂
        List<int> randomPairNumbers = GeneratPairNumbers(cards.Count);

        for (int i = 0; i < cards.Count; i++)
        {
            int pairId = randomPairNumbers[i]; // 이 카드의 페어 ID

            cards[i].SetCard(pairId, cardSprites[pairId]);
        }

        StartCoroutine(ShowCardsAtStart());
    }

    // 시작 시 카드를 보여주는 로직
    IEnumerator ShowCardsAtStart()
    {
        isChecking = true; // 클릭 금지

        // 1. 모든 카드를 앞면으로 보여줍니다.
        foreach (var card in cards)
        {
            card.isFront = true;
        }

        // 외울 시간 주기
        yield return new WaitForSeconds(0.9f);

        // 모든 카드를 뒷면으로
        foreach (var card in cards)
        {
            card.isFront = false;
        }

        // 뒤집고 클릭 잠시 금지 후 허용
        yield return new WaitForSeconds(0.5f);
        isChecking = false;
    }

    public void OnClickCard(Card card)
    {
        // 확인 중이거나 게임 시작 시 연출 중일 때는 입력을 무시
        if (isChecking) return;

        // 게임 매니저가 클릭을 허락했으므로 카드를 앞면으로
        card.isFront = true;

        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null)
        {
            secondCard = card;
            isChecking = true; // 확인 끝날 때까지 클릭 금지
            CheckCard();
        }
    }

    public void CheckCard()
    {
        if (firstCard.cardNumber == secondCard.cardNumber)
        {
            // 맞았을 때 카드를 유지하고 색상 변경
            firstCard.isMatched = true;
            secondCard.isMatched = true;
            firstCard.ChangeColor(Color.magenta);
            secondCard.ChangeColor(Color.magenta);

            // 초기화 후 바로 다음 카드를 누를 수 있게 잠금 해제
            firstCard = null;
            secondCard = null;
            isChecking = false;
        }
        else
        {
            // 틀렸을 때 1초 대기 후 카드를 되돌림
            StartCoroutine(HideCardsAfterDelay());
        }
    }

    // 1초 보여주고 되돌리기
    IEnumerator HideCardsAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);

        // 카드 다시 뒷면으로 뒤집기
        firstCard.isFront = false;
        secondCard.isFront = false;

        // 변수 초기화 및 클릭 허용
        firstCard = null;
        secondCard = null;
        isChecking = false;
    }

    List<int> GeneratPairNumbers(int cardCount)
    {
        int pairCount = cardCount / 2;
        List<int> newCardNumbers = new List<int>();

        for (int i = 0; i < pairCount; ++i)
        {
            newCardNumbers.Add(i);
            newCardNumbers.Add(i);
        }

       
        for (int i = newCardNumbers.Count - 1; i > 0; i--)
        {
            int rnd = UnityEngine.Random.Range(0, i + 1);
            int temp = newCardNumbers[i];
            newCardNumbers[i] = newCardNumbers[rnd];
            newCardNumbers[rnd] = temp;
        }

        return newCardNumbers;
    }
}
