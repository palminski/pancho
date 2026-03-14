using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct Neighbors
{
    public Room left;
    public Room right;
    public Room above;
    public Room below;
}

public class Room : MonoBehaviour
{
    public Neighbors neighbors;
    [SerializeField] private Tilemap tilemap;

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
        print("here");
        if (collision.gameObject.GetComponent<Player>())
        {
            CameraControls cameraControls = Camera.main.GetComponent<CameraControls>();
            if (tilemap && cameraControls != null)
            {
                tilemap.CompressBounds();
                GameController.Instance.RebindPathfindingGrid(tilemap);
                cameraControls.SetTilemap(tilemap);
            }

        }
    }
}
