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
    #region Public Macros
    public enum Mat {ORIGINAL, GLOW }       /* An enum for the indecies of the mats array field */
    public enum Rank { PAWN, KNIGHT, BISHOP, ROOK, QUEEN, KING } /* The possible identity chess piece roles */
    public const float JUMP_DELAY = 0.1f;   /* The time needed for any piece to finish winding for a jump before 
                                               entering the air */
    #endregion

    #region Private fields
    protected Vector3 destination;      /* For tile transitions, this is the destination of the new space */
    protected Animator animator;        /* This piece's animator */
    #endregion

    #region Public fields
    public Rank role;                   /* The role of this piece */
    public bool isWhite;                /* Boolean to determine color of this piece */
    public Material[] mats;             /* Holds the different materials possible for the chesspiece
                                            original material (indx:0), GLOW (indx:1) */
    #endregion

    #region Highlighting Functions
    /** Function:   Highlight()
     *  Argument:   None
     *  Output:     Highlights the piece (when piece is selected)
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
     *  Output:     Unhighlights this piece (when piece is unselected)
     */
    public void Unhighlight()
    {
        Renderer rend = this.GetComponentInChildren<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = mats[(int) Mat.ORIGINAL];

        Light light = this.GetComponent<Light>();
        light.enabled = false;
    }
    #endregion

    /** Function: PossibleMove()
     *  Override move set in each individual piece's class. Each piece should return a Hashtable containing
     *  all the possible moves this piece can currently make from its position and taking into account items
     *  in play and the surroundings
     */
    public virtual Hashtable PossibleMoves()    { return new Hashtable(); }

    #region Transition Animations
    /** Function:   GoToJump(Vec3)
     *  Argument:   Vec3 destination - the exact coordinates (not board coordinates) in the world 
     *                                 to jump to
     *  Output:     This piece will jump to the destination. Override for pieces that have limited
     *              movement schemes (i.e. King, Knight, Pawn)
     */
    public virtual void GoToJump(Vector3 dest)  { return; }

    /** Function:   GoToSlide(Vec3)
     *  Argument:   Vec3 destination - the exact coordinates (not board coordinates) in the world to
     *                                 slide to
     *  Output:     This piece will slide to the destination. Override for pieces that have unlimited
     *              movement schemes (i.e. Queen, Rook, and Bishop)
     */
    public virtual void GoToSlide(Vector3 dest) { return; }

    /** Function:   GoToAttack(Vec3)
     *  Argument:   Vec3 destination - the exact coordinates (not board coordinates) in the world 
     *                                 to jump to
     *  Output:     This piece will go to the destination using the attack animation
     */
    public abstract void GoToAttack(Vector3 destination);
    #endregion

    /** Function:   GoTo(Vec3)
     *  Argument:   Vec3 destination - the exact coordinates for this chesspiece to go to
     *  Output:     Chooses the appropriate movement animation function based on whether the selected piece is
     *              a piece with restricted movement or unrestricted movement and executes the corresponding
     *              GoTo___(Vec3) function with the argument Vec3 destination
     */
    public void GoTo(Vector3 destination)
    {
        switch (this.role)
        {
            case Chesspiece.Rank.PAWN:
            case Chesspiece.Rank.KNIGHT:
            case Chesspiece.Rank.KING:
                this.GoToJump(destination);
                break;
            case Chesspiece.Rank.BISHOP:
            case Chesspiece.Rank.ROOK:
            case Chesspiece.Rank.QUEEN:
                this.GoToSlide(destination);
                break;
        }
    }

    /** Function:   FocusCam()
     *  Argument:   None
     *  Output:     Focuses the camera on this piece with the given camera behavior
     */
    public void FocusCam(CameraFollow.Behavior mode)
    {
        CameraFollow.Instance.mode = mode;
        CameraFollow.Instance.target = this.GetComponentInParent<Transform>();
    }

    /** Function:   FocusCam()
     *  Argument:   None
     *  Output:     Unfocuses the camera from this piece
     */
    public void UnfocusCam()
    {
        CameraFollow.Instance.target = null;
    }

    // A delay function used to time jumps when using the GoToJump() method to move
    protected IEnumerator DelayedTransition(Vector3 target)
    {
        yield return new WaitForSeconds(JUMP_DELAY);
        destination = target;
    }
}
