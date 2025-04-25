using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena
using UnityEngine.InputSystem;
using BowzanGaming.FinalCharacterController;
using System.Collections; // O usa UnityEngine.Input si no usas el nuevo Input System

public class GameManager : MonoBehaviour {
    // --- Singleton Pattern ---
    // Permite acceder al GameManager desde cualquier otro script fácilmente (GameManager.Instance)
    private static GameManager _instance;
    public static GameManager Instance {
        get {
            if (_instance == null) {
                // Busca una instancia existente en la escena
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null) {
                    // Si no existe, crea un nuevo GameObject y le añade el GameManager
                    // Esto es útil si olvidas ponerlo en la escena, pero es mejor ponerlo manualmente.
                    GameObject singletonObject = new GameObject("GameManager_AutoCreated");
                    _instance = singletonObject.AddComponent<GameManager>();
                    Debug.LogWarning("GameManager no encontrado en la escena, se ha creado uno automáticamente.");
                }
            }
            return _instance;
        }
    }

    // --- UI References ---
    [Header("UI Panels")]
    [Tooltip("Arrastra aquí el Panel de la UI que muestra la introducción")]
    public GameObject introPanel; // Arrastra el IntroPanel aquí desde el Inspector

    [Tooltip("Arrastra aquí el Panel de la UI que muestra la ayuda")]
    public GameObject helpPanel;  // Arrastra el HelpPanel aquí desde el Inspector

    // --- Game State ---
    private bool isGamePaused = false; // Para saber si el juego está pausado por la UI

    // --- Settings ---
    [Header("Game Settings")]
    [Tooltip("Nombre exacto de la escena del Menú Principal")]
    public string mainMenuSceneName = "MainMenu"; // ¡¡IMPORTANTE: Cambia esto al nombre real de tu escena de menú!!
    public PlayerController PlayerController;

    [Tooltip("Tecla para mostrar/ocultar el menú de ayuda")]
    public KeyCode helpKey = KeyCode.H; // Puedes cambiarla si quieres (ej: KeyCode.F1)

    // --- Unity Methods ---

    void Awake() {
        // Implementación del Singleton: Asegura que solo haya una instancia
        if (_instance != null && _instance != this) {
            Debug.LogWarning("Ya existe una instancia de GameManager. Destruyendo esta duplicada.");
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        Debug.Log("Current Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
        // Opcional: No destruir el GameManager al cargar otras escenas (si fuera necesario)
        // DontDestroyOnLoad(this.gameObject);
        // En este caso, como volvemos al menú principal, probablemente no sea necesario.
    }

    void Start() {
        // Asegurarse de que los paneles están ocultos al inicio (por si acaso se quedaron activos en el editor)
        if (introPanel != null) introPanel.SetActive(false);
        if (helpPanel != null) helpPanel.SetActive(false);

        // Mostrar la introducción al empezar la partida
        ShowIntro();
    }

    void Update() {
        // Comprobar si se pulsa la tecla de ayuda
        // Usamos GetKeyDown para que solo se active una vez al pulsar
        if (Input.GetKeyDown(helpKey)) // O usa la lógica del nuevo Input System si lo tienes configurado
        {
            ToggleHelpMenu();
        }

        // Opcional: Permitir cerrar los menús con Escape
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (helpPanel != null && helpPanel.activeSelf) {
                HideHelpMenu();
            }
            // Podrías añadir aquí lógica para un menú de pausa general si quieres
            // else if (introPanel != null && introPanel.activeSelf) {
            //     HideIntro(); // Quizás no quieras permitir cerrar la intro con Escape
            // }
        }
    }

    // --- UI Control Methods ---

    public void ShowIntro() {
        if (introPanel != null) {
            introPanel.SetActive(true);
            PauseGame(); // Pausar el juego mientras se lee la intro
        } else {
            Debug.LogError("Intro Panel no está asignado en el GameManager!");
        }
    }

    // Esta función será llamada por el botón "Entendido" del IntroPanel
    public void HideIntro() {
        if (introPanel != null) {
            introPanel.SetActive(false);
            ResumeGame(); // Reanudar el juego
        }
    }

    public void ToggleHelpMenu() {
        if (helpPanel != null) {
            if (!introPanel.activeSelf) {
                bool isHelpActive = helpPanel.activeSelf;
                helpPanel.SetActive(!isHelpActive);

                if (!isHelpActive) // Si se acaba de activar
                {
                    PauseGame(); // Pausar el juego
                } else // Si se acaba de desactivar
                  {
                    ResumeGame(); // Reanudar el juego
                }

            }
           
        } else {
            Debug.LogError("Help Panel no está asignado en el GameManager!");
        }
    }

    // Esta función será llamada por el botón "Cerrar" del HelpPanel
    // También la usamos internamente si cerramos con Escape
    public void HideHelpMenu() {
        if (helpPanel != null && helpPanel.activeSelf) {
            helpPanel.SetActive(false);
            ResumeGame();
        }
    }


    // --- Game State Control ---

    void PauseGame() {
        Time.timeScale = 0f; // Congela el tiempo del juego
        isGamePaused = true;
        // Aquí podrías añadir lógica para desactivar controles del jugador si es necesario
        PlayerController.lookSenseH = 0.0f;
        PlayerController.lookSenseV = 0.0f;
        //Cursor.lockState = CursorLockMode.None; // Mostrar cursor
        // Cursor.visible = true;
    }

    void ResumeGame() {
        // Solo reanudar si no hay OTRO panel abierto (por si acaso)
        // En este caso simple, asumimos que si ocultamos uno, queremos reanudar.
        // Si tuvieras más menús (pausa, inventario), necesitarías una lógica más robusta.
        if ((introPanel == null || !introPanel.activeSelf) && (helpPanel == null || !helpPanel.activeSelf)) {
            //PlayerController.enabled = true;
            PlayerController.lookSenseH = 0.1f;
            PlayerController.lookSenseV = 0.1f;
            Time.timeScale = 1f; // Restaura el tiempo normal del juego
            isGamePaused = false;
            
            // Ocultar cursor si tu juego lo requiere (FPS, TPS)
            //Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }
    }

    // --- Win Condition ---

    // Esta función debe ser llamada por el script del Golden Spirithar cuando es derrotado
    public IEnumerator GoldenSpiritharDefeated() {
        float elapsedTime = 0f;
        Debug.Log("¡Golden Spirithar Derrotado! Volviendo al Menú Principal...");
        // Asegurarse de que el tiempo vuelve a la normalidad antes de cambiar de escena
        Time.timeScale = 1f;
        
        // Fade out
        while (elapsedTime < 0.5f) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Cargar la escena del menú principal
        SceneManager.LoadScene(mainMenuSceneName);
    }

    
}