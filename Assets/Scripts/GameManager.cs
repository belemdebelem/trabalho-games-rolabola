using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Text scoreText;
    [SerializeField] private Text endMessageText;
    [SerializeField, Min(1)] private int targetScore = 12;

    private int score;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("A cena deve ter apenas um GameManager.", this);
            return;
        }

        Instance = this;
        score = 0;
        UpdateScoreText();

        if (endMessageText != null)
        {
            endMessageText.text = string.Empty;
        }
    }

    public void CollectItem()
    {
        if (score >= targetScore)
        {
            return;
        }

        score++;
        UpdateScoreText();

        if (score == targetScore && endMessageText != null)
        {
            endMessageText.text = "Parabens! Voce coletou todos os cubos!";
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Pontuacao: " + score + " / " + targetScore;
        }
    }
}
