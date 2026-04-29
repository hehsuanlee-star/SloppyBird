using System;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public float jumpForce;
    private Rigidbody2D _rb;
    private Vector2 startPos;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        SetOirigin(transform.position);
    }

    private void Start()
    {
    }
    public void StartMotion()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic; 
        isPaused = false;
    }

    private bool isPaused;
    public void PauseMotion()
    {
        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Static;
        isPaused = true;
    }
    public void SetOirigin(Vector2 position)
    {
        startPos = position;
    }

    public void ReturnOrigin()
    { 
        transform.position = startPos;
    }
    private void HandleJump()
    {
        if (!isPaused && Input.GetMouseButtonDown(0))
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        }
    }
    // Update is called once per frame
    void Update()
    {
        HandleJump();
        //Debug.Log($"{_rb.bodyType}");
    }

    public event Action OnTriggeredDead;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DeadZone"))
        {
            OnTriggeredDead?.Invoke();
        }
    }
}
