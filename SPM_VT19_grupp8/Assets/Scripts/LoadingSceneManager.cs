using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#pragma warning disable 0649
public class LoadingSceneManager : MonoBehaviour
{
    private AsyncOperation loadingOperation;
    private bool startLoading = false;

    public int Scene { get; private set; }
    public static LoadingSceneManager Instance { get; set; } = null;

    [SerializeField]
    private Text loadingText;
    [SerializeField]
    private Text progressText;
    [SerializeField]
    private Slider progressBar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Hide();
    }

    void Update()
    {
        if (startLoading)
        {
            SetProgress(loadingOperation.progress);

            if (loadingOperation.isDone)
            {
                Hide();
            }
        }
    }

    private void SetProgress(float progress)
    {
        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
        progressBar.value = progress;
        progressText.text = (int)(progress * 100) + "%";
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        loadingOperation = null;
        startLoading = false;
    }

    public void Show(AsyncOperation operation)
    {
        gameObject.SetActive(true);
        
        loadingOperation = operation;
        
        SetProgress(0f);
        startLoading = true;
    }

    IEnumerator LoadNewScene()
    {
        loadingOperation = SceneManager.LoadSceneAsync(Scene);
        
        while (!loadingOperation.isDone)
        {
            yield return null;
        }

    }

}