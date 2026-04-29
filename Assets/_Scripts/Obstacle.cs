using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Vector2 startPos;
    private float speed;
    private float playerPositionX;
    private void OnEnable()
    {
        bMove = true;
        hasSurpassed = false;
    }
    public void SetRandomLocation()
    {
        int offsetY = UnityEngine.Random.Range(2, 6);
        if (UnityEngine.Random.value > 0.5f) offsetY = -offsetY;
        transform.position = new Vector2(startPos.x, offsetY);
        //Debug.Log($"Set Random Position at {transform.position}");
    }
    public void SetPlayerPositionX(float x)
    { 
        playerPositionX = x;
    }
    public void SetSpeed(float amount)
    { 
        speed = amount;
    }

    public void SetStartPos(Vector2 pos)
    { 
        startPos = pos;
    }

    private bool bMove;
    private void Move()
    {
        if (bMove)
        { 
            Vector2 direction = Vector2.left;
            transform.position += (Vector3)(direction * speed * Time.deltaTime); 
        }
    }

    public void Stop()
    { 
        bMove = false;
    }

    private void Update()
    {
        Move();
        CheckSurpassed();
    }

    public event Action OnSurpassed;

    private bool hasSurpassed;
    private void CheckSurpassed()
    {
        if (!hasSurpassed && transform.position.x < playerPositionX)
        { 
            OnSurpassed?.Invoke();
            hasSurpassed = true;
        }
    }
    public event Action OnCollided;
    public event Action<GameObject> OnRecycled;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        { 
            OnCollided?.Invoke();
        }

        if (collision.gameObject.CompareTag("RecycleGround"))
        { 
            OnRecycled?.Invoke(this.gameObject);
        }
    }
}
