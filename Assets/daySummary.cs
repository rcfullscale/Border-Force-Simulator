using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DaySummary : MonoBehaviour
{
    private bool isShowing = false;
    private BoatInspector boatInspector;

    void Start()
    {
        boatInspector = FindFirstObjectByType<BoatInspector>();
    }

    public void ShowSummary()
    {
        isShowing = true;
    }

    void Update()
    {
        if (!isShowing) return;
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            // Take a fresh snapshot so "gained today" resets for the new day
            PlayerStats.Instance?.TakeSnapshot();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    string GetRank(int total)
    {
        if (total < 20) return "Student";
        if (total <= 80) return "First Officer";
        if (total <= 150) return "Captain";
        return "Mission Coordinator";
    }

    string GetNextRankProgress(int total)
    {
        if (total < 20) return $"{20 - total} integrations until First Officer";
        if (total <= 80) return $"{81 - total} integrations until Captain";
        if (total <= 150) return $"{151 - total} integrations until Mission Coordinator";
        return "Maximum rank achieved";
    }

    // Helper: prefix a float with + or nothing (negatives already have -)
    string Signed(float val, string format = "F2")
        => (val >= 0 ? "+" : "") + val.ToString(format);

    void OnGUI()
    {
        if (!isShowing) return;

        GUI.color = new Color(0f, 0f, 0f, 0.85f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        float cx = Screen.width / 2f;
        float cy = Screen.height / 2f;

        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 40;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.white;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle bodyStyle = new GUIStyle();
        bodyStyle.fontSize = 26;
        bodyStyle.normal.textColor = Color.white;
        bodyStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle goodStyle = new GUIStyle(bodyStyle);
        goodStyle.normal.textColor = Color.green;

        GUIStyle badStyle = new GUIStyle(bodyStyle);
        badStyle.normal.textColor = new Color(1f, 0.4f, 0.4f);

        GUIStyle hintStyle = new GUIStyle();
        hintStyle.fontSize = 18;
        hintStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
        hintStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle rankStyle = new GUIStyle();
        rankStyle.fontSize = 22;
        rankStyle.fontStyle = FontStyle.Bold;
        rankStyle.normal.textColor = new Color(1f, 0.85f, 0.2f);
        rankStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle progressStyle = new GUIStyle();
        progressStyle.fontSize = 16;
        progressStyle.normal.textColor = new Color(0.6f, 0.85f, 1f);
        progressStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle subStyle = new GUIStyle(bodyStyle);
        subStyle.fontSize = 20;
        subStyle.normal.textColor = new Color(0.75f, 0.75f, 0.75f);

        int success = boatInspector != null ? boatInspector.successfulIntegrations : 0;
        int fail = boatInspector != null ? boatInspector.failures : 0;
        int total = success + fail;
        string rating = total == 0 ? "N/A" : $"{Mathf.RoundToInt((float)success / total * 100f)}%";
        string rank = GetRank(success);
        string progress = GetNextRankProgress(success);

        // Money & reputation from PlayerStats
        float money = PlayerStats.Instance != null ? PlayerStats.Instance.Money : 0f;
        float rep = PlayerStats.Instance != null ? PlayerStats.Instance.Reputation : 0f;
        float moneyGained = PlayerStats.Instance != null ? PlayerStats.Instance.MoneyGainedToday : 0f;
        float repGained = PlayerStats.Instance != null ? PlayerStats.Instance.ReputationGainedToday : 0f;

        Color moneyGainCol = moneyGained >= 0 ? Color.green : new Color(1f, 0.4f, 0.4f);
        Color repGainCol = repGained >= 0 ? Color.green : new Color(1f, 0.4f, 0.4f);

        float w = 620f;
        float x = cx - w / 2f;
        float row = 50f;
        float y = cy - 220f;

        GUI.Label(new Rect(x, y, w, 60), "END OF SHIFT", titleStyle);
        GUI.Label(new Rect(x, y + 60, w, 40), "─────────────────────", bodyStyle);

        // Boat stats
        GUI.Label(new Rect(x, y + 110, w, 40), $"Vessels Processed:          {total}", bodyStyle);
        GUI.Label(new Rect(x, y + 155, w, 40), $"Successful Integrations:  {success}", goodStyle);
        GUI.Label(new Rect(x, y + 200, w, 40), $"Failures:                          {fail}", badStyle);
        GUI.Label(new Rect(x, y + 245, w, 40), $"Accuracy:                        {rating}", bodyStyle);

        // Divider
        GUI.Label(new Rect(x, y + 290, w, 40), "─────────────────────", bodyStyle);

        // Money row
        GUIStyle moneyTotalStyle = new GUIStyle(bodyStyle);
        moneyTotalStyle.normal.textColor = new Color(0.2f, 1f, 0.4f);
        GUI.Label(new Rect(x, y + 330, w, 40), $"Money:   $ {money:F2}", moneyTotalStyle);

        GUIStyle moneyGainStyle = new GUIStyle(subStyle);
        moneyGainStyle.normal.textColor = moneyGainCol;
        GUI.Label(new Rect(x, y + 368, w, 30), $"Today:  {Signed(moneyGained)}", moneyGainStyle);

        // Reputation row
        GUIStyle repTotalStyle = new GUIStyle(bodyStyle);
        repTotalStyle.normal.textColor = new Color(0.6f, 0.85f, 1f);
        GUI.Label(new Rect(x, y + 405, w, 40), $"Reputation:   {rep:F0} / 100", repTotalStyle);

        GUIStyle repGainStyle = new GUIStyle(subStyle);
        repGainStyle.normal.textColor = repGainCol;
        GUI.Label(new Rect(x, y + 443, w, 30), $"Today:  {Signed(repGained, "F0")}", repGainStyle);

        // Rank
        GUI.Label(new Rect(x, y + 483, w, 35), rank, rankStyle);
        GUI.Label(new Rect(x, y + 515, w, 25), progress, progressStyle);
        GUI.Label(new Rect(x, y + 550, w, 30), "Press R to play again", hintStyle);
    }
}