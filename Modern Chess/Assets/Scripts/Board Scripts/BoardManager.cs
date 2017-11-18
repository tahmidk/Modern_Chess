using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Class:          BoardManager
 *  Description:    The main class that holds all details about the chess board / map. This includes object and 
 *                  piece information, object locations, and what chess pieces are active on the board. Also
 *                  responsible for initially spawning the chess pieces
 */
public class BoardManager : MonoBehaviour
{
    // Set up an instance of this class so other classes can access it
    public static BoardManager Instance { get; set; }

    #region Public Macros
    public const int NUM_PIECES = 8;                    /* Number of pieces is 8 regardless of board size */
    public const int BOARD_SIZE = 16;                   /* Board will be 16 tiles x 16 tiles (must be even and >8) */
    public const float TILE_SIZE = 1.0f;                /* Size of tiles: 1m x 1m */
    public const float TILE_OFFSET = TILE_SIZE / 2.0f;  /* The offset of the center of the tile from the edges */
    #endregion

    #region Instance Fields
    private Hashtable AllowedMoves {get; set;}

    // Board is a 2D array of BoardObjects that maintains the occupants of the board at each board position
    public BoardObjects[,] Board { get; set; }
    private Chesspiece selected_piece;                  /* The currently selected chess piece */

    /* These fields hold the coordinate location of the selected tile
     * [Note] -1 denotes no selection and (0,0) refers to bottom left tile */
    private int tileSelected_X = -1;
    private int tileSelected_Y = -1;

    /* Maintains which turn it is... even turns are white's and off turns are black's */
    public int turn = 0;    

    /* Enumeration representing what prefab is in what position in chessPiecePrefabs */
    private enum Piece {
        WHT_KING, WHT_QUEEN, WHT_ROOK, WHT_BISHOP, WHT_KNIGHT, WHT_PAWN,
        BLK_KING, BLK_QUEEN, BLK_ROOK, BLK_BISHOP, BLK_KNIGHT, BLK_PAWN
    };

    public List<GameObject> chessPiecePrefabs;  /* Contains a list of the prefabs for the chess pieces */
    private List<GameObject> activeChessPieces; /* Maintains a list of the chess pieces currently on the board */
    #endregion

    /*----------------------------------------[START]----------------------------------------------*/
    private void Start()
    {
        Instance = this;

        // Initialize non-primitive data structure fields
        activeChessPieces = new List<GameObject>();
        Board = new BoardObjects[BOARD_SIZE, BOARD_SIZE];
        AllowedMoves = new Hashtable();

        // Generate world
        TileMap.Instance.GenerateBoard();
        this.SpawnAll();

    }

    /*----------------------------------------[UPDATE]---------------------------------------------*/
    private void Update()
    {
        UpdateSelection();
        DrawChessBoard();
        
        // If user left-clicked
        if (Input.GetMouseButtonDown(0))
            // Check if it's a valid tile selected
            if (tileSelected_X != -1 && tileSelected_Y != -1)
                /* There is no piece already selected, we are in select mode */
                if (!selected_piece)
                    SelectChesspiece(tileSelected_X, tileSelected_Y);
                /* There is a selected piece, we are in move mode */
                else
                {
                    MoveChesspiece(tileSelected_X, tileSelected_Y);
                    TileMap.Instance.UnhighlightMoves();
                }

    }

    /** Function:   SelectChesspiece(int, int)
     *  Arguments:  int x - the x position of the clicked tile on the board
     *              int y - the y position of the clicked tile on the board
     *  Output:     Selects any chess piece at square (x, y) of the board if possible
     *              If not, then this function does nothing
     *  Sideeffect: Mutates the selected_piece field
     */
    private void SelectChesspiece(int x, int y)
    {
        // Fetch the BoardObject and do a validity test
        BoardObjects selection = Board[x, y];
        if (selection == null)
        {
            TileMap.Instance.UnhighlightMoves();
            AllowedMoves.Clear();
            return;
        }

        // If there is no piece at (x, y) do not select anything
        if (selection.ObjType != BoardObjects.Type.PIECE)
        {
            TileMap.Instance.UnhighlightMoves();
            AllowedMoves.Clear();
            return;
        }
 
        // Safely cast selection to a Chesspiece
        Chesspiece sel = (Chesspiece) selection;

        // If trying to select a piece of the opposite team do not select anything
        if (sel.isWhite != (turn % 2 == 0))
        {
            TileMap.Instance.UnhighlightMoves();
            AllowedMoves.Clear();
            return;
        }

        // Otherwise, select whatever is at Board[x,y]
        selected_piece = sel;
        selected_piece.FocusCam(CameraFollow.Behavior.INSTANT);     /* Focus camera on selected piece */
        selected_piece.Highlight();                                 /* Highlight piece */
        TileMap.Instance.HighlightTileMoves(selected_piece.PossibleMoves());    /* Highlight moveset */
    }

