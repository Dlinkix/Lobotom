using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwap : MonoBehaviour
{
    [SerializeField] int number;

    [SerializeField] public Button button;
    void Start()
    {
        button.onClick.AddListener(loadScene);
    }

    public void loadScene()
    {
        SceneManager.LoadScene(number);
    }

}
