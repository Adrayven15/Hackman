using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public Color TileColor; // The color of the tile
    private TileManager tileManager;
    public bool isSelectable = true; // Flag to indicate if the tile is selectable

    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        GetComponent<SpriteRenderer>().color = TileColor; // Set the color of the tile
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.layer == LayerMask.NameToLayer("BottomLayer") && other.gameObject.layer == LayerMask.NameToLayer("TopLayer"))
        {
            SetTransparency(150 / 255f); // Set alpha to 150
            isSelectable = false;
            tileManager.HandleTileCollision(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (gameObject.layer == LayerMask.NameToLayer("BottomLayer") && other.gameObject.layer == LayerMask.NameToLayer("TopLayer"))
        {
            SetTransparency(1f); // Set alpha to 255
            isSelectable = true;
            tileManager.HandleTileCollision(false);
        }
    }

    void OnMouseDown()
    {
        if (isSelectable)
        {
            tileManager.RegisterSelectedTile(this);
        }
    }

    public void SetTransparency(float alpha)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}