    /** Function:   MoveChesspiece(int, int)
     *  Arguments:  int x - the x position of the clicked tile on the board
     *              int y - the y position of the clicked tile on the board
     *  Output:     Attempts to move the selected piece to the position (x, y)
     *              If this is an impossible move, deselects the selected piece
     *  Sideeffect: Sets selected_piece to null at the end of the function
     *              Increments turn at the end of the function
     */
    private void MoveChesspiece(int x, int y)
    {
        // Validity check
        if (selected_piece == null)
            return;

        // Make the move if possible
        BoardCoordinate coord = new BoardCoordinate(x, y);
        AllowedMoves = selected_piece.PossibleMoves();                          /* Fetch the possible moves */
        if (AllowedMoves.Contains(coord.ToString()))
        {
            Board[selected_piece.CurrentX, selected_piece.CurrentY] = Board[x, y];  /* Previous position vacant */
            Board[x, y] = selected_piece;                                           /* Occupy new position */

            // Update selected piece's fields and animate the movement
            selected_piece.CurrentX = x;
            selected_piece.CurrentY = y;
            selected_piece.GoTo(GetTileCenter(x, y));

            // Go to next turn
            turn++;
        }

        // Reset the selected piece
        TileMap.Instance.UnhighlightSelect(x, y);
        selected_piece.Unhighlight();
        AllowedMoves.Clear();
        selected_piece = null;
    }

    /** Function:   SpawnAll()
     *  Argument:   None
     *  Output:     Spawns all black and white chess pieces on both sides of board
     *  
     *  [POSTCONDITION] Populates activeChessPieces in the process
     */
    private void SpawnAll()
    {
        const int MID_R = BOARD_SIZE / 2;
        const int MID_L = MID_R - 1;

        // Spawn Kings
        SpawnChesspiece(Piece.BLK_KING, GetTileCenter(MID_R, BOARD_SIZE - 1));
        SpawnChesspiece(Piece.WHT_KING, GetTileCenter(MID_R, 0));

        // Spawn Queens
        SpawnChesspiece(Piece.BLK_QUEEN, GetTileCenter(MID_L, BOARD_SIZE - 1));
        SpawnChesspiece(Piece.WHT_QUEEN, GetTileCenter(MID_L, 0));

        // Spawn Bishops
        SpawnChesspiece(Piece.BLK_BISHOP, GetTileCenter(MID_L - 1, BOARD_SIZE-1));
        SpawnChesspiece(Piece.BLK_BISHOP, GetTileCenter(MID_R + 1, BOARD_SIZE-1));
        SpawnChesspiece(Piece.WHT_BISHOP, GetTileCenter(MID_L - 1, 0));
        SpawnChesspiece(Piece.WHT_BISHOP, GetTileCenter(MID_R + 1, 0));

        // Spawn Knights
        SpawnChesspiece(Piece.BLK_KNIGHT, GetTileCenter(MID_L - 2, BOARD_SIZE-1));
        SpawnChesspiece(Piece.BLK_KNIGHT, GetTileCenter(MID_R + 2, BOARD_SIZE-1));
        SpawnChesspiece(Piece.WHT_KNIGHT, GetTileCenter(MID_L - 2, 0));
        SpawnChesspiece(Piece.WHT_KNIGHT, GetTileCenter(MID_R + 2, 0));

        // Spawn Rooks
        SpawnChesspiece(Piece.BLK_ROOK, GetTileCenter(MID_L - 3, BOARD_SIZE - 1));
        SpawnChesspiece(Piece.BLK_ROOK, GetTileCenter(MID_R + 3, BOARD_SIZE - 1));
        SpawnChesspiece(Piece.WHT_ROOK, GetTileCenter(MID_L - 3, 0));
        SpawnChesspiece(Piece.WHT_ROOK, GetTileCenter(MID_R + 3, 0));

        // Spawn Pawns
        for (int X = 0; X < 4; X++)
        {
            // We spawn the two pawns of each color relative to the middle each iteration
            SpawnChesspiece(Piece.BLK_PAWN, GetTileCenter(MID_R + X, BOARD_SIZE-2));
            SpawnChesspiece(Piece.BLK_PAWN, GetTileCenter(MID_L - X, BOARD_SIZE-2));
            SpawnChesspiece(Piece.WHT_PAWN, GetTileCenter(MID_R + X, 1));
            SpawnChesspiece(Piece.WHT_PAWN, GetTileCenter(MID_L - X, 1));
        }
    }

