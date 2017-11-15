using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Class:          TileTypes
 *  Description:    A stand alone class used to represent the types of tiles
 *                  that can possibly be spawned as part of the physical chess
 *                  board and their properties
 */
[System.Serializable]
public class TileType
{
    public enum Tiletype {GRASS, BRIDGE_EDGE_A, BRIDGE_EDGE_B, BRIDGE_BODY, FOLIAGE};
    public enum Mat {LIGHT, SHADED, GLOW};

    public Tiletype type;               /* The type of this tile */

    public GameObject[] tilePrefab;     /* Holds both the Light and Shaded prefabs of the tile type */
    public Material[] tileMaterials;    /* Holds the Light (indx:0), Shaded (indx:1), and Glow (indx:2) 
                                           materials of the tile */
}
