using System.Collections;
using UnityEngine;

public class MovimientoPlayer : MonoBehaviour
{
    [Header("Referencia")]
    public Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Movimiento")]
    public float velocidadCaminar;
    public float velocidadCorrer;
    private float velocidadActual;
    private float movX;
    private bool estaCorriendo = true;

    public bool congelado = false;
    public bool estaResbalando = false;

    [Header("Salto")]
    public float fuerzaSalto;
    public LayerMask entorno;
    [SerializeField] private bool enSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector2 dimensionesCaja;

    [Header("Salto Pared")]
    public float fuerzaSaltoPared = 3f;
    [SerializeField] private Vector2 dimensionesCajaP = new Vector2(0.2f, 1f);
    [SerializeField] private Transform pivoteLaterales;
    [SerializeField] float maxYlinearVelocity = 0.2f;
    public float longitudDeteccion;
    public float fuerzaHorSalto;
    private bool xD = false;

    [Header("Dash")]
    public float DashCooldown;
    public float dashForce = 20;
    private bool canDash, isDashing = false;
    private float dashingTime = 0.2f;

    private VidaJugador vidaJugador;
    private HInvisibilidad invisibilidad;

    private void Start()
    {
        canDash = true;
        rb = GetComponent<Rigidbody2D>();
        vidaJugador = GetComponent<VidaJugador>();
        invisibilidad = GetComponent<HInvisibilidad>();
    }

    private void Update()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, entorno);

        estaResbalando = false;
        if (!enSuelo &&
            ((EnParedR() && rb.linearVelocity.y < 0 && movX > 0) ||
             (EnParedL() && rb.linearVelocity.y < 0 && movX < 0)))
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -0.5f);
            estaResbalando = true;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        animator.SetBool("Resbalando", estaResbalando);

        if (congelado) return;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (invisibilidad != null && invisibilidad.PuedeActivar())
            {
                estaCorriendo = !estaCorriendo;

                if (!estaCorriendo)
                    invisibilidad.IntentarActivarInvisibilidad();
            }
        }

        velocidadActual = estaCorriendo ? velocidadCorrer : velocidadCaminar;
        movX = Input.GetAxis("Horizontal");
        velocidadActual = movX * velocidadActual;

        if ((movX < 0 && EnParedL()) || (movX > 0 && EnParedR()))
            velocidadActual = 0;

        if (enSuelo && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
            animator.SetTrigger("Saltando");
        }
        else if (EnParedR() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(-fuerzaHorSalto, fuerzaSaltoPared), ForceMode2D.Impulse);
            StartCoroutine(XD());
            animator.SetTrigger("Saltando");
        }
        else if (EnParedL() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(fuerzaHorSalto, fuerzaSaltoPared), ForceMode2D.Impulse);
            StartCoroutine(XD());
            animator.SetTrigger("Saltando");
        }

        if (!enSuelo && !estaResbalando && rb.linearVelocity.y < -0.1f)
        {
            animator.SetBool("Cayendo", true);
        }
        else if (enSuelo)
        {
            animator.SetBool("Cayendo", false);
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -6, maxYlinearVelocity));

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            StartCoroutine(Dash());

        MirarDireccionMovimiento();
        ControladorAnimaciones();
        VerificarCancelacionInvisibilidad();
    }

    private void FixedUpdate()
    {
        if (isDashing || congelado) return;

        if (movX != 0)
        {
            rb.linearVelocity = new Vector2(xD ? rb.linearVelocity.x : velocidadActual, rb.linearVelocity.y);
        }
    }

    private void VerificarCancelacionInvisibilidad()
    {
        if (!invisibilidad.EstaInvisible()) return;

        bool estaMoviendose = Mathf.Abs(movX) > 0.1f;
        bool corrio = estaCorriendo && estaMoviendose;
        bool ataco = Input.GetKeyDown(KeyCode.Mouse0);
        bool lanzo = Input.GetKeyDown(KeyCode.Q);

        if (corrio || ataco || lanzo)
        {
            invisibilidad.CancelarInvisibilidad();
        }
    }

    public void ForzarCorrer()
    {
        estaCorriendo = true;
    }

    bool EnParedR()
    {
        Vector2 origin = (Vector2)pivoteLaterales.position + Vector2.right * (dimensionesCajaP.x / 2 + longitudDeteccion);
        return Physics2D.OverlapBox(origin, dimensionesCajaP, 0f, entorno);
    }

    bool EnParedL()
    {
        Vector2 origin = (Vector2)pivoteLaterales.position + Vector2.left * (dimensionesCajaP.x / 2 + longitudDeteccion);
        return Physics2D.OverlapBox(origin, dimensionesCajaP, 0f, entorno);
    }

    IEnumerator XD()
    {
        xD = true;
        yield return new WaitForSeconds(0.1f);
        xD = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        vidaJugador.ActivarInvulnerabilidad(true);

        float gravedadOriginal = rb.gravityScale;
        rb.gravityScale = 0;

        Vector2 direccion = transform.right * (transform.localScale.x > 0 ? 1 : -1);
        rb.linearVelocity = direccion * dashForce;

        yield return new WaitForSeconds(dashingTime);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = gravedadOriginal;
        isDashing = false;

        vidaJugador.ActivarInvulnerabilidad(false);

        yield return new WaitForSeconds(DashCooldown);
        canDash = true;
    }

    private void MirarDireccionMovimiento()
    {
        if ((movX > 0 && !MirarDerecha()) || (movX < 0 && MirarDerecha()))
            GirarE();
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

    public void InterrumpirDash()
    {
        if (isDashing)
        {
            StopAllCoroutines();
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 1;
            isDashing = false;
            canDash = true;

            vidaJugador.ActivarInvulnerabilidad(false);
            animator.SetBool("Dash", false);
        }
    }

    // ✅ Métodos faltantes (corrigen errores CS1061)
    public bool EstaDasheando() => isDashing;

    public bool PuedeAtacarCuerpoACuerpo() => !estaResbalando;

    public void EventoGolpe()
    {
        if (invisibilidad != null && invisibilidad.EstaInvisible())
            invisibilidad.CancelarInvisibilidad();
    }
    public void ForzarMovimientoTrasCuracion()
    {
        congelado = false;
    }

    private void ControladorAnimaciones()
    {
        animator.SetFloat("Velocidad", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("Caminar", !estaCorriendo);
        animator.SetBool("Dash", isDashing);
        animator.SetBool("EnSuelo", enSuelo);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);

        Gizmos.color = Color.blue;
        Vector2 rightOrigin = (Vector2)pivoteLaterales.position + Vector2.right * (dimensionesCajaP.x / 2 + longitudDeteccion);
        Gizmos.DrawWireCube(rightOrigin, dimensionesCajaP);

        Gizmos.color = Color.green;
        Vector2 leftOrigin = (Vector2)pivoteLaterales.position + Vector2.left * (dimensionesCajaP.x / 2 + longitudDeteccion);
        Gizmos.DrawWireCube(leftOrigin, dimensionesCajaP);
    }
}
