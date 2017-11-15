using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Class:          EmptyBoardObject
 *  Description:    An EmptyBoardObject is a BoardObject that represents an occupiable space on the 
 *                  chess board / map
 */
public class EmptyBoardObject : BoardObjects
{
    public EmptyBoardObject()
    {
        ObjType = BoardObjects.Type.EMPTY;
    }
}
