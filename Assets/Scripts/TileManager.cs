using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileManager : MonoBehaviour
{
    private TileBehaviour selectedTile;
    private Camera mainCamera;

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
                // ADD scoring
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
}
