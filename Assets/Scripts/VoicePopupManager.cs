using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class VoicePopupManager : MonoBehaviour
{
    public static VoicePopupManager Instance;

    [Header("Popup Settings")]
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Transform popupParent;
    [SerializeField] private float popupDuration = 1.2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowPopup(string text)
    {
        if (popupPrefab == null)
        {
            Debug.LogWarning("[VoicePopupManager] No popup prefab assigned!");
            return;
        }

        GameObject popup = Instantiate(popupPrefab, popupParent);
        TMP_Text tmp = popup.GetComponentInChildren<TMP_Text>();

        if (tmp != null)
        {
            tmp.text = text.ToUpper();
            tmp.color = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.8f, 1f);
        }

        StartCoroutine(FadeAndDestroy(popup, popupDuration));
    }

    private IEnumerator FadeAndDestroy(GameObject popup, float duration)
    {
        CanvasGroup group = popup.AddComponent<CanvasGroup>();
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, t / duration);
            popup.transform.Translate(Vector3.up * Time.deltaTime * 40f); // move upward slightly
            yield return null;
        }

        Destroy(popup);
    }
}
