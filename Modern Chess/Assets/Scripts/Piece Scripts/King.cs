using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chesspiece
{
    private const float BASE_SPEED = 7.0f / 3.0f;
    private float speed;

    private void Start()
    {
        // Initialize fields
        animator = this.GetComponent<Animator>();
        role = Rank.KING;
        destination = this.transform.position;
    }

    private void Update()
    {
        // If there is a new destination, move towards that destination
        if (transform.position != destination)
        {
            Vector3 start_pos = this.GetComponentInParent<Transform>().position;
            Vector3 direction = destination - start_pos;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position = Vector3.MoveTowards(start_pos, destination, speed * Time.deltaTime);
        }
    }

    public override Hashtable PossibleMoves()
    {
        Hashtable moveset = new Hashtable();
        BoardObjects[,] board = BoardManager.Instance.Board;

        BoardObjects temp = null;
        BoardCoordinate this_coord = new BoardCoordinate(CurrentX, CurrentY);   /* BoardCoordinates of the King */
        BoardCoordinate temp_coord = null;  /* A temporary coordinate */
        int offset = 1;

        foreach(BoardCoordinate.Direction dir in System.Enum.GetValues(typeof(BoardCoordinate.Direction)))
        {
            temp_coord = this_coord.GetOffsetCoord(dir, offset);
            temp = (temp_coord == null) ? null : board[temp_coord.X, temp_coord.Y];
            if (temp)
            {
                if (temp.ObjType == BoardObjects.Type.PIECE)
                {
                    Chesspiece neighbor = temp.GetComponentInChildren<Chesspiece>();
                    bool evenTurn = (BoardManager.Instance.turn % 2 == 0);
                    if (neighbor.isWhite == !evenTurn)
                        moveset.Add(temp_coord.ToString(), temp_coord);
                }
                else
                    moveset.Add(temp_coord.ToString(), temp_coord);
            }
        }

        return moveset;
    }

    public override void GoToJump(Vector3 dest)
    {
        if (dest == destination)
            return;

        // Play jump animation
        animator.Play("King_JUMP", -1, 0f);

        // Transition to new position
        speed = BASE_SPEED * Vector3.Distance(destination, dest);
        StartCoroutine(DelayedTransition(dest));
    }

    public override void GoToAttack(Vector3 destination)
    {
        //Animator animator = this.GetComponent<Animator>();
    }
}
