using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Components")]
    public TextMeshProUGUI text;
    public Image frontImage; // [추가] 카드 앞면의 그림을 담당할 Image 컴포넌트

    [Header("Settings")]
    public int cardNumber;
    public float rotationSpeed = 15f;
    public bool isFront = false;
    public bool isMatched = false;

    // 명확하게 앞면(0도), 뒷면(180도)으로 정의
    private Quaternion frontRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion backRotation = Quaternion.Euler(0, 180f, 0);

    [HideInInspector] public CardGame cardGame;

    void Update()
    {
        // 1. 부드러운 회전 로직 (기존과 동일)
        Quaternion targetRotation = isFront ? frontRotation : backRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 2. [추가] 각도에 따라 앞면 내용(그림, 숫자) 숨기기
        // transform.eulerAngles.y는 현재 카드의 Y축 회전값입니다.
        float yAngle = transform.eulerAngles.y;

        // 0도에 가까우면(앞면) 보이고, 180도에 가까우면(뒷면) 안 보이게 설정
        // 보통 90도를 기준으로 앞/뒤를 판단합니다.
        bool isFaceUp = (yAngle < 90f || yAngle > 270f);

        if (frontImage != null) frontImage.enabled = isFaceUp;
        if (text != null) text.enabled = isFaceUp;
    }

    public void ClickCard()
    {
        // 이미 짝을 맞췄거나, 이미 앞면인 카드는 눌러도 아무 일도 안 일어나게 차단
        if (isMatched || isFront) return;

        // 게임 매니저에게 나(this) 클릭됐다고 알림. 
        if (cardGame != null)
        {
            cardGame.OnClickCard(this);
        }
    }

    // [수정] 숫자와 함께 그림(Sprite)도 같이 받도록 함수 변경
    public void SetCard(int newNumber, Sprite newSprite)
    {
        // 컴포넌트가 제대로 연결 안 되었을 때를 대비해 한 번 더 체크 (성능을 위해 인스펙터 연결 권장)
        if (text == null) text = GetComponentInChildren<TextMeshProUGUI>();
        if (frontImage == null) frontImage = transform.Find("FrontImage").GetComponent<Image>();

        cardNumber = newNumber;
        text.text = cardNumber.ToString();

        // [추가] 전달받은 그림을 이미지 컴포넌트에 할당
        if (newSprite != null)
        {
            frontImage.sprite = newSprite;
        }
    }

    public void ChangeColor(Color newcolor)
    {
        GetComponent<Image>().color = newcolor;
    }
}
