using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class BoatRadar : MonoBehaviour
{
    [Header("Radar Settings")]
    public float radarRange = 30f;
    public float sweepSpeed = 90f;

    [Header("World Reference")]
    public Transform radarCenter; // Drag your player/camera here. Leave empty to use this object.

    [Header("UI References")]
    public RectTransform radarScreen;  // The circular radar panel
    public RectTransform sweepLine;    // Optional rotating sweep line

    [Header("Blip Settings")]
    public Color blipColor = new Color(0f, 1f, 0.4f, 1f);
    public float blipSize = 14f;
    public float blipFadeTime = 2.5f;

    private float _sweepAngle = 0f;
    private float _radarRadius = 0f;

    private BoatRandomMovement2D[] _boats;

    private class BlipData
    {
        public RectTransform rt;
        public Image img;
        public float lastSweptTime = -999f;
    }

    private List<BlipData> _blips = new List<BlipData>();
    private bool _ready = false;

    // ── Wait one frame so Canvas has finished its layout before reading rect sizes ──
    IEnumerator Start()
    {
        yield return null; // Wait one frame

        if (radarCenter == null)
            radarCenter = transform;

        if (radarScreen == null)
        {
            Debug.LogError("[BoatRadar] radarScreen is not assigned! Assign your radar panel RectTransform.");
            yield break;
        }

        _radarRadius = radarScreen.rect.width * 0.5f;
        Debug.Log($"[BoatRadar] Radar screen width: {radarScreen.rect.width}, radius: {_radarRadius}");

        if (_radarRadius <= 0f)
        {
            Debug.LogError("[BoatRadar] Radar screen has zero width! Make sure the RectTransform has a non-zero size.");
            yield break;
        }

        RefreshBoats();
        Debug.Log($"[BoatRadar] Found {_boats.Length} boats.");

        if (_boats.Length == 0)
        {
            Debug.LogWarning("[BoatRadar] No boats found! Make sure BoatRandomMovement2D is on your boat GameObjects and they are active.");
        }

        RebuildBlips();
        _ready = true;
    }

    void Update()
    {
        if (!_ready) return;

        if (sweepLine != null)
        {
            _sweepAngle -= sweepSpeed * Time.deltaTime;
            sweepLine.localRotation = Quaternion.Euler(0f, 0f, _sweepAngle);
        }
        if (Keyboard.current.gKey.wasPressedThisFrame && radarRange > 100f)
            radarRange -= 100f;

        if (Keyboard.current.tKey.wasPressedThisFrame)
            radarRange += 100f;

        UpdateBlips();
    }

    void UpdateBlips()
    {
        Vector2 centerWorld = radarCenter.position;

        for (int i = 0; i < _blips.Count; i++)
        {
            if (i >= _boats.Length || _boats[i] == null)
            {
                _blips[i].rt.gameObject.SetActive(false);
                continue;
            }

            Vector2 worldOffset = (Vector2)_boats[i].transform.position - centerWorld;
            float dist = worldOffset.magnitude;

            if (dist > radarRange)
            {
                _blips[i].rt.gameObject.SetActive(false);
                continue;
            }

            _blips[i].rt.gameObject.SetActive(true);

            // Map world position to radar pixel space
            float pixelScale = _radarRadius / radarRange;
            float playerAngle = radarCenter.eulerAngles.z * Mathf.Deg2Rad;
            float cos = Mathf.Cos(-playerAngle);
            float sin = Mathf.Sin(-playerAngle);
            Vector2 rotated = new Vector2(
                worldOffset.x * cos - worldOffset.y * sin,
                worldOffset.x * sin + worldOffset.y * cos
            );
            Vector2 blipPos = rotated * pixelScale;
            _blips[i].rt.anchoredPosition = blipPos;

            // Fade logic
            float alpha = 1f;
            if (sweepLine != null)
            {
                if (SweptOver(blipPos))
                    _blips[i].lastSweptTime = Time.time;

                float age = Time.time - _blips[i].lastSweptTime;
                alpha = Mathf.Max(Mathf.Clamp01(1f - age / blipFadeTime), 0.2f);
            }

            Color c = blipColor;
            c.a = alpha;
            _blips[i].img.color = c;
        }
    }

    bool SweptOver(Vector2 blipPos)
    {
        float blipAngle = Mathf.Atan2(blipPos.y, blipPos.x) * Mathf.Rad2Deg;
        float diff = Mathf.DeltaAngle(_sweepAngle % 360f, blipAngle);
        return Mathf.Abs(diff) < sweepSpeed * Time.deltaTime * 2f;
    }

    void RebuildBlips()
    {
        foreach (var b in _blips)
            if (b.rt != null) Destroy(b.rt.gameObject);
        _blips.Clear();

        for (int i = 0; i < _boats.Length; i++)
        {
            GameObject go = new GameObject($"Blip_{i}", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(radarScreen, false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(blipSize, blipSize);
            rt.anchoredPosition = Vector2.zero;

            Image img = go.GetComponent<Image>();
            img.color = blipColor;
            img.sprite = MakeCircleSprite();
            img.raycastTarget = false;

            _blips.Add(new BlipData { rt = rt, img = img });
            Debug.Log($"[BoatRadar] Created blip {i} for boat '{_boats[i].gameObject.name}'");
        }
    }

    Sprite MakeCircleSprite()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Bilinear;
        Vector2 c = Vector2.one * (size * 0.5f);
        float r = size * 0.5f;

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                float a = Mathf.Clamp01(r - Vector2.Distance(new Vector2(x, y), c));
                tex.SetPixel(x, y, new Color(1, 1, 1, a));
            }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), Vector2.one * 0.5f);
    }

    void RefreshBoats()
    {
        _boats = FindObjectsByType<BoatRandomMovement2D>(FindObjectsSortMode.None);
    }
}