using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Chesspiece
{
    private const float BASE_SPEED = 7f;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        role = Rank.ROOK;
        destination = this.transform.position;
    }

    private void Update()
    {
        // If there is a new destination, move towards that destination
        if (transform.position != destination)
        {
            float distance = Vector3.Distance(transform.position, destination);
            if (distance > 3)
                animator.Play("Rook_SLIDE", -1, 0f);
            else
                animator.Play("Rook_SLIDE_STOP", -1, 0f);

            Vector3 start_pos = this.GetComponentInParent<Transform>().position;
            Vector3 direction = destination - start_pos;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position = Vector3.MoveTowards(start_pos, destination, BASE_SPEED * Time.deltaTime);
        }
    }

    /** Function:   PossibleMoves (Rook Override)
     *  Argument:   None
     *  Output:     Returns the possible moves of a rook. A rook can attack in and move any number of spaces in any 
     *              off-diagonal directions (N, S, E, W)
     */
    public override Hashtable PossibleMoves()
    {
        // Initialize hashtable to return
        Hashtable moveset = new Hashtable();
        BoardObjects[,] board = BoardManager.Instance.Board;

        BoardObjects temp_obj = null;
        BoardCoordinate this_coord = new BoardCoordinate(CurrentX, CurrentY);   /* BoardCoordinates of the Pawn */
        BoardCoordinate temp_coord = null;  /* A temporary coordinate */

        bool proceedForward = true;
        bool proceedBackward = true;
        bool proceedLeft = true;
        bool proceedRight = true;

        for(int offset = 1; offset < BoardManager.BOARD_SIZE; offset++)
        {
            if (!proceedForward && !proceedBackward && !proceedLeft && !proceedRight)
                break;

            // Check the [offset]th tile in front 
            if(proceedForward)
            {
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.NORTH, offset);
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
                            proceedForward = false;
                            break;
                        default:
                            proceedForward = false;
                            break;
                    }
                }
                else
                    proceedForward = false;
            }

            // Check the [offset]th tile behind 
            if (proceedBackward)
            {
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.SOUTH, offset);
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
                            proceedBackward = false;
                            break;
                        default:
                            proceedBackward = false;
                            break;
                    }
                }
                else
                    proceedBackward = false;
            }

            // Check the [offset]th tile to the left 
            if (proceedLeft)
            {
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.WEST, offset);
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
                            proceedLeft = false;
                            break;
                        default:
                            proceedLeft = false;
                            break;
                    }
                }
                else
                    proceedLeft = false;
            }

            // Check the [offset]th tile to the right 
            if (proceedRight)
            {
                temp_coord = this_coord.GetOffsetCoord(BoardCoordinate.Direction.EAST, offset);
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
                            proceedRight = false;
                            break;
                        default:
                            proceedRight = false;
                            break;
                    }
                }
                else
                    proceedRight = false;
            }
        }

        return moveset;
    }

    /** Function:   GoToSlide(Vec3)
     *  Argument:   Vec3 dest - the exact world coordinate to jump to (NOT board coordinates)
     *  Output:     Executes an animated slide to the position given by the vector dest with 
     *              the appropriate animations
     */
    public override void GoToSlide(Vector3 dest)
    {
        if (dest == destination)
            return;

        // Begin slide animation
        animator.Play("Rook_SLIDE_BEGIN", -1, 0f);

        // Transition to new position
        StartCoroutine(DelayedTransition(dest));
    }

    public override void GoToAttack(Vector3 destination)
    {
        //Animator animator = this.GetComponent<Animator>();
    }
}
