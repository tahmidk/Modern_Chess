using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCoordinate
{
    public enum Direction {NORTH, NE, EAST, SE, SOUTH, SW, WEST, NW}

    private int _x;     /* the x board coordinate */
    private int _y;     /* the y board coordinate */
    public int X { get { return _x; } set { return; } }    
    public int Y { get { return _y; } set { return; } }    

    public BoardCoordinate(int x, int y)
    {
        _x = x;
        _y = y;
    }

    /** Function:   GetCoord(Direction, int)
     *  Argument:   Direction dir - one of the 8 possible directions
     *              int offset - the number of tiles away from this BoardCoordinate's coordinates
     *  Output:     Returns a new BoardCoordinate with the new cooridnates if it is within the boundaries
     *              of the square chess board
     *              Otherwise, if it is out of bounds, returns NULL
     */
    public BoardCoordinate GetOffsetCoord(Direction dir, int offset)
    {
        int newX = _x, newY = _y;
        switch(dir)
        {
            case Direction.NORTH:
                newY += offset;
                break;

            case Direction.NE:
                newX += offset;
                newY += offset;
                break;

            case Direction.EAST:
                newX += offset;
                break;

            case Direction.SE:
                newX += offset;
                newY -= offset;
                break;

            case Direction.SOUTH:
                newY -= offset;
                break;

            case Direction.SW:
                newX -= offset;
                newY -= offset;
                break;

            case Direction.WEST:
                newX -= offset;
                break;

            case Direction.NW:
                newX -= offset;
                newY += offset;
                break;
        }

        if (newX > 0 && newX < BoardManager.BOARD_SIZE &&
            newY > 0 && newY < BoardManager.BOARD_SIZE)
            return new BoardCoordinate(newX, newY);

        return null;
    }

    // Overrided to string method
    public override string ToString()
    {
        return "(" + _x + ", " + _y + ")"; ;
    }
}
