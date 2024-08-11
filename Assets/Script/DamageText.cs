using System.Collections;
using Photon.Pun;
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
        // Destroy(gameObject, destroyTime);
        StartCoroutine(DestroyAfter(gameObject,destroyTime));
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(damageText.color.a, 0, Time.deltaTime * fadeOutSpeed));
    }

    public IEnumerator DestroyAfter(GameObject _gameObject, float _delay)// delay 시간 만큼 유지후 게임 오브젝트 파괴
    {  
        yield return new WaitForSeconds(_delay);
        if(_gameObject !=null)
        {
            PhotonNetwork.Destroy(_gameObject);
        }
    }

    
}
