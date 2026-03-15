using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]


public class Room : MonoBehaviour
{
    public List<Room> neighbors;
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

                ActivateNeighbors();
            }
        }
    }

    void ActivateNeighbors()
    {
        Room[] allRooms = FindObjectsByType<Room>(FindObjectsInactive.Include,FindObjectsSortMode.None);

        HashSet<Room> roomsToKeepActive = new HashSet<Room>
        {
            this
        };

        foreach(Room neighbor in neighbors)
        {
            if (neighbor != null) roomsToKeepActive.Add(neighbor);
        }

        foreach(Room room in allRooms)
        {
            bool shouldBeActive = roomsToKeepActive.Contains(room);
            room.gameObject.SetActive(shouldBeActive);
        }
    }
}
