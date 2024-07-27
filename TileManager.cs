using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileManager : MonoBehaviour
{
    private TileBehaviour selectedTile;
    private Camera mainCamera;
    public GameObject TopLayerContainer;
    public GameObject BottomLayerContainer;
    public GameObject topTilePrefab;
    public GameObject bottomTilePrefab;
    public float overlapCircleRadius = 1f;
    private int gridwidth = 10;
    private int gridheight = 10;
    void Start()
    {
        mainCamera = Camera.main;
        GenerateGameBoard();
    }
    void GenerateGameBoard()
    {
        for (int x = 0; x < gridwidth; x++)
        {
            for (int y = 0; y < gridheight; y++)
            {
                // Create top layer tile
            GameObject topTileObject = Instantiate(topTilePrefab, TopLayerContainer.transform);
            TileBehaviour topTile = topTileObject.GetComponent<TileBehaviour>();
            topTile.transform.position = new Vector3(x, y, 0);
            // Assign color or type to the top tile (implement your logic here)

            // Create bottom layer tile (initially grayed out)
            GameObject bottomTileObject = Instantiate(bottomTilePrefab, BottomLayerContainer.transform);
            TileBehaviour bottomTile = bottomTileObject.GetComponent<TileBehaviour>();
            bottomTile.transform.position = new Vector3(x, y, 0);
            bottomTile.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.1f); // Adjust alpha for initial grayed-out effect
            // Assign color or type to the bottom tile (implement your logic here)
            }
        }
    }

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void RegisterSelectedTile(TileBehaviour tile)
    {
        if (selectedTile == null)
        {
            selectedTile = tile;
        }
        else
        {
            if (CheckForMatch(tile))
            {
                // Tiles match, destroy both
                Destroy(selectedTile.gameObject);
                Destroy(tile.gameObject);
                selectedTile = null;
            }
            else
            {
                // Tiles do not match, deselect the previously selected tile
                selectedTile = null;
            }
        }
    }

    public bool CheckForMatch(TileBehaviour tile)
    {
        return selectedTile != null && selectedTile != tile && selectedTile.TileColor == tile.TileColor;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 clickPosition = context.ReadValue<Vector2>();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(clickPosition.x, clickPosition.y, mainCamera.nearClipPlane));

            // Use a raycast to detect the clicked tile
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            if (hit.collider != null)
            {
                TileBehaviour tile = hit.collider.GetComponent<TileBehaviour>();
                if (tile != null)
                {
                    RegisterSelectedTile(tile);
                }
            }
        }
    }
    public void RevealBottomLayerTile(Vector3 position)
    {
        // Use Physics2D.OverlapCircle to find the bottom layer tile at the given position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 1f);
        foreach (Collider2D collider in colliders)
        {
            TileBehaviour tile = collider.GetComponent<TileBehaviour>();
            if (tile != null && tile.transform.parent == BottomLayerContainer.transform)
            {
                tile.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

}
