using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    private float mapMinX = -65f;
    private float mapMaxX = 55f;
    private float minY = 2f;
    private float maxY = 22f;
    private float minSpeed = 0.5f;
    private float maxSpeed = 2f;

    private bool moveRight = true;

    private float currentSpeed;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentSpeed = Random.Range(minSpeed, maxSpeed);

        Vector3 pos = transform.position;
        pos.y = Random.Range(minY, maxY);
        transform.position = pos;
    }

    void Update()
    {
        float direction = moveRight ? 1f : -1f;
        transform.Translate(Vector3.right * direction * currentSpeed * Time.deltaTime);

        if (moveRight && transform.position.x > mapMaxX)
        {
            ResetCloud(true);
        }
        else if (!moveRight && transform.position.x < mapMinX)
        {
            ResetCloud(false);
        }
    }

    void ResetCloud(bool wasMovingRight)
    {
        Vector3 newPos = transform.position;
        newPos.x = wasMovingRight ? mapMinX : mapMaxX;
        newPos.y = Random.Range(minY, maxY);
        transform.position = newPos;
        currentSpeed = Random.Range(minSpeed, maxSpeed);

    }

    public void SetDirection(bool movingRight)
    {
        moveRight = movingRight;
    }
}