using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Chesspiece
{
    private const float BASE_SPEED = 7f;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        role = Rank.BISHOP;
        destination = this.transform.position;
    }

    private void Update()
    {
        // If there is a new destination, move towards that destination
        if (transform.position != destination)
        {
            float distance = Vector3.Distance(transform.position, destination);
            if (distance > 3)
                animator.Play("Bishop_SLIDE", -1, 0f);
            else
                animator.Play("Bishop_SLIDE_STOP", -1, 0f);

            Vector3 start_pos = this.GetComponentInParent<Transform>().position;
            Vector3 direction = destination - start_pos;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position = Vector3.MoveTowards(start_pos, destination, BASE_SPEED * Time.deltaTime);
        }
    }

    /** Function:   PossibleMoves (Bishop Override)
     *  Argument:   None
     *  Output:     Returns the possible moves of a rook. A rook can attack in and move any number of spaces in any 
     *              of the diagonal directions (NE, SE, NW, SW)
     */
    public override Hashtable PossibleMoves()
    {
        // Initialize hashtable to return
        Hashtable moveset = new Hashtable();
        BoardObjects[,] board = BoardManager.Instance.Board;

        BoardObjects temp_obj = null;
        BoardCoordinate this_coord = new BoardCoordinate(CurrentX, CurrentY);   /* BoardCoordinates of the Pawn */
        BoardCoordinate temp_coord = null;  /* A temporary coordinate */

        bool proceedNE = true;
        bool proceedSE = true;
        bool proceedNW = true;
        bool proceedSW = true;

        for (int offset = 1; offset < BoardManager.BOARD_SIZE; offset++)
        {
            if (!proceedNE && !proceedSE && !proceedNW && !proceedSW)
                break;

            // Check the [offset]th tile in the north east direction
            if (proceedNE)
            {
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.NE, offset);
                temp_obj = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
                if (temp_obj)
                {
                    switch (temp_obj.ObjType)
                    {
                        case BoardObjects.Type.EMPTY:
                            moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                        case BoardObjects.Type.PIECE:
                            Chesspiece neighbor = temp_obj.GetComponentInChildren<Chesspiece>();
                            bool evenTurn = (BoardManager.Instance.turn % 2 == 0);
                            if (neighbor.isWhite == !evenTurn)
                                moveset.Add(temp_coord.ToString(), temp_coord);
                            // Cannot proceed in this direction anymore
                            proceedNE = false;
                            break;
                        default:
                            proceedNE = false;
                            break;
                    }
                }
                else
                    proceedNE = false;
            }

            // Check the [offset]th tile in the south east direction 
            if (proceedSE)
            {
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.SE, offset);
                temp_obj = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
                if (temp_obj)
                {
                    switch (temp_obj.ObjType)
                    {
                        case BoardObjects.Type.EMPTY:
                            moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                        case BoardObjects.Type.PIECE:
                            Chesspiece neighbor = temp_obj.GetComponentInChildren<Chesspiece>();
                            bool evenTurn = (BoardManager.Instance.turn % 2 == 0);
                            if (neighbor.isWhite == !evenTurn)
                                moveset.Add(temp_coord.ToString(), temp_coord);
                            // Cannot proceed in this direction anymore
                            proceedSE = false;
                            break;
                        default:
                            proceedSE = false;
                            break;
                    }
                }
                else
                    proceedSE = false;
            }

            // Check the [offset]th tile in the north west direction 
            if (proceedNW)
            {
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.NW, offset);
                temp_obj = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
                if (temp_obj)
                {
                    switch (temp_obj.ObjType)
                    {
                        case BoardObjects.Type.EMPTY:
                            moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                        case BoardObjects.Type.PIECE:
                            Chesspiece neighbor = temp_obj.GetComponentInChildren<Chesspiece>();
                            bool evenTurn = (BoardManager.Instance.turn % 2 == 0);
                            if (neighbor.isWhite == !evenTurn)
                                moveset.Add(temp_coord.ToString(), temp_coord);
                            // Cannot proceed in this direction anymore
                            proceedNW = false;
                            break;
                        default:
                            proceedNW = false;
                            break;
                    }
                }
                else
                    proceedNW = false;
            }

            // Check the [offset]th tile in the south west direction 
            if (proceedSW)
            {
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.SW, offset);
                temp_obj = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
                if (temp_obj)
                {
                    switch (temp_obj.ObjType)
                    {
                        case BoardObjects.Type.EMPTY:
                            moveset.Add(temp_coord.ToString(), temp_coord);
                            break;
                        case BoardObjects.Type.PIECE:
                            Chesspiece neighbor = temp_obj.GetComponentInChildren<Chesspiece>();
                            bool evenTurn = (BoardManager.Instance.turn % 2 == 0);
                            if (neighbor.isWhite == !evenTurn)
                                moveset.Add(temp_coord.ToString(), temp_coord);
                            // Cannot proceed in this direction anymore
                            proceedSW = false;
                            break;
                        default:
                            proceedSW = false;
                            break;
                    }
                }
                else
                    proceedSW = false;
            }
        }

        return moveset;
    }

    public override void GoToSlide(Vector3 dest)
    {
        if (dest == destination)
            return;

        // Begin slide animation
        animator.Play("Bishop_SLIDE_BEGIN", -1, 0f);

        // Transition to new position
        StartCoroutine(DelayedTransition(dest));
    }

    public override void GoToAttack(Vector3 dest)
    {
        //Animator animator = this.GetComponent<Animator>();
    }
}
