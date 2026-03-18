using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Starting Values")]
    public int startingMoney = 0;
    public float startingReputation = 15f;

    [Header("Rewards")]
    public int moneyCorrectClear = 200;
    public int moneyCorrectDestroy = 150;

    [Header("Penalties")]
    public int moneyWrongClear = 500;   // let through invalid VISA
    public int moneyWrongDestroy = 300;   // destroyed valid VISA
    public float repCorrect = 5f;
    public float repWrong = 10f;

    public int Money { get; private set; }
    public float Reputation { get; private set; }   // 0 – 100

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        Money = startingMoney;
        Reputation = startingReputation;
    }

    // Call these from BoatInspector after each decision
    public void RecordCorrectClear() { Money += moneyCorrectClear; Reputation = Mathf.Clamp(Reputation + repCorrect, 0f, 100f); }
    public void RecordCorrectDestroy() { Money += moneyCorrectDestroy; Reputation = Mathf.Clamp(Reputation + repCorrect, 0f, 100f); }
    public void RecordWrongClear() { Money -= moneyWrongClear; Reputation = Mathf.Clamp(Reputation - repWrong, 0f, 100f); }
    public void RecordWrongDestroy() { Money -= moneyWrongDestroy; Reputation = Mathf.Clamp(Reputation - repWrong, 0f, 100f); }

    void OnGUI()
    {
        float panelW = 260f;
        float panelX = Screen.width - panelW - 10f;
        float panelY = 90f;
        float panelH = 90f;

        // Background panel
        GUI.color = new Color(0f, 0f, 0f, 0.65f);
        GUI.DrawTexture(new Rect(panelX - 10, panelY - 8, panelW, panelH), Texture2D.whiteTexture);
        GUI.color = Color.white;

        // --- Money ---
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 20;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.normal.textColor = new Color(0.2f, 1f, 0.4f);   // green

        GUI.Label(new Rect(panelX, panelY, panelW, 28), $"$ {Money:N0}", labelStyle);

        // --- Reputation label ---
        GUIStyle repStyle = new GUIStyle(labelStyle);
        repStyle.fontSize = 16;
        repStyle.normal.textColor = RepColour(Reputation);

        GUI.Label(new Rect(panelX, panelY + 30, panelW, 22), $"Reputation: {Reputation:F0} / 100", repStyle);

        // --- Reputation bar ---
        float barX = panelX;
        float barY = panelY + 54f;
        float barW = panelW - 20f;
        float barH = 14f;

        // Background track
        GUI.color = new Color(1f, 1f, 1f, 0.2f);
        GUI.DrawTexture(new Rect(barX, barY, barW, barH), Texture2D.whiteTexture);

        // Fill
        GUI.color = RepColour(Reputation);
        GUI.DrawTexture(new Rect(barX, barY, barW * (Reputation / 100f), barH), Texture2D.whiteTexture);

        GUI.color = Color.white;
    }

    // Colour shifts from red → yellow → green based on reputation
    private Color RepColour(float rep)
    {
        if (rep < 50f)
            return Color.Lerp(new Color(1f, 0.2f, 0.2f), new Color(1f, 0.85f, 0f), rep / 50f);
        else
            return Color.Lerp(new Color(1f, 0.85f, 0f), new Color(0.2f, 1f, 0.4f), (rep - 50f) / 50f);
    }
}