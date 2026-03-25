using UnityEngine;
using UnityEngine.InputSystem;

public class GameClock : MonoBehaviour
{
    private const float GameHoursPerRealSecond = 12f / 300f;

    private float currentGameHour = 5f;
    private bool isDaytime = true;
    private int day = 1;

    private DaySummary daySummary;

    void Start()
    {
        daySummary = FindFirstObjectByType<DaySummary>();
        if (daySummary == null)
            Debug.LogWarning("GameClock: No DaySummary script found in scene!");
    }

    void Update()
    {
        if (!isDaytime) return;

        currentGameHour += GameHoursPerRealSecond * Time.deltaTime;

        if (currentGameHour >= 17f || Keyboard.current.nKey.wasPressedThisFrame)
        {
            currentGameHour = Mathf.Min(currentGameHour, 17f);
            isDaytime = false;
            daySummary?.ShowSummary();
        }
    }

    void OnGUI()
    {
        if (!isDaytime) return;

        int hours = Mathf.FloorToInt(currentGameHour);
        int rawMinutes = Mathf.FloorToInt((currentGameHour - hours) * 60f);
        int minutes = (rawMinutes / 10) * 10;
        string ampm = hours >= 12 ? "PM" : "AM";
        int display = hours > 12 ? hours - 12 : hours;
        if (display == 0) display = 12;

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 130, 200, 30), $"Time: {display}:{minutes:D2} {ampm}", style);
        GUI.Label(new Rect(20, 160, 200, 30), $"Day: {day}", style);
    }
}