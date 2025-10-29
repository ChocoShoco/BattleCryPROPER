using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FightUIManager : MonoBehaviour
{
    public static FightUIManager Instance;

    [Header("UI Elements")]
    public Slider player1Health;
    public Slider player2Health;
    public TextMeshProUGUI roundText;

    void Awake() => Instance = this;

    public void UpdateHealth(int player, float percent)
    {
        if (player == 1) player1Health.value = percent;
        else player2Health.value = percent;
    }

    public void ShowMessage(string message, float duration = 2f)
    {
        roundText.text = message;
        roundText.gameObject.SetActive(true);
        CancelInvoke();
        Invoke(nameof(HideMessage), duration);
    }

    void HideMessage() => roundText.gameObject.SetActive(false);
}
