using UnityEngine;
using UnityEngine.InputSystem;

public class BoatInspector : MonoBehaviour
{
    public float detectionRange = 5f;
    public Sprite destroyedSprite;

    private BoatData nearbyBoat = null;
    private bool showInfo = false;

    private int successfulIntegrations = 0;
    private int failures = 0;

    // Feedback message shown briefly after a decision
    private string feedbackMessage = "";
    private float feedbackTimer = 0f;
    private bool feedbackCorrect = true;

    void Update()
    {
        nearbyBoat = null;
        float closestDist = detectionRange;

        foreach (BoatData boat in FindObjectsByType<BoatData>(FindObjectsSortMode.None))
        {
            float dist = Vector2.Distance(transform.position, boat.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                nearbyBoat = boat;
            }
        }

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (nearbyBoat != null)
                showInfo = !showInfo;
            else
                showInfo = false;
        }

        // H = let through (valid VISA)
        if (Keyboard.current.hKey.wasPressedThisFrame && nearbyBoat != null)
        {
            if (nearbyBoat.IsVisaValid())
            {
                successfulIntegrations++;
                ShowFeedback("CORRECT - Valid VISA, vessel cleared", true);
            }
            else
            {
                failures++;
                ShowFeedback("FAILURE - Invalid VISA, should have destroyed", false);
            }

            showInfo = false;
            nearbyBoat = null;
        }

        // K = destroy (invalid VISA)
        if (Keyboard.current.kKey.wasPressedThisFrame && nearbyBoat != null)
        {
            if (!nearbyBoat.IsVisaValid())
            {
                successfulIntegrations++;
                ShowFeedback("CORRECT - Invalid VISA, vessel destroyed", true);
            }
            else
            {
                failures++;
                ShowFeedback("FAILURE - Valid VISA, should have cleared", false);
            }

            SpriteRenderer sr = nearbyBoat.GetComponent<SpriteRenderer>();
            if (sr != null && destroyedSprite != null)
                sr.sprite = destroyedSprite;

            nearbyBoat.isDead = true;
            Destroy(nearbyBoat.gameObject, 30f);
            showInfo = false;
            nearbyBoat = null;
        }

        if (nearbyBoat == null)
            showInfo = false;

        if (feedbackTimer > 0f)
            feedbackTimer -= Time.deltaTime;
    }

    void ShowFeedback(string msg, bool correct)
    {
        feedbackMessage = msg;
        feedbackCorrect = correct;
        feedbackTimer = 2.5f;
    }

    void OnGUI()
    {
        // Score counter top right
        GUIStyle scoreStyle = new GUIStyle();
        scoreStyle.fontSize = 20;
        scoreStyle.normal.textColor = Color.white;
        scoreStyle.alignment = TextAnchor.UpperRight;

        GUI.Label(new Rect(Screen.width - 320, 20, 300, 30), $"Successful Integrations: {successfulIntegrations}", scoreStyle);

        scoreStyle.normal.textColor = new Color(1f, 0.4f, 0.4f);
        GUI.Label(new Rect(Screen.width - 320, 50, 300, 30), $"Failures: {failures}", scoreStyle);

        // Prompt when near a boat
        if (nearbyBoat != null && !showInfo)
        {
            GUIStyle promptStyle = new GUIStyle();
            promptStyle.fontSize = 18;
            promptStyle.normal.textColor = Color.yellow;
            promptStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height - 80, 400, 30), "I - Inspect  |  H - Clear  |  K - Destroy", promptStyle);
        }

        // Feedback flash
        if (feedbackTimer > 0f)
        {
            GUIStyle fbStyle = new GUIStyle();
            fbStyle.fontSize = 22;
            fbStyle.fontStyle = FontStyle.Bold;
            fbStyle.normal.textColor = feedbackCorrect ? Color.green : new Color(1f, 0.3f, 0.3f);
            fbStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width / 2 - 250, Screen.height - 120, 500, 35), feedbackMessage, fbStyle);
        }

        // Boat info panel
        if (showInfo && nearbyBoat != null)
        {
            GUI.color = new Color(0f, 0f, 0f, 0.75f);
            GUI.DrawTexture(new Rect(0, Screen.height - 150, Screen.width, 150), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.padding = new RectOffset(20, 0, 0, 0);

            int y = Screen.height - 140;
            GUI.Label(new Rect(20, y, Screen.width, 30), $"Vessel Name:  {nearbyBoat.vesselName}", style);
            GUI.Label(new Rect(20, y + 28, Screen.width, 30), $"Type:              {nearbyBoat.vesselType}", style);
            GUI.Label(new Rect(20, y + 56, Screen.width, 30), $"Cargo:            {nearbyBoat.cargo}", style);
            GUI.Label(new Rect(20, y + 84, Screen.width, 30), $"POB:              {nearbyBoat.pob}", style);
            GUI.Label(new Rect(20, y + 112, Screen.width, 30), $"VISA No:       {nearbyBoat.visaNumber}", style);
        }
    }
}