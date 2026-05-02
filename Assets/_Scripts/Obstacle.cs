using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Vector2 startPos;
    private float speed;
    private float playerXPos;
    private void OnEnable()
    {
        bMove = true;
        hasSurpassed = false;
    }

    public void ResetSpeedandLocation(float speed)
    { 
        SetSpeed(speed);
        SetRandomLocation();
    }

    public void SetStartPosandPlayerXPos(Vector2 startPos, float playerX)
    {
        SetStartPos(startPos);
        SetPlayerXPos(playerX);
    }

    private void SetRandomLocation()
    {
        int offsetY = UnityEngine.Random.Range(2, 6);
        if (UnityEngine.Random.value > 0.5f) offsetY = -offsetY;
        transform.position = new Vector2(startPos.x, offsetY);
        //Debug.Log($"Set Random Position at {transform.position}");
    }
    private void SetPlayerXPos(float x)
    { 
        playerXPos = x;
    }
    private void SetSpeed(float amount)
    { 
        speed = amount;
    }

    private void SetStartPos(Vector2 pos)
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
        if (!hasSurpassed && transform.position.x < playerXPos)
        { 
            OnSurpassed?.Invoke();
            hasSurpassed = true;
        }
    }
    
    public event Action<GameObject> OnRecycled;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RecycleGround"))
        { 
            OnRecycled?.Invoke(this.gameObject);
        }
    }
}
