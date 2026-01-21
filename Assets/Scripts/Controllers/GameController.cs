using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(InputController))]
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    [HideInInspector] public InputController Input;
    public TMP_Text debugText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Input = GetComponent<InputController>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
}
