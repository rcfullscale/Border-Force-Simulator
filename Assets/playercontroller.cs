using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

public class AircraftController : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float minSpeed = 0f;
    public float acceleration = 2f;
    public float turnSpeed = 60f;
    private float currentSpeed = 0f;
    private float heading = 0f;
    private float stallTimer = 0f;
    private DaySummary daySummary;

    private bool showCartelOffer = true;
    private int cartelMoney;
    public int cartelVisaNumber;

    void Start()
    {
        daySummary = FindObjectOfType<DaySummary>();
        cartelMoney = Random.Range(100, 1501);
        cartelVisaNumber = Random.Range(1000, 10000);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // Dismiss cartel offer with any key
        if (showCartelOffer && kb.anyKey.wasPressedThisFrame)
        {
            showCartelOffer = false;
            return;
        }

        // Throttle
        if (kb.wKey.isPressed)
            currentSpeed += acceleration * Time.deltaTime;
        if (kb.sKey.isPressed)
            currentSpeed -= acceleration * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);

        // Turning
        if (currentSpeed > 0f)
        {
            if (kb.aKey.isPressed)
                heading -= turnSpeed * Time.deltaTime;
            if (kb.dKey.isPressed)
                heading += turnSpeed * Time.deltaTime;
        }

        if (heading < 0f) heading += 360f;
        if (heading >= 360f) heading -= 360f;

        transform.rotation = Quaternion.Euler(0f, 0f, -heading);
        transform.position += transform.up * currentSpeed * Time.deltaTime;
    }

    void OnGUI()
    {
        if (showCartelOffer)
        {
            // Dark overlay
            GUI.color = new Color(0f, 0f, 0f, 0.75f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.fontSize = 22;
            boxStyle.normal.textColor = Color.white;
            boxStyle.alignment = TextAnchor.MiddleCenter;

            float boxW = 520f;
            float boxH = 160f;
            float bx = (Screen.width - boxW) / 2f;
            float by = (Screen.height - boxH) / 2f;

            GUI.Box(new Rect(bx, by, boxW, boxH), "");

            GUIStyle titleStyle = new GUIStyle();
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = new Color(1f, 0.4f, 0.1f);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(bx, by + 20, boxW, 35), "⚠ CARTEL OFFER", titleStyle);

            GUIStyle msgStyle = new GUIStyle();
            msgStyle.fontSize = 19;
            msgStyle.normal.textColor = Color.white;
            msgStyle.alignment = TextAnchor.MiddleCenter;
            msgStyle.wordWrap = true;
            GUI.Label(new Rect(bx, by + 65, boxW, 50),
                $"A cartel has offered you ${cartelMoney} if you let Visa Number {cartelVisaNumber} through.", msgStyle);

            GUIStyle hintStyle = new GUIStyle();
            hintStyle.fontSize = 14;
            hintStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
            hintStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(bx, by + 125, boxW, 25), "Press any key to continue", hintStyle);

            return;
        }

        // Normal HUD
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;
        int displaySpeed = Mathf.RoundToInt(currentSpeed * 30f);
        GUI.Label(new Rect(20, 20, 300, 40), $"SPD:  {displaySpeed} kts", style);
        GUI.Label(new Rect(20, 55, 300, 40), $"HDG: {Mathf.RoundToInt(heading)}°", style);

        if (displaySpeed < 70)
        {
            stallTimer += Time.deltaTime;
            if (stallTimer >= 10f) { daySummary.ShowSummary("crashed"); }
            GUIStyle stallStyle = new GUIStyle();
            stallStyle.fontSize = 50;
            stallStyle.fontStyle = FontStyle.Bold;
            stallStyle.normal.textColor = Color.red;
            stallStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width / 2 - 150, 20, 300, 80), "STALL", stallStyle);
        }
        else
        {
            stallTimer = 0f;
        }
    }
}