using System.Collections;
using UnityEngine;

public class EnemiControlador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private AtaqueEnemigo ataque;
    private Rigidbody2D rb;

    [Header("Patrullaje")]
    [SerializeField] private Transform[] waypoints;
    private int waypointActual;
    [SerializeField] private float speed;
    [SerializeField] private float tiempoEspera;
    private bool estaEsperando;
    private Vector2 proximoDestino;

    [Header("Persecución")]
    [SerializeField] private Transform jugador;
    public Transform Jugador => jugador;
    [SerializeField] private float speedPersecucion;
    [SerializeField] private float distanciaDetenerse;
    public float DistanciaDetenerse => distanciaDetenerse;

    [Header("Detección")]
    [SerializeField] private Vector2 tamañoDeteccion = new Vector2(4f, 2f);
    [SerializeField] private LayerMask capaJugador;

    private bool jugadorDetectado = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        proximoDestino = waypoints[waypointActual].position;
        GirarHacia(proximoDestino);
    }

    private void Update()
    {
        Collider2D deteccion = Physics2D.OverlapBox(transform.position, tamañoDeteccion, 0f, capaJugador);

        if (deteccion != null && !JugadorEsInvisible(deteccion))
        {
            jugador = deteccion.transform;
            jugadorDetectado = true;
            Perseguir(jugador);
        }
        else
        {
            if (jugadorDetectado)
            {
                jugadorDetectado = false;
                jugador = null;
                GirarHacia(proximoDestino); // volver a mirar al destino patrullaje
            }

            Patrullar();
        }

        ControladorAnimacionesEnemigo();
    }

    private void Patrullar()
    {
        if (waypointActual >= waypoints.Length)
            waypointActual = EncontrarWaypointMasCercano();

        Vector2 destino = new Vector2(waypoints[waypointActual].position.x, transform.position.y);
        proximoDestino = waypoints[waypointActual].position;

        if (Vector2.Distance(transform.position, destino) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, destino, speed * Time.deltaTime);

            // Solo girar si no está atacando ni persiguiendo
            if (!ataque.Atacando && !jugadorDetectado && !estaEsperando)
                GirarHacia(proximoDestino);

            estaEsperando = false;
        }
        else if (!estaEsperando)
        {
            StartCoroutine(EsperarYPasarSiguiente());
        }
    }

    private IEnumerator EsperarYPasarSiguiente()
    {
        estaEsperando = true;
        animator.SetBool("estaEsperando", true);

        yield return new WaitForSeconds(tiempoEspera);

        waypointActual = (waypointActual + 1) % waypoints.Length;
        proximoDestino = waypoints[waypointActual].position;
        GirarHacia(proximoDestino);

        estaEsperando = false;
        animator.SetBool("estaEsperando", false);
    }

    private void Perseguir(Transform jugadorDetectado)
    {
        float distancia = Vector2.Distance(transform.position, jugadorDetectado.position);

        if (distancia > distanciaDetenerse)
        {
            Vector2 destino = new Vector2(jugadorDetectado.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, destino, speedPersecucion * Time.deltaTime);
            GirarHacia(jugadorDetectado.position);
            estaEsperando = false;
        }
        else
        {
            estaEsperando = true;
        }
    }

    private void GirarHacia(Vector2 objetivo)
    {
        if (transform.position.x > objetivo.x)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private bool JugadorEsInvisible(Collider2D col)
    {
        return col.TryGetComponent(out HInvisibilidad invis) && invis.EstaInvisible();
    }

    private int EncontrarWaypointMasCercano()
    {
        int indice = 0;
        float menorDistancia = Mathf.Infinity;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float distancia = Vector2.Distance(transform.position, waypoints[i].position);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                indice = i;
            }
        }

        return indice;
    }

    private void ControladorAnimacionesEnemigo()
    {
        animator.SetBool("estaEsperando", estaEsperando);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, tamañoDeteccion);
    }
}
