using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Starting Values")]
    public float startingMoney = 0f;
    public float startingReputation = 30f;

    [Header("Penalties")]
    public float moneyWrongClear = 500f;
    public float moneyWrongDestroy = 300f;
    public float repWrong = 10f;

    public float Money { get; private set; }
    public float Reputation { get; private set; }   // 0 – 100

    // Snapshot taken at the start of each day
    public float DayStartMoney { get; private set; }
    public float DayStartReputation { get; private set; }

    // Gains this day (can be negative)
    public float MoneyGainedToday => Money - DayStartMoney;
    public float ReputationGainedToday => Reputation - DayStartReputation;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Only set starting values on first load
        Money = startingMoney;
        Reputation = startingReputation;

        TakeSnapshot();
    }

    // Call this at the start of each new day to reset the "gained today" counters
    public void TakeSnapshot()
    {
        DayStartMoney = Money;
        DayStartReputation = Reputation;
    }

    private void RewardCorrect()
    {
        Reputation = Mathf.Clamp(Reputation + 1f, 0f, 100f);
        Money += Reputation * 0.10f;
    }

    public void RecordCorrectClear() { RewardCorrect(); }
    public void RecordCorrectDestroy() { RewardCorrect(); }
    public void RecordWrongClear() { Money -= moneyWrongClear; Reputation = Mathf.Clamp(Reputation - repWrong, 0f, 100f); }
    public void RecordWrongDestroy() { Money -= moneyWrongDestroy; Reputation = Mathf.Clamp(Reputation - repWrong, 0f, 100f); }

    void OnGUI()
    {
        float panelW = 260f;
        float panelX = Screen.width - panelW - 10f;
        float panelY = 90f;
        float panelH = 90f;

        GUI.color = new Color(0f, 0f, 0f, 0.65f);
        GUI.DrawTexture(new Rect(panelX - 10, panelY - 8, panelW, panelH), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 20;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.normal.textColor = new Color(0.2f, 1f, 0.4f);

        GUI.Label(new Rect(panelX, panelY, panelW, 28), $"$ {Money:F2}", labelStyle);

        GUIStyle repStyle = new GUIStyle(labelStyle);
        repStyle.fontSize = 16;
        repStyle.normal.textColor = RepColour(Reputation);

        GUI.Label(new Rect(panelX, panelY + 30, panelW, 22), $"Reputation: {Reputation:F0} / 100", repStyle);

        float barX = panelX;
        float barY = panelY + 54f;
        float barW = panelW - 20f;
        float barH = 14f;

        GUI.color = new Color(1f, 1f, 1f, 0.2f);
        GUI.DrawTexture(new Rect(barX, barY, barW, barH), Texture2D.whiteTexture);

        GUI.color = RepColour(Reputation);
        GUI.DrawTexture(new Rect(barX, barY, barW * (Reputation / 100f), barH), Texture2D.whiteTexture);

        GUI.color = Color.white;
    }

    private Color RepColour(float rep)
    {
        if (rep < 50f)
            return Color.Lerp(new Color(1f, 0.2f, 0.2f), new Color(1f, 0.85f, 0f), rep / 50f);
        else
            return Color.Lerp(new Color(1f, 0.85f, 0f), new Color(0.2f, 1f, 0.4f), (rep - 50f) / 50f);
    }
}