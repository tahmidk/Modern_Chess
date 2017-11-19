using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Chesspiece
{
    private const float BASE_SPEED = 7f;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        role = Rank.QUEEN;
        destination = this.transform.position;
    }

    private void Update()
    {
        // If there is a new destination, move towards that destination
        if (transform.position != destination)
        {
            float distance = Vector3.Distance(transform.position, destination);
            if (distance > 3)
                animator.Play("Queen_SLIDE", -1, 0f);
            else
                animator.Play("Queen_SLIDE_STOP", -1, 0f);

            Vector3 start_pos = this.GetComponentInParent<Transform>().position;
            Vector3 direction = destination - start_pos;
            transform.rotation = Quaternion.LookRotation(-1 * direction);
            transform.position = Vector3.MoveTowards(start_pos, destination, BASE_SPEED * Time.deltaTime);
        }
    }

    /** Function:   PossibleMoves (Queen Override)
     *  Argument:   None
     *  Output:     Returns the possible moves of a rook. A queen can normally attack in any of the 8 directions
     *              unrestrictedly. This implementation is based on the fact that a queen has the movability
     *              of both a rook and bishop at the same position 
     */
    public override Hashtable PossibleMoves()
    {
        GameObject temp = new GameObject();
        temp.AddComponent<Rook>();
        temp.AddComponent<Bishop>();

        // Simulate a rook's moves if it were at this queen's position
        Rook tempRook = temp.GetComponent<Rook>();
        tempRook.SetPosition(this.CurrentX, this.CurrentY);
        tempRook.isWhite = this.isWhite;
        Hashtable vertMoves = tempRook.PossibleMoves();

        // Simulate bishop's moves if it were at this queen's position
        Bishop tempBishop = temp.GetComponent<Bishop>();
        tempBishop.SetPosition(this.CurrentX, this.CurrentY);
        tempBishop.isWhite = this.isWhite;
        Hashtable diagMoves = tempBishop.PossibleMoves();

        // Union the above movesets to get the queen's moveset. There will be no
        // Overlap issues as the horizontal and diagonal moves don't intersect
        Hashtable moveset = new Hashtable();
        foreach (DictionaryEntry entry in vertMoves)
            moveset.Add(entry.Key, entry.Value);
        foreach (DictionaryEntry entry in diagMoves)
            moveset.Add(entry.Key, entry.Value);

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
        animator.Play("Queen_SLIDE_BEGIN", -1, 0f);

        // Transition to new position
        StartCoroutine(DelayedTransition(dest));
    }

    public override void GoToAttack(Vector3 dest)
    {
        
    }
}
