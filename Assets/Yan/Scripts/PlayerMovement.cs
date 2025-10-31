using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Vector2 movement;
    private Rigidbody2D rbPlayer;

    private void Start()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
        rbPlayer.gravityScale = 0f;
        rbPlayer.freezeRotation = true;
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        rbPlayer.MovePosition(rbPlayer.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}