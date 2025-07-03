using UnityEngine;

public class AtaqueEnemigo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;

    [Header("Ataque")]
    [SerializeField] private float radioAtaque;
    [SerializeField] private int dañoAtaqueM;
    [SerializeField] private float tiempoEntreAtaques;
    [SerializeField] private LayerMask capaJugador;

    private float tiempoUltimoAtaque;
    private EnemiControlador controlador;

    private void Start()
    {
        controlador = GetComponent<EnemiControlador>();
    }

    private void Update()
    {
        if (controlador == null || controlador.Jugador == null) return;

        float distancia = Vector2.Distance(transform.position, controlador.Jugador.position);

        if (distancia <= controlador.DistanciaDetenerse)
        {
            IntentarAtacar();
        }
    }

    private void IntentarAtacar()
    {
        if (Time.time < tiempoUltimoAtaque + tiempoEntreAtaques)
            return;

        Atacar();
    }

    private void Atacar()
    {
        animator.SetTrigger("Atacar");
        tiempoUltimoAtaque = Time.time;

        // El daño se aplica luego desde el evento en la animación con: AnimEvent → AplicarDaño()
    }

    // Llamada desde el evento de animación
    public void AplicarDaño()
    {
        Collider2D[] objetosTocados = Physics2D.OverlapCircleAll(transform.position, radioAtaque, capaJugador);

        foreach (Collider2D obj in objetosTocados)
        {
            if (obj.TryGetComponent(out VidaJugador vida))
            {
                vida.TomarDaño(dañoAtaqueM);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioAtaque);
    }
}
