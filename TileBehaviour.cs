using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public Color TileColor; // The color of the tile
    private TileManager tileManager;

    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        GetComponent<SpriteRenderer>().color = TileColor; // Set the color of the tile
    }

    void OnMouseDown()
    {
        tileManager.RegisterSelectedTile(this);
    }
}