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

    // [추가] 카드 앞면에 보여줄 그림(Sprite)들의 목록을 인스펙터에서 넣어주세요.
    // 최소한 pairCount의 개수만큼 그림이 필요합니다.
    public List<Sprite> cardSprites;

    private List<Card> cards = new List<Card>(); // 생성된 카드들을 담을 리스트
    private Card firstCard = null;
    private Card secondCard = null;

    // 기본값을 true로 두어 시작 연출 전엔 클릭 차단
    public bool isChecking = true;

    void Start()
    {
        SetupCards(); // 카드 생성 및 배치
        StartGame();  // 게임 로직 시작
    }

    private void SetupCards()
    {
        // [안전장치 추가] 그림 개수가 충분한지 확인
        if (cardSprites == null || cardSprites.Count < pairCount)
        {
            Debug.LogError($"Error: Not enough Sprites in 'Card Sprites' list. Need {pairCount}, but only have {(cardSprites == null ? 0 : cardSprites.Count)}.");
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
        // 셔플된 숫자 리스트 생성 (전체 카드 수 전달)
        List<int> randomPairNumbers = GeneratPairNumbers(cards.Count);

        for (int i = 0; i < cards.Count; i++)
        {
            int pairId = randomPairNumbers[i]; // 이 카드의 페어 ID (0,0,1,1,...)

            // [수정] 카드 스크립트의 새 함수를 호출하며 숫자와 그 ID에 맞는 그림을 같이 전달
            // 페어 ID가 곧 그림 리스트의 인덱스가 됩니다.
            cards[i].SetCard(pairId, cardSprites[pairId]);
        }

        StartCoroutine(ShowCardsAtStart());
    }

    // [새로 추가된 기능] 시작 시 카드를 보여주는 로직
    IEnumerator ShowCardsAtStart()
    {
        isChecking = true; // 클릭 금지

        // 1. 모든 카드를 앞면으로 보여줍니다.
        foreach (var card in cards)
        {
            card.isFront = true;
        }

        // 2. 2초 동안 플레이어가 외울 시간을 줍니다. (원하는 시간으로 수정 가능)
        yield return new WaitForSeconds(2.0f);

        // 3. 다시 모든 카드를 뒷면으로 뒤집습니다.
        foreach (var card in cards)
        {
            card.isFront = false;
        }

        // 4. 뒤집히는 애니메이션 시간을 살짝 기다려준 후 클릭을 허용합니다.
        yield return new WaitForSeconds(0.5f);
        isChecking = false;
    }

    public void OnClickCard(Card card)
    {
        // 확인 중이거나 게임 시작 시 연출 중일 때는 입력을 무시합니다.
        if (isChecking) return;

        // 게임 매니저가 클릭을 허락했으므로 카드를 앞면으로 바꿉니다.
        card.isFront = true;

        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null)
        {
            secondCard = card;
            isChecking = true; // 두 장을 다 골랐으니 확인 끝날 때까지 클릭 금지
            CheckCard();
        }
    }

    public void CheckCard()
    {
        if (firstCard.cardNumber == secondCard.cardNumber)
        {
            // [맞았을 때] 카드를 유지하고 색상 변경
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
            // [틀렸을 때] 1초 대기 후 카드를 되돌리는 코루틴 실행
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

        // 정석적인 피셔-예이츠 셔플 알고리즘으로 수정
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
