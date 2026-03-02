using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// GameController is a singleton that is accessable from anywhere. 
// It contains multiple subcontrollers that relate to game state and Unity engine systems like the input system.

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(InputController))]
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    [HideInInspector] public InputController Input;
    [HideInInspector] public PathfindingGridController PathfindingGrid;

    public TMP_Text debugText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            Input = GetComponent<InputController>();
            PathfindingGrid = GetComponent<PathfindingGridController>();
            PathfindingGrid.Build();
            
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (debugText != null) debugText.text = "-";
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PathfindingGrid.RebindAndBuild();
    }
}
