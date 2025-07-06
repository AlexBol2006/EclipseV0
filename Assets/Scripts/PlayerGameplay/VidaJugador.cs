using UnityEngine;

public class VidaJugador : MonoBehaviour
{
    [Header("Curación")]
    [SerializeField] private int usosCuracionMaximos;
    private int usosCuracionRestantes;

    [SerializeField] private int vidaMaxima;
    [SerializeField] private int vidaActual;
    private bool esInvulnerable = false;

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private UIConsumibleUnico uiCuraciones; // Nueva referencia a la UI de curación con una sola imagen
    [SerializeField] private BarraDeVIdaUI barraDeVIdaUI;

    private void Awake()
    {
        vidaActual = vidaMaxima;
        usosCuracionRestantes = usosCuracionMaximos;

        if (barraDeVIdaUI != null)
            barraDeVIdaUI.InicairBarraDeVida(vidaMaxima, vidaActual);

        // Actualiza la UI al comenzar
        if (uiCuraciones != null)
            uiCuraciones.ActualizarUI(usosCuracionRestantes);
    }

    private void Update()
    {
        if (barraDeVIdaUI != null)
            barraDeVIdaUI.CambiarBarraDeVida(vidaActual);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            IntentarCurar();
        }
    }

    public void TomarDaño(int daño)
    {
        if (esInvulnerable) return;

        vidaActual -= daño;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        if (animator != null)
            animator.SetTrigger("Daño");

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void ActivarInvulnerabilidad(bool estado)
    {
        esInvulnerable = estado;
    }

    public void Morir()
    {
        if (animator != null)
            animator.SetTrigger("Morir");

        transform.position = ControladorJuego.instance.ObtenerCheckpoint();
        vidaActual = vidaMaxima;
        usosCuracionRestantes = usosCuracionMaximos;

        if (uiCuraciones != null)
            uiCuraciones.ActualizarUI(usosCuracionRestantes);

        CombateJugador combate = GetComponent<CombateJugador>();
        if (combate != null)
        {
            combate.RecargarProyectiles();
        }
    }

    public void RestaurarVidaTotal()
    {
        vidaActual = vidaMaxima;
    }

    public void ResetearCuraciones()
    {
        usosCuracionRestantes = usosCuracionMaximos;

        if (uiCuraciones != null)
            uiCuraciones.ActualizarUI(usosCuracionRestantes);
    }

    private void IntentarCurar()
    {
        if (usosCuracionRestantes <= 0 || vidaActual >= vidaMaxima)
        {
            Debug.Log("¡No quedan curaciones o ya tienes vida completa!");
            return;
        }

        int vidaPorCurar = Mathf.RoundToInt(vidaMaxima * 0.7f);
        vidaActual = Mathf.Clamp(vidaActual + vidaPorCurar, 0, vidaMaxima);
        usosCuracionRestantes--;

        if (animator != null)
            animator.SetTrigger("Curar");

        if (uiCuraciones != null)
            uiCuraciones.ActualizarUI(usosCuracionRestantes);

        Debug.Log($"Curado +{vidaPorCurar}. Usos restantes: {usosCuracionRestantes}");
    }

    public void ReiniciarDesdeCheckpoint()
    {
        Vector2 posicion = ControladorJuego.instance.ObtenerCheckpoint();
        transform.position = posicion;
    }
}
