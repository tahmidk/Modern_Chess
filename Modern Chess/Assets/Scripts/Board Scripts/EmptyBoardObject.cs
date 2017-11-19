using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Class:          EmptyBoardObject
 *  Description:    An EmptyBoardObject is a BoardObject that represents an occupiable space on the 
 *                  chess board / map
 */
public class EmptyBoardObject : BoardObjects
{
    /* The [status] component implemented for the EmptyBoardObjects is pivotal to the implementation of
     * the check/checkmate system. The following is what each status denots:
     * -----Status------|------------------------Description-------------------------
     * FREE             | this space is not currently in the moveset of any of the chesspieces
     * WHITE_OCCUPIED   | this space is in the moveset of at least 1 white piece
     * BLACK_OCCUPIED   | this space is in the moveset of at least 1 black piece
     * BOTH_OCCUPIED    | this space is in the moveset of at least 1 piece from both teams
     */
    public enum Status {FREE, NULL, WHITE_OCCUPIED, BLACK_OCCUPIED, BOTH_OCCUPIED}

    public Status status;   /* Holds the occupation status of this empty space */

    public EmptyBoardObject()
    {
        ObjType = BoardObjects.Type.EMPTY;
        status = Status.FREE;
    }

    /** Function:   RemoveStatus(Status)
     *  Argument:   s - the status to remove
     *  Output:     Removes the status s from this object and appropriately updates the status
     *              field to accurately reflect the change
     */
    public void RemoveStatus(Status s)
    {
        switch(status)
        {
            // If [status] is already FREE, RemoveStatus changes nothing regardless of s
            case Status.FREE:           
                break;
            // If [status] is WHITE_OCCUPIED and we're trying to remove the status s = WHITE_OCCUPIED, the new 
            // [status] is FREE. Otherwise, nothing changes
            case Status.WHITE_OCCUPIED:
                if (s == Status.WHITE_OCCUPIED)
                    status = Status.FREE;
                break;
            // The black analog to the above case
            case Status.BLACK_OCCUPIED:
                if (s == Status.BLACK_OCCUPIED)
                    status = Status.FREE;
                break;
            // If [status] is BOTH_OCCUPIED, it becomes FREE if we remove the status BOTH_OCCUPIED
            //                              ... becomes WHITE_OCCUPIED if we remove the BLACK_OCCUPIED status
            //                              ... becomes BLACK_OCCUPIED if we remove the WHITE_OCCUPIED status
            case Status.BOTH_OCCUPIED:
                if (s == Status.BOTH_OCCUPIED)
                    status = Status.FREE;
                else if (s == Status.BLACK_OCCUPIED)
                    status = Status.WHITE_OCCUPIED;
                else if (s == Status.WHITE_OCCUPIED)
                    status = Status.BLACK_OCCUPIED;
                break;
        }
    }

    /** Function:   UpdateStatus(Status)
     *  Argument:   s - the status to add
     *  Output:     Updates the [status] field such that it appropriately includes the status s
     */
    public void UpdateStatus(Status s)
    {
        switch(status)
        {
            // Update normally if [status] field is FREE
            case Status.FREE:
                status = s;
                break;
            // If [status] field is WHITE_OCCUPIED and s is BLACK_OCCUPIED, [status] becomes both occupied.
            // Otherwise, if s is WHITE_OCCUPIED, nothing changes
            case Status.WHITE_OCCUPIED:
                if (s == Status.BLACK_OCCUPIED)
                    status = Status.BOTH_OCCUPIED;
                break;
            // The black analog to the above case
            case Status.BLACK_OCCUPIED:
                if (s == Status.WHITE_OCCUPIED)
                    status = Status.BOTH_OCCUPIED;
                break;
            // If [status] is BOTH_OCCUPIED, UpdateStatus() does not change anything regardless of s
            case Status.BOTH_OCCUPIED:
                break;
        }
    }
}
