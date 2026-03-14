using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif
public enum RoomSide
{
    Left,
    Right,
    Top,
    Bottom
}
public class RoomShifter : MonoBehaviour
{
    public RoomSide roomSide;

    #if UNITY_EDITOR
    [SerializeField] private SceneAsset targetScene;
    #endif
    [HideInInspector]public string targetScenePath;

    private void OnValidate()
    {
        #if UNITY_EDITOR
        if(targetScene != null)
        {
            targetScenePath = AssetDatabase.GetAssetPath(targetScene);
        }
        #endif
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            print(targetScenePath);
        }
    }
}
