using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Class:          Chesspiece
 *  Description:    An uninstantiable class that inherits from BoardObjects. A Chesspiece is a BoardObject
 *                  that the players can directly move.
 *  Properties:     mats    - All Chesspieces can be highlighted so they come with an array of materials containing
 *                            the original material of the Chesspiece and the highlighted version
 *                  isWhite - All Chesspieces are either on the white team or the black team, which this property is
 *                            used to indicate
 */
public abstract class Chesspiece : BoardObjects
{
    // An enum for the indecies of the mats array field
    public enum Mat {ORIGINAL, GLOW}

    public Material[] mats;             /* Holds the different materials possible for the chesspiece
                                            original material (indx:0), GLOW (indx:1) */
    public bool isWhite;                /* Boolean to determine color of this piece */

    /** Function:   Highlight()
     *  Argument:   
     *  Output:     Highlights the 
     */
    public void Highlight()
    {
        Renderer rend = this.GetComponentInChildren<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = mats[(int) Mat.GLOW];

        Light light = this.GetComponent<Light>();
        light.enabled = true;
    }

    /** Function:   Unhighlight()
     *  Argument:   None
     *  Output:     Unhighlights this piece and reverts it back to its original material
     */
    public void Unhighlight()
    {
        Renderer rend = this.GetComponentInChildren<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = mats[(int) Mat.ORIGINAL];

        Light light = this.GetComponent<Light>();
        light.enabled = false;
    }

    // Override move set in each individual piece's class
    public virtual Hashtable PossibleMoves()
    {
        return new Hashtable();
    }

    /** Function:   GoToJump(Vec3)
     *  Argument:   Vec3 destination - the exact coordinates (not board coordinates) in the world 
     *                                 to jump to
     *  Output:     This piece will jump to the destination 
     */
    public abstract void GoToJump(Vector3 destination);

    /** Function:   GoToAttack(Vec3)
     *  Argument:   Vec3 destination - the exact coordinates (not board coordinates) in the world 
     *                                 to jump to
     *  Output:     This piece will go to the destination using the attack animation
     */
    public abstract void GoToAttack(Vector3 destination);
}
