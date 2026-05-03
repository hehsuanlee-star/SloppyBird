using System;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public float jumpForce;
    private Rigidbody2D _rb;
    private Vector2 startPos;
    private MyInputActions _input;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        SetOrigin(transform.position);
        _input = new MyInputActions();
        _input.GamePlay.Jump.performed += _ => TryJump();
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();
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
    public void SetOrigin(Vector2 position)
    {
        startPos = position;
    }

    public void ReturnOrigin()
    { 
        transform.position = startPos;
    }
    //private void HandleJump()
    //{
    //    if (!isPaused && Input.GetMouseButtonDown(0))
    //    {
    //        _rb.linearVelocity = Vector2.zero;
    //        _rb.AddForceY(jumpForce, ForceMode2D.Impulse);
    //    }
    //}

    private void TryJump()
    {
        if (isPaused) return;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForceY(jumpForce, ForceMode2D.Impulse);
    }

    public event Action OnTriggeredDead;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            OnTriggeredDead?.Invoke();
        }
    }
}