    /** Function:   SpawnChesspieces(int, Vec3)
     *  Arguments:  Piece piece - the chess piece to spawn from the chessPiecePrefabs List
     *              Vec3 location - the exact coordinates in the world where this piece should be spawned
     *  Output:     Generates the single chess piece chessPievePrefabs[index] at position
     *  Sideeffect: Populates the activeChessPieces Lists with the piece spawned
     */
    private void SpawnChesspiece(Piece piece, Vector3 location)
    {
        // Instantiate the chess piece at position with no added rotation (Quaternion.identity)
        GameObject obj = (GameObject) Instantiate(chessPiecePrefabs[(int) piece], location, Quaternion.identity);
        obj.transform.SetParent(transform);

        // Set this piece's position in the Chesspieces array 
        int pos_x = (int) location.x;
        int pos_y = (int) location.z;
        Board[pos_x, pos_y] = obj.GetComponent<Chesspiece>();
        Board[pos_x, pos_y].SetPosition(pos_x, pos_y);

        activeChessPieces.Add(obj);

        if (piece == Piece.WHT_KING)
            obj.GetComponentInChildren<Chesspiece>().FocusCam(CameraFollow.Behavior.INSTANT);
    }

    /** Function:   GetTileCenter(int, int)
     *  Argument:   Take the board position of the tile to be (X, Y)
     *              int x - represents X, an int in range [0, BOARD_SIZE]
     *              int y - represents Y, an int in range [0, BOARD_SIZE]
     *  Output:     Returns a Vec3 containing the exact offset (x, y) position of the center of the tile
     */
    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 res = Vector3.zero;
        res.x += (TILE_SIZE * x) + TILE_OFFSET;
        res.z += (TILE_SIZE * y) + TILE_OFFSET;

        return res;
    }

    /** Function:   UpdateSelection()
     *  Argument:   None 
     *  Output:     If there is a camera, this method uses a Raycast to set the tile pointed to by the mouse
     *              as the selection tile and appropriately highlights the tile
     */
    private void UpdateSelection()
    {
        // If there is no camera, we can't cast a ray from it... return
        if (!Camera.main)
            return;

        // Highlight the tile pointed to by the mouse
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("RayIgnoreLayer")))
        {
            tileSelected_X = (int) hit.point.x;
            tileSelected_Y = (int) hit.point.z;
            if (TileMap.Instance.highlightedTile_X != tileSelected_X || TileMap.Instance.highlightedTile_Y != tileSelected_Y)
            {
                // Unhighlight previous selected tile
                TileMap.Instance.UnhighlightSelect(TileMap.Instance.highlightedTile_X, 
                                                    TileMap.Instance.highlightedTile_Y);
                // Highlight newly selected tile
                TileMap.Instance.HighlightTileSelect(tileSelected_X, tileSelected_Y);
            }
        }
        else
        {
            // If the board is not being moused over, reset coordinates to NULL
            tileSelected_X = -1;
            tileSelected_Y = -1;
        }
    }

    /* A debug method used to visualize the chess board */
    private void DrawChessBoard()
    {
        Vector3 widthLine = Vector3.right * BOARD_SIZE;     // Horizontal lines
        Vector3 heightLine = Vector3.forward * BOARD_SIZE;  // Vertical lines

        for (int i = 0; i <= BOARD_SIZE; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for (int j = 0; j <= BOARD_SIZE; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);

            }
        }
    }
}
