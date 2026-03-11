using UnityEngine;
using UnityEngine.InputSystem;

public class AircraftController : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float minSpeed = 0f;
    public float acceleration = 2f;
    public float turnSpeed = 60f;

    private float currentSpeed = 0f;
    private float heading = 0f;

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

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

        // Movement
        transform.position += transform.up * currentSpeed * Time.deltaTime;
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;

        int displaySpeed = Mathf.RoundToInt(currentSpeed * 30f);
        GUI.Label(new Rect(20, 20, 300, 40), $"SPD:  {displaySpeed} kts", style);
        GUI.Label(new Rect(20, 55, 300, 40), $"HDG: {Mathf.RoundToInt(heading)}°", style);
    }
}