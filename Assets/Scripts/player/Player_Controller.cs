using UnityEngine;


[SelectionBase]
public class Player_Controller : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float _moveSpeed = 50f;

    [Header("Dependencies")]
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] Animator _animator;
    [SerializeField] SpriteRenderer _spriteRenderer;

    Vector2 moveDir = Vector2.zero;
    public AudioSource audioSource;
    public AudioClip footstepClip;

    
    private void Update()
    {
        moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        //on spacebar set jump trigger for animator
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetTrigger("jump");
        }
    }
    private void FixedUpdate()
    {
        MovementUpdate();
        AnimatorUpdate();
    }

    private void MovementUpdate()
    {
        _rb.linearVelocity = moveDir * _moveSpeed * Time.fixedDeltaTime;
    }
    private void AnimatorUpdate()
    {
        

        if (moveDir.x > 0)
            _spriteRenderer.flipX = false;
        else if (moveDir.x < 0)
            _spriteRenderer.flipX = true;
        //set walking trigger for the animator
        if (moveDir != Vector2.zero && !_animator.GetBool("walking"))
        {
            _animator.SetBool("walking", true);
            //start looping foostep sound
            audioSource.clip = footstepClip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (moveDir == Vector2.zero && _animator.GetBool("walking"))
        {
            _animator.SetBool("walking", false);
            //stop immediately stop footstep sound
            audioSource.Stop();
        }
    }
 
}
