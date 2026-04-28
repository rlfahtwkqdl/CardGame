using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Components")]
    public TextMeshProUGUI text;
    public Image frontImage; // 카드 앞면의 그림을 담당할 Image 컴포넌트

    [Header("Settings")]
    public int cardNumber;
    public float rotationSpeed = 15f;
    public bool isFront = false;
    public bool isMatched = false;

    // 앞면 뒷면 정의
    private Quaternion frontRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion backRotation = Quaternion.Euler(0, 180f, 0);

    [HideInInspector] public CardGame cardGame;

    void Update()
    {
        // 부드러운 회전
        Quaternion targetRotation = isFront ? frontRotation : backRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 각도에 따라 앞면  숨기기
        float yAngle = transform.eulerAngles.y;

        // 0도에 가까우면(앞면) 보이고, 180도에 가까우면(뒷면) 안 보이게
        bool isFaceUp = (yAngle < 90f || yAngle > 270f);

        if (frontImage != null) frontImage.enabled = isFaceUp;
        if (text != null) text.enabled = isFaceUp;
    }

    public void ClickCard()
    {
        // 이미 짝을 맞췄거나, 이미 앞면인 카드는 눌러도 아무 일도 안 일어나게 차단
        if (isMatched || isFront) return;

        // 게임 매니저에게 클릭됐다고 알림. 
        if (cardGame != null)
        {
            cardGame.OnClickCard(this);
        }
    }

    
    public void SetCard(int newNumber, Sprite newSprite)
    {
        
        if (text == null) text = GetComponentInChildren<TextMeshProUGUI>();
        if (frontImage == null) frontImage = transform.Find("FrontImage").GetComponent<Image>();

        cardNumber = newNumber;
        text.text = cardNumber.ToString();

        
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
