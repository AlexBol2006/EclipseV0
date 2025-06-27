using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MovimientoPlayer : MonoBehaviour
{
    private const string STRING_VELOCIDAD_HORIZONTAL = "VelocidadHorizontal";
    [Header("Referencia")]
    public Rigidbody2D rb;
    [SerializeField] private Animator animator;


    [Header("Movimiento")]
    public float velocidadCaminar;
    public float velocidadCorrer;
    private float velocidadActual;
    private float movX;
    private bool estaCorriendo = true;

    [Header("Salto")]
    public float fuerzaSalto;
    public LayerMask entorno;
    [SerializeField] private bool enSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector2 dimensionesCaja;

    [Header("Salto Pared")]
    public float fuerzaSaltoPared = 3f;
    [SerializeField] private Vector2 dimensionesCajaP;
    public float longitudDeteccion;
    public float fuerzaHorSalto;
    public Transform puntoDeteccionDerecha;
    public Transform puntoDeteccionIzquierda;
    bool xD = false;
    [SerializeField] float maxYVelocity = 0.2f;
    private bool tocandoPared;

    [Header("Dash")]
    public float DashCooldown;
    public float dashForce = 20;
    bool canDash, isDashing = false;
    float dashingTime = 0.2f;




    void Start()
    {
        canDash = true;
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        //MovimientoHorizontal

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            estaCorriendo = !estaCorriendo;
        }

        if (estaCorriendo)
        {
            velocidadActual = velocidadCorrer;
        }
        else
        {
            velocidadActual = velocidadCaminar;
        }

        movX = Input.GetAxis("Horizontal");
        velocidadActual = movX * velocidadActual;


        //Salto 
        if (movX < 0 && EnParedL() || movX > 0 && EnParedR())
        {
            velocidadActual = 0;
        }

        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, entorno);

        if (EnParedR() && rb.linearVelocityY < 0 && movX > 0 || EnParedL() && rb.linearVelocityY < 0 && movX < 0)
        {
            rb.gravityScale = 0f;
            rb.linearVelocityY = -0.1f;
            transform.Translate(Vector3.down * 1 * Time.deltaTime);

            tocandoPared = true;

        }
        else
        {
            rb.gravityScale = 1;
            tocandoPared = false;
        }

        rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -6, maxYVelocity);

        if (enSuelo && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);

        }
        else if (EnParedR() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(-fuerzaHorSalto, fuerzaSaltoPared), ForceMode2D.Impulse);
            StartCoroutine(XD());
        }
        else if (EnParedL() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(fuerzaHorSalto, fuerzaSaltoPared), ForceMode2D.Impulse);
            StartCoroutine(XD());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        MirarDireccionMovimiento();
        ControladorAnimaciones();
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        if (movX != 0)
        {
            rb.linearVelocity = new Vector2(xD ? rb.linearVelocityX : velocidadActual, rb.linearVelocity.y);
        }
    }
    bool EnParedR()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.right * (dimensionesCajaP.x / 2 + longitudDeteccion);
        return Physics2D.OverlapBox(origin, dimensionesCajaP, 0f, entorno);
    }
    bool EnParedL()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.left * (dimensionesCajaP.x / 2 + longitudDeteccion);
        return Physics2D.OverlapBox(origin, dimensionesCajaP, 0f, entorno);
    }
    IEnumerator XD()
    {
        xD = true;
        yield return new WaitForSeconds(0.1f);
        xD = false;
    }
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.eulerAngles.y == 0 ? 1 : -1, 0) * dashForce;
        yield return new WaitForSeconds(dashingTime);
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(DashCooldown);
        canDash = true;
    }

    private void MirarDireccionMovimiento()
    {
        if ((movX > 0 && !MirarDerecha()) || (movX < 0 && MirarDerecha()))
        {
            GirarE();
        }
    }
    private bool MirarDerecha()
    {
        return transform.eulerAngles.y == 0;
    }

    private void GirarE()
    {
        Vector3 rotacion = transform.eulerAngles;
        rotacion.y = rotacion.y == 0 ? 180 : 0;
        transform.eulerAngles = rotacion;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);

        Gizmos.color = Color.blue;
        Vector2 rightOrigin = (Vector2)transform.position + Vector2.right * (dimensionesCajaP.x / 2 + longitudDeteccion);
        Gizmos.DrawWireCube(rightOrigin, dimensionesCajaP);

        Gizmos.color = Color.green;
        Vector2 leftOrigin = (Vector2)transform.position + Vector2.left * (dimensionesCajaP.x / 2 + longitudDeteccion);
        Gizmos.DrawWireCube(leftOrigin, dimensionesCajaP);
    }
    private void ControladorAnimaciones()
    {
        animator.SetFloat(STRING_VELOCIDAD_HORIZONTAL, Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("Caminar", !estaCorriendo);
        animator.SetBool("Dash", isDashing);
        animator.SetBool("EnPared",tocandoPared);



    }
}

