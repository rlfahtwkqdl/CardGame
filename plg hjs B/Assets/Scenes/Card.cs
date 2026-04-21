using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int cardNumber;
    public float rotationSpeed = 15f; // 너무 느리면 답답할 수 있으니 15 정도로 고정/추천
    public bool isFront = false;
    public bool isMatched = false;

    // 명확하게 앞면(0도), 뒷면(180도)으로 정의
    private Quaternion frontRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion backRotation = Quaternion.Euler(0, 180f, 0);

    public CardGame cardGame;

    void Update()
    {
        // isFront 상태에 따라 앞면 또는 뒷면으로 부드럽게 회전
        Quaternion targetRotation = isFront ? frontRotation : backRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void ClickCard()
    {
        // 이미 짝을 맞췄거나, 이미 앞면인 카드는 눌러도 아무 일도 안 일어나게 차단
        if (isMatched || isFront) return;

        // 게임 매니저에게 나(this) 클릭됐다고 알림. 
        // 카드를 뒤집는(isFront = true) 역할은 CardGame.cs의 OnClickCard 함수가 해줍니다.
        if (cardGame != null)
        {
            cardGame.OnClickCard(this);
        }
    }

    public void SetCardNumber(int newNumber)
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        cardNumber = newNumber;
        text.text = cardNumber.ToString();
    }

    public void ChangeColor(Color newcolor)
    {
        GetComponent<Image>().color = newcolor;
    }
}
