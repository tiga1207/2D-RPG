using TMPro;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float fadeOutSpeed = 2.0f;
    public float destroyTime = 2.0f;

    private TextMeshProUGUI damageText;
    private Color originalColor;

    void Awake()
    {
        damageText = GetComponentInChildren<TextMeshProUGUI>();
        originalColor = damageText.color;

        // Canvas 설정
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingLayerName = "UI"; // 원하는 Sorting Layer
            canvas.sortingOrder = 1000; // Order in Layer 설정
        }
    }

    public void Initialize(float damageAmount)
    {
        damageText.text = damageAmount.ToString();
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(damageText.color.a, 0, Time.deltaTime * fadeOutSpeed));
    }
}
