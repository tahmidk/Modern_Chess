using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Function:       BoardObjects
 *  Description:    A BoardObject is any item that can be placed on the chess board/map and can occupy 
 *                  a tile of the board
 *  Properties:     objType - The type of object that the BoardObject is
 *                  CurrentX - All BoardObjects have a x-cooridnate on the board
 *                  CurrentY - All BoardObjects have a y-coordinate on the board
 */
public abstract class BoardObjects : MonoBehaviour
{
    /** enum Type:
     *      EMPTY - represents a place BoardObjects can potentially occupy but is currently not occupied
     *      PIECE - represents a Chesspiece
     *      ROCK -  represents an interactable Rock obstacle
     *      PROP -  represents any uninteractable obstacle
     *      ITEM -  represents an interactable Item
     */
    public enum Type {EMPTY, PIECE, ROCK, TREE, ITEM};

    public Type ObjType;                /* The type of this object */
    public int CurrentX { get; set; }   /* Current x board position */
    public int CurrentY { get; set; }   /* Current y board position */

    /** Function:   SetPosition(int, int)
     *  Argument:   int x - the new x position to move to
     *              int y - the new y position to move to
     *  Output:     Mutates CurrentX and CurrentY appropriately
     */
    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }
}
