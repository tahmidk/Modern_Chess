using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chesspiece
{
    /** Function:   PossibleMoves (Pawn Override)
     *  Argument:   None
     *  Output:     Returns a boolean matrix of size BoardManager.BOARD_SIZE x BoardManager.BOARD_SIZE
     *              where each entry is either TRUE if it is possible for this Pawn to move there this 
     *              turn or FALSE if it cannot be reached this turn
     */
    public override Hashtable PossibleMoves()
    {
        Hashtable moveset = new Hashtable();
        BoardObjects[,] board = BoardManager.Instance.Board;

        BoardObjects temp = null;
        BoardCoordinate this_coord = new BoardCoordinate(CurrentX, CurrentY);   /* BoardCoordinates of the Pawn */
        BoardCoordinate temp_coord = null;  /* A temporary coordinate */
        BoardCoordinate prev_coord = null;  /* A coordinate 1 offset below temp_coord in the same direction */

        // If it is the first turn, can move 3 spaces in any direction, otherwise, can move max 2
        int range = (BoardManager.Instance.turn == 0 || BoardManager.Instance.turn == 1) ? 3 : 2;
        for (int offset = 1; offset <= range; offset++)
        {
            if(offset == 1)
            {
                // Check 1 tile to the right
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.EAST, offset);
                temp = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
                if (temp)
                    switch (temp.ObjType)
                    {
                        case BoardObjects.Type.EMPTY:    /* Space is empty */
                            moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                        case BoardObjects.Type.PIECE:   /* Space is occupied by a chess piece */
                            // Can only move here if the piece is of the opposite team
                            if(this.isWhite != temp.GetComponentInChildren<Chesspiece>().isWhite)
                                moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                    }

                // Check 1 tile to the left
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.WEST, offset);
                temp = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
                if (temp)
                    switch (temp.ObjType)
                    {
                        case BoardObjects.Type.EMPTY:    /* Space is empty */
                            moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                        case BoardObjects.Type.PIECE:   /* Space is occupied by a chess piece */
                            // Can only move here if the piece is of the opposite team
                            if (this.isWhite != temp.GetComponentInChildren<Chesspiece>().isWhite)
                                moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                    }

                // Check 1 tile to the front
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.NORTH, offset);
                temp = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
                if (temp)
                    switch (temp.ObjType)
                    {
                        case BoardObjects.Type.EMPTY:    /* Space is empty */
                            moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                        case BoardObjects.Type.PIECE:   /* Space is occupied by a chess piece */
                            // Can only move here if the piece is of the opposite team
                            if (this.isWhite != temp.GetComponentInChildren<Chesspiece>().isWhite)
                                moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                    }

                // Check 1 tile behind
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.SOUTH, offset);
                temp = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
                if (temp)
                    switch (temp.ObjType)
                    {
                        case BoardObjects.Type.EMPTY:    /* Space is empty */
                            moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                        case BoardObjects.Type.PIECE:   /* Space is occupied by a chess piece */
                            // Can only move here if the piece is of the opposite team
                            if (this.isWhite != temp.GetComponentInChildren<Chesspiece>().isWhite)
                                moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                    }
            }
            else
            {
                // Check 1 tile to the right
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.EAST, offset);
                prev_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.EAST, offset - 1);

                temp = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
                string prev_key = (prev_coord == null) ? "" : prev_coord.ToString();
                if (temp && moveset.ContainsKey(prev_key))
                {
                    // If the previous space was occupied by a piece, then we can't continue in this direction
                    if (board[prev_coord.X, prev_coord.Y].GetComponentInChildren<Chesspiece>() != null)
                        break;

                    switch (temp.ObjType)
                    {
                        case BoardObjects.Type.EMPTY:    /* Space is empty */
                            moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                        case BoardObjects.Type.PIECE:   /* Space is occupied by a chess piece */
                            // Can only move here if the piece is of the opposite team
                            if (this.isWhite != temp.GetComponentInChildren<Chesspiece>().isWhite)
                                moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                    }
                }

                // Check 1 tile to the left
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.WEST, offset);
                prev_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.WEST, offset - 1);

                temp = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
                prev_key = (prev_coord == null) ? "" : prev_coord.ToString();
                if (temp && moveset.ContainsKey(prev_key))
                {
                    // If the previous space was occupied by a piece, then we can't continue in this direction
                    if (board[prev_coord.X, prev_coord.Y].GetComponentInChildren<Chesspiece>() != null)
                        break;
                    
                    switch (temp.ObjType)
                    {
                        case BoardObjects.Type.EMPTY:    /* Space is empty */
                            moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                        case BoardObjects.Type.PIECE:   /* Space is occupied by a chess piece */
                            // Can only move here if the piece is of the opposite team
                            if (this.isWhite != temp.GetComponentInChildren<Chesspiece>().isWhite)
                                moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                    }
                }

                // Check 1 tile to the front
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.NORTH, offset);
                prev_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.NORTH, offset - 1);

                temp = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
                prev_key = (prev_coord == null) ? "" : prev_coord.ToString();
                if (temp && moveset.ContainsKey(prev_key))
                {
                    // If the previous space was occupied by a piece, then we can't continue in this direction
                    if (board[prev_coord.X, prev_coord.Y].GetComponentInChildren<Chesspiece>() != null)
                        break;

                    switch (temp.ObjType)
                    {
                        case BoardObjects.Type.EMPTY:    /* Space is empty */
                            moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                        case BoardObjects.Type.PIECE:   /* Space is occupied by a chess piece */
                            // Can only move here if the piece is of the opposite team
                            if (this.isWhite != temp.GetComponentInChildren<Chesspiece>().isWhite)
                                moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                    }
                }

                // Check 1 tile behind
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.SOUTH, offset);
                prev_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.SOUTH, offset - 1);

                temp = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
                prev_key = (prev_coord == null) ? "" : prev_coord.ToString();
                if (temp && moveset.ContainsKey(prev_key))
                {
                    // If the previous space was occupied by a piece, then we can't continue in this direction
                    if (board[prev_coord.X, prev_coord.Y].GetComponentInChildren<Chesspiece>() != null)
                        break;

                    switch (temp.ObjType)
                    {
                        case BoardObjects.Type.EMPTY:    /* Space is empty */
                            moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                        case BoardObjects.Type.PIECE:   /* Space is occupied by a chess piece */
                            // Can only move here if the piece is of the opposite team
                            if (this.isWhite != temp.GetComponentInChildren<Chesspiece>().isWhite)
                                moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                    }
                }
            }
        }

        return moveset;
    }

    public override void GoToJump(Vector3 destination)
    {
        Animator animator = this.GetComponent<Animator>();
        animator.Play("Pawn_JUMP", -1, 0f);
        transform.position = destination;
    }

    public override void GoToAttack(Vector3 destination)
    {
        Animator animator = this.GetComponent<Animator>();
    }
}
