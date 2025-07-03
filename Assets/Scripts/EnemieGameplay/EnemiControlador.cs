using System.Collections;
using UnityEngine;

public class EnemiControlador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;
    private Rigidbody2D rb;

    [Header("Patrullaje")]
    [SerializeField] private Transform[] waypoints;
    private int waypointActual;
    [SerializeField] private float speed;
    [SerializeField] private float tiempoEspera;
    private bool estaEsperando;

    [Header("Persecución")]
    [SerializeField] private Transform jugador;
    public Transform Jugador => jugador;
    [SerializeField] private float speedPersecucion;
    [SerializeField] private float distanciaDetenerse;
    public float DistanciaDetenerse => distanciaDetenerse;

    [Header("Detección")]
    [SerializeField] private Vector2 tamañoDeteccion = new Vector2(4f, 2f);
    [SerializeField] private LayerMask capaJugador;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        GirarHacia(waypoints[waypointActual].position);
    }

    private void Update()
    {
        Collider2D jugadorDetectado = Physics2D.OverlapBox(transform.position, tamañoDeteccion, 0f, capaJugador);

        if (jugadorDetectado == null || JugadorEsInvisible(jugadorDetectado))
        {
            Patrullar();
        }
        else
        {
            Perseguir(jugadorDetectado.transform);
        }

        ControladorAnimacionesEnemigo();
    }

    private void Patrullar()
    {
        if (waypointActual >= waypoints.Length)
            waypointActual = EncontrarWaypointMasCercano();

        Vector2 destino = new Vector2(waypoints[waypointActual].position.x, transform.position.y);

        if (Vector2.Distance(transform.position, destino) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, destino, speed * Time.deltaTime);
            GirarHacia(waypoints[waypointActual].position);
            estaEsperando = false;
        }
        else if (!estaEsperando)
        {
            StartCoroutine(Esperar());
        }
    }

    private void Perseguir(Transform jugadorDetectado)
    {
        jugador = jugadorDetectado;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia > distanciaDetenerse)
        {
            Vector2 destino = new Vector2(jugador.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, destino, speedPersecucion * Time.deltaTime);
            GirarHacia(jugador.position);
            estaEsperando = false;
        }
        else
        {
            estaEsperando = true;
        }
    }

    private bool JugadorEsInvisible(Collider2D jugadorDetectado)
    {
        return jugadorDetectado.TryGetComponent(out HInvisibilidad invis) && invis.EstaInvisible();
    }

    private IEnumerator Esperar()
    {
        estaEsperando = true;
        yield return new WaitForSeconds(tiempoEspera);
        waypointActual = (waypointActual + 1) % waypoints.Length;
        estaEsperando = false;
    }

    private void GirarHacia(Vector2 objetivo)
    {
        transform.rotation = (transform.position.x > objetivo.x)
            ? Quaternion.Euler(0f, 180f, 0f)
            : Quaternion.Euler(0f, 0f, 0f);
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

    public bool IgnorarJugador(Transform jugador)
    {
        if (jugador.TryGetComponent(out HInvisibilidad invis))
        {
            return invis.EstaInvisible();
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, tamañoDeteccion);
    }
}
