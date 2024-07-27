using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileManager : MonoBehaviour
{
    private Camera mainCamera;
    public GameObject TopLayerContainer;
    public GameObject BottomLayerContainer;
    public GameObject topTilePrefab;
    public GameObject bottomTilePrefab;
    private int gridwidth = 10;
    private int gridheight = 10;
    private int activeTopTiles;
    private List<TileBehaviour> selectedTiles = new List<TileBehaviour>();

    void Start()
    {
        Debug.Log("Start method called");
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }
        GenerateGameBoard();
        activeTopTiles = gridwidth * gridheight;
        Debug.Log("Game board generated");
    }

    void GenerateGameBoard()
    {
        int topLayerIndex = LayerMask.NameToLayer("TopLayer");
        int bottomLayerIndex = LayerMask.NameToLayer("BottomLayer");

        if (topLayerIndex == -1)
        {
            Debug.LogError("TopLayer not found in Layer settings. Please add it.");
            return;
        }

        if (bottomLayerIndex == -1)
        {
            Debug.LogError("BottomLayer not found in Layer settings. Please add it.");
            return;
        }

        for (int x = 0; x < gridwidth; x++)
        {
            for (int y = 0; y < gridheight; y++)
            {
                // Create top layer tile
                GameObject topTileObject = Instantiate(topTilePrefab, TopLayerContainer.transform);
                TileBehaviour topTile = topTileObject.GetComponent<TileBehaviour>();
                topTile.transform.position = new Vector3(x, y, 0);
                topTile.GetComponent<SpriteRenderer>().sortingLayerName = "Top Layer";
                topTile.GetComponent<SpriteRenderer>().material.renderQueue = 3000;
                topTile.gameObject.layer = topLayerIndex;
                // Assign color or type to the top tile (implement your logic here)

                // Create bottom layer tile (initially partially transparent)
                GameObject bottomTileObject = Instantiate(bottomTilePrefab, BottomLayerContainer.transform);
                TileBehaviour bottomTile = bottomTileObject.GetComponent<TileBehaviour>();
                bottomTile.transform.position = new Vector3(x, y, 0);
                bottomTile.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 150 / 255f);
                bottomTile.GetComponent<SpriteRenderer>().sortingLayerName = "Bottom Layer";
                bottomTile.GetComponent<SpriteRenderer>().material.renderQueue = 2000;
                bottomTile.gameObject.layer = bottomLayerIndex;
                bottomTile.isSelectable = false; // Make bottom tiles non-clickable initially
            }
        }
    }

    public void RegisterSelectedTile(TileBehaviour tile)
    {
        if (!selectedTiles.Contains(tile))
        {
            selectedTiles.Add(tile);
        }

        if (selectedTiles.Count == 3)
        {
            if (CheckForMatch())
            {
                // Tiles match, destroy all three
                foreach (var selectedTile in selectedTiles)
                {
                    Destroy(selectedTile.gameObject);
                }
                activeTopTiles -= 3;
                if (activeTopTiles == 0)
                {
                    // All top layer tiles destroyed, reveal bottom layer
                    RevealBottomLayer();
                }
            }
            // Clear the list regardless of match
            selectedTiles.Clear();
        }
    }

    public bool CheckForMatch()
    {
        if (selectedTiles.Count != 3)
        {
            return false;
        }

        // Check if all three tiles have the same color
        return selectedTiles[0].TileColor == selectedTiles[1].TileColor &&
               selectedTiles[1].TileColor == selectedTiles[2].TileColor;
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

    public void RevealBottomLayer()
    {
        foreach (Transform child in BottomLayerContainer.transform)
        {
            TileBehaviour bottomTile = child.GetComponent<TileBehaviour>();
            if (bottomTile != null)
            {
                bottomTile.SetTransparency(1f); // Set alpha to 255
                bottomTile.isSelectable = true; // Make tiles clickable
            }
        }
    }

    public void HandleTileCollision(bool collisionDetected)
    {
        foreach (Transform child in BottomLayerContainer.transform)
        {
            TileBehaviour bottomTile = child.GetComponent<TileBehaviour>();
            if (bottomTile != null)
            {
                if (collisionDetected)
                {
                    bottomTile.SetTransparency(1f);
                    bottomTile.isSelectable = false;
                }
                else
                {
                    bottomTile.SetTransparency(150/255f);
                    bottomTile.isSelectable = true;
                }
            }
        }
    }
}
