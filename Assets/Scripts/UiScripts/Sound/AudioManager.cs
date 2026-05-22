using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("UI Sounds")]
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    public AudioClip panelOpenSound;
    public AudioClip panelCloseSound;

    [Header("Game Sounds")]
    public AudioClip workStartSound;
    public AudioClip workCompleteSound;
    public AudioClip peBoxCollectSound;

    [Header("Settings")]
    [Range(0f, 1f)] public float uiVolume = 0.7f;
    [Range(0f, 1f)] public float gameVolume = 0.8f;

    private AudioSource audioSource;

    void Awake()
    {
        // Создаём синглтон
        if (Instance == null)
        {

            // Добавляем AudioSource если его нет
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.playOnAwake = false;

            Debug.Log("AudioManager инициализирован");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayUISound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, uiVolume);
        }
    }

    public void PlayGameSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, gameVolume);
        }
    }

    // Удобные методы для быстрого вызова
    public void PlayButtonClick()
    {
        PlayUISound(buttonClickSound);
    }
    public void PlayButtonHover()
    {
        PlayUISound(buttonHoverSound);
    }

    public void PlayPanelOpen()
    {
        PlayUISound(panelOpenSound);
    }

    public void PlayPanelClose()
    {
        PlayUISound(panelCloseSound);
    }

    public void PlayWorkStart()
    {
        PlayGameSound(workStartSound);
    }

    public void PlayWorkComplete()
    {
        PlayGameSound(workCompleteSound);
    }

    public void PlayPeBoxCollect()
    {
        PlayGameSound(peBoxCollectSound);
    }
}