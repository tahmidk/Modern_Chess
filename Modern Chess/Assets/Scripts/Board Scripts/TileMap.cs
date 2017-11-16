using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    /* Set up an instance of this TileMap so it can be accessed freely by other classes */
    public static TileMap Instance { get; set; }

    #region Public Macros
    public enum Obstacle {TREE}             /* Enum to help indexing mapPrefabs array */
    const int mapSize = BoardManager.BOARD_SIZE;

    public GameObject[] obstaclePrefabs;     /* Holds the prefabs of the possible map obstacles */
    public GameObject emptyBoardObject;     /* Holds the prefab for the empty board object */

    public Material select_glow;            /* Holds the material used to make tiles pointed to by mouse glow */
    public Material attack_glow;            /* Holds the material used to make tiles glow if it has an enemy on it */
    public TileType[] tileTypes;            /* Maintains a list of possible tile types */
    #endregion

    #region Instance Fields
    private TileType.Tiletype[,] tiles;      /* Keeps tile type info of position (x, y) on the board */
    private GameObject[,] allTiles;          /* Keeps each physical tile game object at position (x, y) */

    public int highlightedTile_X = -1;              /* The x coordinate of the tile pointed to by the mouse */
    public int highlightedTile_Y = -1;              /* The y coordinate of the tile pointed to by the mouse */
    private Material highlightedTile_OriginalMat;    /* Temporary storage for highlighted tile's original mat */

    /* Maintains a list of all the (x, y) coordinates of the tiles highlighted as possible moves for a 
     * selected piece
     * Linked List because we will only be inserting and deleting, O(1) ops */
    private LinkedList<int[]> highlightedTileMove;
    #endregion

    /*----------------------------------------[START]----------------------------------------------*/
    private void Start()
    {
        highlightedTileMove = new LinkedList<int[]>();
        Instance = this;
    }

    #region Generation Functions
    /** Function:   GenerateBoard()
     *  Argument:   None
     *  Output:     Initializes and populates the tiles array and spawns the visual prefabs of the tiles
     *              accordingly
     */
    public void GenerateBoard()
    {
        // Initialize tiles
        tiles = new TileType.Tiletype[mapSize, mapSize];
        allTiles = new GameObject[mapSize, mapSize];

        // Populate and Spawn grass tiles
        for (int i = 0; i < mapSize; i++)
            for (int j = 0; j < mapSize; j++)
            {
                // Create a ditch spanning rows 6 - 9
                if (j >= 6 && j <= 9)
                    continue;

                tiles[i, j] = TileType.Tiletype.GRASS;
                SpawnTile(i, j, Quaternion.identity);
            }

        // Populate and Spawn a bridge
        SpawnBridge(6, 6, 4, 4);

        // Spawn the stage obstacles
        SpawnObstacle(1, 1, Obstacle.TREE);
        SpawnObstacle(4, 4, Obstacle.TREE);
        SpawnObstacle(13, 3, Obstacle.TREE);
        SpawnObstacle(4, 12, Obstacle.TREE);
        SpawnObstacle(14, 13, Obstacle.TREE);
        SpawnObstacle(12, 10, Obstacle.TREE);
    }

    /** Function:   SpawnBridge(int, int, int, int)
     *  Argument:   int x - the starting x position of the bridge
     *              int y - the starting y position of the bridge
     *              int width - the width of the bridge (x - directional)
     *              int length - the length of the bridge (y - directional)
     *  Output:     Instantiates an entire bridge structure using the BRIDGE_EDGE and BRIDGE_BODY prefabs
     */
    private void SpawnBridge(int x, int y, int width, int length)
    {
        for (int x_offset = 0; x_offset < width; x_offset++)
            for(int y_offset = 0; y_offset < length; y_offset++)
            {
                int newX = x + x_offset;
                int newY = y + y_offset;

                /* Use BRIDGE_EDGE prefab over the front and back edges of bridge */
                if (newY == y)
                    tiles[newX, newY] = TileType.Tiletype.BRIDGE_EDGE_A;
                else if (newY == y + length - 1)
                    tiles[newX, newY] = TileType.Tiletype.BRIDGE_EDGE_B;
                else
                    tiles[newX, newY] = TileType.Tiletype.BRIDGE_BODY;

                SpawnTile(newX, newY, Quaternion.identity);
            }

    }

    /** Function:   SpawnTile(int, int)
     *  Argument:   int x - the x position of the tile to spawn ( expected range:[0, BOARD_SIZE] )
     *              int y - the y position of the tile to spawn ( expected range:[0, BOARD_SIZE] )
     *  Output:     Instantiates a tile at board position (x, y) based on the Tiletype at (x, y) in
     *              the tiles array
     *  [PRECONDITION] tiles must be initialized and properly populated
     */
    private void SpawnTile(int x, int y, Quaternion rot)
    {
        // Fetch the tile prefab:
        // We fetch the light tile prefab if (x + y) is even. Otherwise, fetch the dark version
        TileType tt = tileTypes[(int) tiles[x, y]];
        GameObject objToSpawn = ((x + y) % 2 == 0) ? tt.tilePrefab[0] : tt.tilePrefab[1];

        Vector3 pos = new Vector3(x, 0, y);     /* Physical location at which to spawn objToSpawn */

        GameObject tile = Instantiate(objToSpawn, pos, rot);
        tile.transform.SetParent(transform);
        allTiles[x, y] = tile;

        GameObject boardItem = Instantiate(emptyBoardObject, pos, Quaternion.identity);
        boardItem.transform.SetParent(transform);
        BoardManager.Instance.Board[x, y] = boardItem.GetComponent<EmptyBoardObject>();
    }

    /** Function:   SpawnObstacle(int, int)
     *  Argument:   int x - the x coordinate of the board position on which to spawn this obstacle
     *              int y - the y coordinate of the board position on which to spawn this obstacle
     *              MapItem obs - the index of the obstacle to spawn
     */
    private void SpawnObstacle(int x, int y, Obstacle obs)
    {
        // Fetch the obstacle prefab
        GameObject obstacle = obstaclePrefabs[(int) obs];

        // Spawn it and add it to Board
        Vector3 pos = new Vector3(x, 0, y);
        GameObject go = Instantiate(obstacle, pos, Quaternion.identity);
        go.transform.SetParent(transform);
        BoardManager.Instance.Board[x, y] = go.GetComponent<BoardObjects>();
    }
    #endregion

    #region Highlight Functions
    /** Function:   HighlightTileSelect(int, int)
     *  Argument:   int x - the x coordinate of the tile to highlight
     *              int y - the y coordinate of the tile to highlight
     *  Output:     Makes the tile at board position (x, y) glow
     *  
     *  [NOTE]      Used for highlighting the tile pointed to by the mouse
     */
    public void HighlightTileSelect(int x, int y)
    {
        /* Fetch the tile game object */
        GameObject tile = allTiles[x, y];
        if (!tile)
            return;

        /* Fetch the Renderer component of the tile so we can change its material to the glow version */
        Renderer tile_rend = tile.GetComponentInChildren<Renderer>();
        tile_rend.enabled = true;
        highlightedTile_OriginalMat = tile_rend.sharedMaterial;
        tile_rend.sharedMaterial = select_glow;

        /* Update coordinates of the newly highlighted tile */
        highlightedTile_X = x;
        highlightedTile_Y = y;
    }

    /** Function:   HighlightTileMove(int, int)
     *  Argument:   int x - the x coordinate of the tile to highlight
     *              int y - the y coordinate of the tile to highlight
     *  Output:     Makes the tile at board position (x, y) glow
     *  
     *  [NOTE]      Used for highlighting the tiles a selected piece can possible move to
     */
    public void HighlightTileMoves(Hashtable moves)
    {
        int x, y;
        foreach(DictionaryEntry entry in moves)
        {
            x = ((BoardCoordinate)entry.Value).X;
            y = ((BoardCoordinate)entry.Value).Y;
            /* Fetch the tile game object */
            GameObject tile = allTiles[x, y];
            if (!tile)
                return;

            /* Determine the tile's Tiletype */
            TileType.Tiletype tile_type = tiles[x, y];

            /* Fetch the Renderer component of the tile so we can change its material to the glow version */
            Renderer tile_rend = tile.GetComponentInChildren<Renderer>();
            tile_rend.enabled = true;
            if (BoardManager.Instance.Board[x, y].ObjType == BoardObjects.Type.PIECE)
                tile_rend.sharedMaterial = attack_glow;
            else
                tile_rend.sharedMaterial = tileTypes[(int)tile_type].tileMaterials[(int)TileType.Mat.GLOW];

            /* Add coordinates to the list of highlighted tiles */
            highlightedTileMove.AddLast(new int[2] { x, y });
        }
    }

    /** Function:   UnhighlightTileSelect(int, int)
     *  Argument:   int x - the x coordinate of the tile to highlight
     *              int y - the y coordinate of the tile to highlight
     *  Output:     Makes the tile at board position (x, y) normal
     *  
     *  [NOTE]      Used for undoing the highlight on the tile pointed to by the mouse
     */
    public void UnhighlightSelect(int x, int y)
    {
        // Bounds check
        if (x < 0 || x > BoardManager.BOARD_SIZE - 1 || y < 0 || y > BoardManager.BOARD_SIZE - 1)
            return;

        // Fetch tile and do a validity check
        GameObject tile = allTiles[x, y];
        if (!tile)
            return;

        // Fetch Renderer component of the tile and restore it to its original mat
        Renderer tile_rend = tile.GetComponentInChildren<Renderer>();
        tile_rend.enabled = true;
        tile_rend.sharedMaterial = highlightedTile_OriginalMat;

        highlightedTile_X = -1;
        highlightedTile_Y = -1;
    }

    /** Function:   UnhighlightMoves(int, int)
     *  Argument:   int x - the x coordinate of the tile to highlight
     *              int y - the y coordinate of the tile to highlight
     *  Output:     Unhighlights all the tiles in the highlightedTileMove LL and empties the LL in the process
     *  
     *  [NOTE]      Used for undoing the highlight on the tile pointed to by the mouse
     */
    public void UnhighlightMoves()
    {
        int length = highlightedTileMove.Count;
        for (int i = 0; i < length; i++)
        {
            /* Extract coordinate data from linked list */
            int[] coord = highlightedTileMove.Last.Value;
            int x = coord[0];
            int y = coord[1];

            /* Fetch the tile at (x, y)*/
            GameObject tile = allTiles[x, y];
            if (!tile)
                return;

            /* Fetch the Tiletype of tile */
            TileType.Tiletype tile_type = tiles[x, y];

            /* Revert tile's material to normal unhighlighted material */
            Renderer tile_rend = tile.GetComponentInChildren<Renderer>();
            tile_rend.enabled = true;
            int mat = ((x + y) % 2 == 0) ? (int)TileType.Mat.LIGHT : (int)TileType.Mat.SHADED;
            tile_rend.sharedMaterial = tileTypes[(int)tile_type].tileMaterials[mat];

            /* Remove this element from the linked list */
            highlightedTileMove.RemoveLast();
        }
    }
    #endregion

}
