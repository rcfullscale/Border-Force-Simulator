using UnityEngine;
using UnityEngine.InputSystem;

public class BoatInspector : MonoBehaviour
{
    public float detectionRange = 5f;   // How close you need to be

    private BoatData nearbyBoat = null;
    private bool showInfo = false;

    void Update()
    {
        // Find the closest boat within range
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

        // Toggle info display on I press
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (nearbyBoat != null)
                showInfo = !showInfo;
            else
                showInfo = false;
        }

        // Hide info if you move out of range
        if (nearbyBoat == null)
            showInfo = false;
    }

    void OnGUI()
    {
        // Prompt to press I when in range
        if (nearbyBoat != null && !showInfo)
        {
            GUIStyle promptStyle = new GUIStyle();
            promptStyle.fontSize = 18;
            promptStyle.normal.textColor = Color.yellow;
            promptStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height - 80, 300, 30), "Press I to inspect vessel", promptStyle);
        }

        // Show boat info panel at bottom of screen
        if (showInfo && nearbyBoat != null)
        {
            // Background box
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