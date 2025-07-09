using System.Collections;
using UnityEngine;

public class CombateJugador : MonoBehaviour
{
    [Header("Daño Cuerpo a Cuerpo")]
    [SerializeField] private Transform controladorAtaque;
    [SerializeField] private float radioAtaque = 1f;
    [SerializeField] private int dañoAtaqueM = 1;
    [SerializeField] private float tiempoEntreAtaques = 0.5f;
    private float tiempoUltimoAtaque;

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private MovimientoPlayer movimientoJugador;

    [Header("Daño Lanzable")]
    [SerializeField] private GameObject prefabProyectil;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private int dañoProyectil = 1;
    [SerializeField] private int maxProyectilesTotales = 5;

    [Header("Cooldown Lanzables")]
    [SerializeField] private int proyectilesAntesCooldown = 3;
    [SerializeField] private float tiempoCooldownLanzables = 3f;

    private int proyectilesLanzadosTotales;
    private int proyectilesLanzadosEnRonda;
    private bool enCooldownLanzables = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            IntentarAtacar();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            IntentarAnimacionLanzar();
        }
    }

    private void IntentarAtacar()
    {
        if (Time.time < tiempoUltimoAtaque + tiempoEntreAtaques) return;
        if (movimientoJugador == null) return;

        if (movimientoJugador.EstaDasheando()) return;
        if (!movimientoJugador.PuedeAtacarCuerpoACuerpo()) return;

        Atacar();
    }

    private void Atacar()
    {
        movimientoJugador.InterrumpirDash();
        movimientoJugador.congelado = true;

        animator.SetTrigger("Atacar");
        tiempoUltimoAtaque = Time.time;

        Collider2D[] enemigos = Physics2D.OverlapCircleAll(controladorAtaque.position, radioAtaque);
        foreach (Collider2D enemigo in enemigos)
        {
            if (enemigo.TryGetComponent(out VidaEnemigo vida))
            {
                vida.TomarDaño(dañoAtaqueM);
            }
        }

        if (TryGetComponent(out HInvisibilidad invisibilidad) && invisibilidad.EstaInvisible())
        {
            invisibilidad.CancelarInvisibilidad();
        }
    }

    public void FinalizarAtaque()
    {
        if (movimientoJugador != null)
            movimientoJugador.congelado = false;
    }

    private void IntentarAnimacionLanzar()
    {
        if (enCooldownLanzables || proyectilesLanzadosTotales >= maxProyectilesTotales)
        {
            Debug.Log("No puedes lanzar: cooldown o sin proyectiles.");
            return;
        }

        animator.SetTrigger("Lanzar");

        if (TryGetComponent(out HInvisibilidad invisibilidad) && invisibilidad.EstaInvisible())
        {
            invisibilidad.CancelarInvisibilidad();
        }
    }

    public void LanzarProyectilEvento()
    {
        if (prefabProyectil == null || puntoDisparo == null)
        {
            Debug.LogWarning("Falta asignar prefab del proyectil o punto de disparo en el Inspector.");
            return;
        }

        if (enCooldownLanzables || proyectilesLanzadosTotales >= maxProyectilesTotales)
            return;

        GameObject proyectil = Instantiate(prefabProyectil, puntoDisparo.position, Quaternion.identity);
        Vector2 direccion = transform.right * Mathf.Sign(transform.localScale.x);

        Proyectil scriptProyectil = proyectil.GetComponent<Proyectil>();
        if (scriptProyectil != null)
        {
            scriptProyectil.Inicializar(direccion, dañoProyectil);
        }
        else
        {
            Debug.LogWarning("El proyectil instanciado no tiene un script Proyectil.");
        }

        Collider2D colJugador = GetComponent<Collider2D>();
        Collider2D colProyectil = proyectil.GetComponent<Collider2D>();
        if (colJugador != null && colProyectil != null)
        {
            Physics2D.IgnoreCollision(colJugador, colProyectil);
        }

        proyectilesLanzadosTotales++;
        proyectilesLanzadosEnRonda++;

        if (proyectilesLanzadosEnRonda >= proyectilesAntesCooldown)
        {
            StartCoroutine(CooldownLanzables());
        }
    }

    private IEnumerator CooldownLanzables()
    {
        enCooldownLanzables = true;
        yield return new WaitForSeconds(tiempoCooldownLanzables);
        proyectilesLanzadosEnRonda = 0;
        proyectilesLanzadosTotales = 0;
        enCooldownLanzables = false;
    }

    public void RecargarProyectiles()
    {
        proyectilesLanzadosTotales = 0;
        proyectilesLanzadosEnRonda = 0;
        enCooldownLanzables = false;
    }

    private void OnDrawGizmos()
    {
        if (controladorAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(controladorAtaque.position, radioAtaque);
        }
    }
}