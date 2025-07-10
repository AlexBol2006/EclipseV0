using System.Collections;
using UnityEngine;

public class Centinela : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform puntoVista;
    [SerializeField] private LayerMask capaJugador;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 3;
    private int vidaActual;

    private int indiceActual = 0;
    private bool esperando = false;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float tiempoEspera = 2f;

    [Header("Detección")]
    [SerializeField] private Vector2 tamañoDeteccion = new Vector2(4f, 2f);
    private Transform jugadorDetectado;
    private bool haDetectado = false;

    [Header("Muerte del jugador tras detección")]
    [SerializeField] private float tiempoParaMatar = 4f;
    private Coroutine cuentaRegresiva;

    private void Awake()
    {
        vidaActual = vidaMaxima;
    }

    private void Update()
    {
        DetectarJugador();

        if (jugadorDetectado == null)
        {
            if (haDetectado)
            {
                haDetectado = false;
                animator.SetBool("Detectando", false);
                animator.SetBool("Esperar", false);

                if (cuentaRegresiva != null)
                {
                    StopCoroutine(cuentaRegresiva);
                    cuentaRegresiva = null;
                }
            }

            Patrullar();
        }
        else
        {
            if (!haDetectado)
            {
                haDetectado = true;
                animator.SetBool("Detectando", true);
                animator.SetBool("Esperar", true);
                cuentaRegresiva = StartCoroutine(ContarYEliminarJugador(jugadorDetectado));
            }

            GirarHacia(jugadorDetectado.position);
        }
    }

    private void DetectarJugador()
    {
        Collider2D deteccion = Physics2D.OverlapBox(puntoVista.position, tamañoDeteccion, 0f, capaJugador);

        if (deteccion != null && !JugadorEsInvisible(deteccion))
        {
            jugadorDetectado = deteccion.transform;
        }
        else
        {
            jugadorDetectado = null;
        }
    }

    private bool JugadorEsInvisible(Collider2D col)
    {
        return col.TryGetComponent(out HInvisibilidad invis) && invis.EstaInvisible();
    }

    private void Patrullar()
    {
        if (waypoints.Length == 0 || esperando) return;

        Transform destino = waypoints[indiceActual];
        float distancia = Vector2.Distance(transform.position, destino.position);

        if (distancia > 0.1f)
        {
            animator.SetBool("Esperar", false);
            transform.position = Vector2.MoveTowards(transform.position, destino.position, velocidad * Time.deltaTime);
            GirarHacia(destino.position);
        }
        else
        {
            StartCoroutine(EsperarYPasarSiguiente());
        }
    }

    private IEnumerator EsperarYPasarSiguiente()
    {
        esperando = true;
        animator.SetBool("Esperar", true);
        yield return new WaitForSeconds(tiempoEspera);
        indiceActual = (indiceActual + 1) % waypoints.Length;
        esperando = false;
        animator.SetBool("Esperar", false);
    }

    private IEnumerator ContarYEliminarJugador(Transform jugador)
    {
        float tiempo = 0f;

        while (tiempo < tiempoParaMatar)
        {
            if (jugador == null || Vector2.Distance(puntoVista.position, jugador.position) > tamañoDeteccion.x)
            {
                animator.SetBool("Detectando", false);
                yield break;
            }

            // 🚫 Verificar si se volvió invisible mientras contaba
            if (JugadorEsInvisible(jugador.GetComponent<Collider2D>()))
            {
                animator.SetBool("Detectando", false);
                yield break;
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        if (jugador.TryGetComponent(out VidaJugador vida))
        {
            vida.TomarDaño(999); // Cambia esto si tienes una forma especial de matar al jugador
        }
    }

    private void GirarHacia(Vector2 objetivo)
    {
        transform.rotation = (transform.position.x > objetivo.x)
            ? Quaternion.Euler(0, 180, 0)
            : Quaternion.Euler(0, 0, 0);
    }

    public void TomarDaño(int daño)
    {
        vidaActual -= daño;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        animator.SetTrigger("Muerte");
        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoVista != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(puntoVista.position, tamañoDeteccion);
        }
    }
}
