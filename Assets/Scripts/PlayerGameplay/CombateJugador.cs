using System.Collections;
using UnityEngine;

public class CombateJugador : MonoBehaviour
{
    [Header("Daño Cuerpo a Cuerpo")]
    [SerializeField] private Transform controladorAtaque;
    [SerializeField] private float radioAtaque;
    [SerializeField] private int dañoAtaqueM;
    [SerializeField] private float tiempoEntreAtaques;
    private float tiempoUltimoAtaques;

    [Header("Referencias")]
    [SerializeField] private Animator animator;

    [Header("Daño Lanzable")]
    [SerializeField] private GameObject prefabProyectil;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private int dañoProyectil;
    [SerializeField] private int maxProyectilesTotales ;

    [Header("Cooldown Lanzables")]
    [SerializeField] private int proyectilesAntesCooldown ;
    [SerializeField] private float tiempoCooldownLanzables ;

    private int proyectilesLanzadosTotales ;
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
            LanzarProyectil();
        }
    }

    private void IntentarAtacar()
    {
        if (Time.time < tiempoUltimoAtaques + tiempoEntreAtaques)
            return;

        Atacar();
    }

    private void Atacar()
    {
        animator.SetTrigger("Atacar");
        tiempoUltimoAtaques = Time.time;

        Collider2D[] objetosTocados = Physics2D.OverlapCircleAll(controladorAtaque.position, radioAtaque);
        foreach (Collider2D objeto in objetosTocados)
        {
            if (objeto.TryGetComponent(out VidaEnemigo vidaEnemigo))
            {
                vidaEnemigo.TomarDaño(dañoAtaqueM);
            }
        }
    }

    private void LanzarProyectil()
    {
        if (enCooldownLanzables)
        {
            Debug.Log("Esperando cooldown para volver a lanzar proyectiles...");
            return;
        }

        if (proyectilesLanzadosTotales >= maxProyectilesTotales)
        {
            Debug.Log("¡Ya lanzaste todos los proyectiles disponibles!");
            return;
        }

        GameObject proyectil = Instantiate(prefabProyectil, puntoDisparo.position, Quaternion.identity);
        Vector2 direccion = transform.right * Mathf.Sign(transform.localScale.x);

        Proyectil scriptProyectil = proyectil.GetComponent<Proyectil>();
        if (scriptProyectil != null)
        {
            scriptProyectil.Inicializar(direccion, dañoProyectil);
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
        Debug.Log("Esperando " + tiempoCooldownLanzables + " segundos de cooldown...");
        yield return new WaitForSeconds(tiempoCooldownLanzables);

        proyectilesLanzadosEnRonda = 0;
        proyectilesLanzadosTotales = 0; // ✅ Resetea proyectiles disponibles
        enCooldownLanzables = false;

        Debug.Log("Cooldown finalizado. Puedes lanzar nuevamente.");
    }
    public void RecargarProyectiles()
    {
        proyectilesLanzadosTotales = 0;
        proyectilesLanzadosEnRonda = 0;
        enCooldownLanzables = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorAtaque.position, radioAtaque);
    }
}
