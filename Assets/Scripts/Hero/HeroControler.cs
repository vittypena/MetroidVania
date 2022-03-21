using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControler : MonoBehaviour
{
    [Header("Attack Variables")]
    [SerializeField] SwordController swordController;

    [Header("Animation Variables")]
    [SerializeField] AnimatorController animatorController;

    [Header("Checked Variables")]
    [SerializeField] LayerChecker footA;
    [SerializeField] LayerChecker footB;

    [Header("Boolean Variables")]
    public bool playerIsAttacking;
    public bool canDoubleJump;
    private bool playerIsOnGround;

    [Header("Control Variables")]
    private bool jumpPressed = false;//Cuando se salte es true
    private bool attackPressed = false;

    [Header("Interruption Variables")]
    public bool canCheckGround;
    public bool canMove;

    [Header("Rigid Variables")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float speed;
    [SerializeField] private Vector2 movementDirection;

    
    private Rigidbody2D rigidbody2D;
    

    // Start is called before the first frame update
    void Start()
    {
        canCheckGround = true;
        canMove = true;
        rigidbody2D = GetComponent<Rigidbody2D>();
        animatorController.Play(AnimationId.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        HandleIsGrounding();
        HandleControls();
        HandleMovement();
        HandleFlip();
        HandleJump();
        HandleAtack();
    }

    void HandleIsGrounding()//Comprueba si el personaje esta tocando el suelo
    {
        if (!canCheckGround) return;
        playerIsOnGround = footA.isTouching || footB.isTouching;
    }

    void HandleControls()
    {
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));//Comprobar que se esta pretando las teclas de movimiento
        jumpPressed = Input.GetButtonDown("Jump");
        attackPressed= Input.GetButtonDown("Attack");
    }
    
    void HandleMovement()
    {
        if (!canMove) return;

        rigidbody2D.velocity = new Vector2(movementDirection.x* speed, rigidbody2D.velocity.y);//Mover cuando se pretan las teclas de movimiento

        if (playerIsOnGround) { 
            if (Mathf.Abs(rigidbody2D.velocity.x) > 0)//Cambiar animacion segun se esta quieto o en movimiento
            {
                animatorController.Play(AnimationId.Run);
            }
            else
            {
                animatorController.Play(AnimationId.Idle);
            }
        }
    }

    void HandleFlip()
    {        
        if(rigidbody2D.velocity.magnitude > 0) { //Girar el personaje
            if(rigidbody2D.velocity.x > 0)
            {
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if(rigidbody2D.velocity.x < 0)
            {
                this.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    void HandleJump()
    {
        if(canDoubleJump && jumpPressed && !playerIsOnGround)//Para poder hacer el doble salto
        {
            this.rigidbody2D.velocity = Vector2.zero;
            this.rigidbody2D.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
            canDoubleJump = false; //Para evitar que haga doble salto infinito
        }

        if (jumpPressed && playerIsOnGround)
        {
            this.rigidbody2D.AddForce(Vector2.up* jumpForce, ForceMode2D.Impulse);//Cuando se sale, se añade una fuerza hacia arriba para saltar
            StartCoroutine(HandleJumpAnimation());
            canDoubleJump = true;
        }
    }

    IEnumerator HandleJumpAnimation()//Corutina para que las animaciones del salto y preparar salto no se ejecuten en el mismo segundo si no que vaya una detras de otra
    {
        canCheckGround = false;
        playerIsOnGround = false;
        if (!playerIsAttacking)
        {
            animatorController.Play(AnimationId.PrepareJump);
            yield return new WaitForSeconds(0.3f);//Provoca un retraso de 0.3 seg
        }
        if (!playerIsAttacking) { 
            animatorController.Play(AnimationId.Jump);
        }
        canCheckGround = true;
    }

    void HandleAtack()
    {
        if (attackPressed && !playerIsAttacking)
        {
            animatorController.Play(AnimationId.Attack);
            playerIsAttacking = true;
            swordController.Attack(0.4f);
            StartCoroutine(RestoreAttack());
        }
    }

    IEnumerator RestoreAttack()
    {
        if (playerIsOnGround)
        {
            canMove = false;
        }        
        yield return new WaitForSeconds(0.25f);
        playerIsAttacking = false;
        if (!playerIsOnGround)
        {
            animatorController.Play(AnimationId.Jump);
        }
        canMove = true;
    }
}
