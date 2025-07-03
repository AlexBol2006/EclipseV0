using UnityEngine;

public class HInvisibilidad : MonoBehaviour
{
    [Header("Invisibilidad")]
    [SerializeField] private float duracionInvisibilidad ;
    [SerializeField] private float cooldownInvisibilidad ;

    private float tiempoUltimoUso;
    private bool esInvisible = false;
    private SpriteRenderer sr;
    private VidaJugador vidaJugador; // Referencia para invulnerabilidad si la usas
    private EnemiControlador[] enemigos; // Solo si quieres que los enemigos ignoren al jugador

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        vidaJugador = GetComponent<VidaJugador>();
        enemigos = Object.FindObjectsByType<EnemiControlador>(FindObjectsSortMode.None);

        tiempoUltimoUso = -cooldownInvisibilidad;

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F) && Time.time >= tiempoUltimoUso + cooldownInvisibilidad)
        {
            ActivarInvisibilidad();
        }

        // Interrupciones (agrega tus condiciones específicas aquí)
        if (esInvisible && (EstáAtacando() || EstáLanzando() || EstáHaciendoDash()))
        {
            CancelarInvisibilidad();
        }
    }

    private void ActivarInvisibilidad()
    {
        esInvisible = true;
        tiempoUltimoUso = Time.time;

        sr.color = new Color(1, 1, 1, 0.4f);

        if (vidaJugador != null)
            vidaJugador.ActivarInvulnerabilidad(true);

        Invoke(nameof(CancelarInvisibilidad), duracionInvisibilidad);
    }

    public void CancelarInvisibilidad()
    {
        if (!esInvisible) return;

        esInvisible = false;

        // Visual (normal)
        sr.color = new Color(1, 1, 1, 1f);

       

        if (vidaJugador != null)
            vidaJugador.ActivarInvulnerabilidad(false);
    }
    public bool EstaInvisible()
    {
        return esInvisible;
    }

    // Puedes reemplazar estos con tus propias condiciones
    private bool EstáAtacando()
    {
        return Input.GetKey(KeyCode.Mouse0);
    }

    private bool EstáLanzando()
    {
        return Input.GetKey(KeyCode.Q);
    }

    private bool EstáHaciendoDash()
    {
        return Input.GetKey(KeyCode.LeftShift); // O tu tecla real de dash
    }
}
