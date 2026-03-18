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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    void OnGUI()
    {
        if (!isShowing) return;
        // Dim background
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
        int success = boatInspector != null ? boatInspector.successfulIntegrations : 0;
        int fail = boatInspector != null ? boatInspector.failures : 0;
        int total = success;
        string rating = total == 0 ? "N/A" : $"{Mathf.RoundToInt((float)success / total * 100f)}%";
        string rank = GetRank(total);
        string progress = GetNextRankProgress(total);
        GUI.Label(new Rect(cx - 300, cy - 160, 600, 60), "END OF SHIFT", titleStyle);
        GUI.Label(new Rect(cx - 300, cy - 80, 600, 40), "─────────────────────", bodyStyle);
        GUI.Label(new Rect(cx - 300, cy - 30, 600, 40), $"Vessels Processed:          {total + fail}", bodyStyle);
        GUI.Label(new Rect(cx - 300, cy + 20, 600, 40), $"Successful Integrations:  {success}", goodStyle);
        GUI.Label(new Rect(cx - 300, cy + 70, 600, 40), $"Failures:                          {fail}", badStyle);
        GUI.Label(new Rect(cx - 300, cy + 120, 600, 40), $"Accuracy:                        {rating}", bodyStyle);
        GUI.Label(new Rect(cx - 300, cy + 170, 600, 35), rank, rankStyle);
        GUI.Label(new Rect(cx - 300, cy + 205, 600, 25), progress, progressStyle);
        GUI.Label(new Rect(cx - 300, cy + 240, 600, 30), "Press R to play again", hintStyle);
    }
}