using UnityEngine;

public class TileCollisionManager : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public bool isAtBoundary = false;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision enter");
        // Check if the collision is with the background grid's outer edge.
        if (collision.gameObject.CompareTag("Boundary"))
        {
            Debug.Log("boundary collision");
            // Set the flag to indicate that the tile is at the boundary.
            isAtBoundary = true;

            // Stop the tile's movement by freezing its Rigidbody.
            rb2d.velocity = Vector2.zero;
        }

        // Check if the collision is with another movable tile.
        if (collision.gameObject.CompareTag("MovableTile"))
        {
            Debug.Log("tile collision");
            // Stop the tile's movement by freezing its Rigidbody.
            rb2d.velocity = Vector2.zero;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("collision exit");
        // Check if the tile has exited the boundary.
        if (collision.gameObject.CompareTag("Boundary"))
        {
            // Reset the flag since the tile is no longer at the boundary.
            isAtBoundary = false;
        }
    }

    // Add a method to check if the tile is at the boundary.
    public bool IsAtBoundary()
    {
        return isAtBoundary;
    }

    public bool IsCollidingWithTile()
    {
        // Check if the tile is colliding with another movable tile.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f); // Adjust the radius as needed.

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("MovableTile") && collider.gameObject != gameObject)
            {
                return true; // Colliding with another tile.
            }
        }

        return false; // Not colliding with any tile.
    }

}


