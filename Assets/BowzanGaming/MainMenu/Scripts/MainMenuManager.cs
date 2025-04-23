using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject howToPlayPanel; // Panel con instrucciones
    [SerializeField] private CanvasGroup _fadeCanvas;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private Color _fadeColor;

    [Header("Settings")]
    [SerializeField] private string gameSceneName = "GameScene"; // Nombre de tu escena del juego
    [SerializeField] private float fadeDuration = .5f;

    private void Start() {
        // Configuración inicial
        howToPlayPanel.SetActive(false); // Oculta el panel al inicio

        // Asignar eventos a los botones
        playButton.onClick.AddListener(StartGame);
        howToPlayButton.onClick.AddListener(ToggleHowToPlay);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void StartGame() {
        // Efecto de fade opcional antes de cargar
        StartCoroutine(LoadSceneWithFade(gameSceneName));
    }

    private void ToggleHowToPlay() {
        howToPlayPanel.SetActive(!howToPlayPanel.activeSelf);
    }

    private void ExitGame() {
        // Precaución: No funciona en el Editor, solo en build
        Application.Quit();
    }

    // Corrutina para transición de fade (opcional)
    private IEnumerator LoadSceneWithFade(string sceneName) {
        float elapsedTime = 0f;
        _fadeCanvas.blocksRaycasts = true; // block clicks
        // Fade out
        while (elapsedTime < fadeDuration) {
            _fadeCanvas.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //yield return null;

        SceneManager.LoadScene(sceneName);
    }
}
