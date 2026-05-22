using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int sceneNumber;
    [Range(0f, 1f)][SerializeField] private float hoverAlpha = 0.7f;

    private Image buttonImage;
    private float normalAlpha;

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(LoadScene);

        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
            normalAlpha = buttonImage.color.a;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneNumber);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null)
            SetAlpha(hoverAlpha);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null)
            SetAlpha(normalAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color = buttonImage.color;
        color.a = alpha;
        buttonImage.color = color;
    }
}