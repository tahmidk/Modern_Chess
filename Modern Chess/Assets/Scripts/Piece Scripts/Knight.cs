using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Chesspiece
{
    private const float BASE_SPEED = 7.0f / 3.0f;
    private float speed;

    private void Start()
    {
        // Initialize fields
        animator = this.GetComponent<Animator>();
        role = Rank.KNIGHT;
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
        BoardCoordinate temp_coord = null;  /* A temporary coordinate */

        List<BoardCoordinate> possible = new List<BoardCoordinate>();
        BoardCoordinate NEAnchor = this_coord.GetOffsetCoord(BoardCoordinate.Direction.NE, 1);
        BoardCoordinate SEAnchor = this_coord.GetOffsetCoord(BoardCoordinate.Direction.SE, 1);
        BoardCoordinate NWAnchor = this_coord.GetOffsetCoord(BoardCoordinate.Direction.NW, 1);
        BoardCoordinate SWAnchor = this_coord.GetOffsetCoord(BoardCoordinate.Direction.SW, 1);

        if(NEAnchor != null)
        {
            if ((temp_coord = NEAnchor.GetOffsetCoord(BoardCoordinate.Direction.NORTH, 1)) != null)
                possible.Add(temp_coord);
            if ((temp_coord = NEAnchor.GetOffsetCoord(BoardCoordinate.Direction.EAST, 1)) != null)
                possible.Add(temp_coord);
        }

        if (SEAnchor != null)
        {
            if ((temp_coord = SEAnchor.GetOffsetCoord(BoardCoordinate.Direction.SOUTH, 1)) != null)
                possible.Add(temp_coord);
            if ((temp_coord = SEAnchor.GetOffsetCoord(BoardCoordinate.Direction.EAST, 1)) != null)
                possible.Add(temp_coord);
        }

        if (NWAnchor != null)
        {
            if ((temp_coord = NWAnchor.GetOffsetCoord(BoardCoordinate.Direction.NORTH, 1)) != null)
                possible.Add(temp_coord);
            if ((temp_coord = NWAnchor.GetOffsetCoord(BoardCoordinate.Direction.WEST, 1)) != null)
                possible.Add(temp_coord);
        }

        if (SWAnchor != null)
        {
            if ((temp_coord = SWAnchor.GetOffsetCoord(BoardCoordinate.Direction.SOUTH, 1)) != null)
                possible.Add(temp_coord);
            if ((temp_coord = SWAnchor.GetOffsetCoord(BoardCoordinate.Direction.WEST, 1)) != null)
                possible.Add(temp_coord);
        }

        // Do a final check to make sure there aren't any allies in the list of possible moves before adding
        // them to the moveset hashtable
        foreach (BoardCoordinate coord in possible)
        {
            temp = board[coord.X, coord.Y];
                Chesspiece neighbor = temp.GetComponentInChildren<Chesspiece>();
                bool evenTurn = (BoardManager.Instance.turn % 2 == 0);
                if (neighbor.isWhite == !evenTurn)
                    moveset.Add(coord.ToString(), coord);
            }
<<<<<<< HEAD
            else if (temp && temp.ObjType == BoardObjects.Type.EMPTY)
=======
>>>>>>> 5b66876aa7c3baad75ee3417dd76fd5cd87fc180
                moveset.Add(coord.ToString(), coord);
        }

        return moveset;
    }

    public override void GoToJump(Vector3 dest)
    {
        if (dest == destination)
            return;

        // Play jump animation
        animator.Play("Knight_JUMP", -1, 0f);

        // Transition to new position
        speed = BASE_SPEED * Vector3.Distance(destination, dest);
        StartCoroutine(DelayedTransition(dest));
    }

    public override void GoToAttack(Vector3 destination)
    {
        //Animator animator = this.GetComponent<Animator>();
    }
}
