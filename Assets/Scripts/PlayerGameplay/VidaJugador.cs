using System.Collections;
using UnityEngine;

public class VidaJugador : MonoBehaviour
{
    [Header("Curación")]
    [SerializeField] private int usosCuracionMaximos;
    private int usosCuracionRestantes;

    [SerializeField] private int vidaMaxima;
    [SerializeField] private int vidaActual;
    private bool esInvulnerable = false;
    private bool estaMuerto = false;

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private UIConsumibleUnico uiCuraciones;
    [SerializeField] private BarraDeVIdaUI barraDeVIdaUI;

    private void Awake()
    {
        vidaActual = vidaMaxima;
        usosCuracionRestantes = usosCuracionMaximos;

        if (barraDeVIdaUI != null)
            barraDeVIdaUI.InicairBarraDeVida(vidaMaxima, vidaActual);

        if (uiCuraciones != null)
            uiCuraciones.ActualizarUI(usosCuracionRestantes);
    }

    private void Update()
    {
        if (estaMuerto) return;

        if (barraDeVIdaUI != null)
            barraDeVIdaUI.CambiarBarraDeVida(vidaActual);

        if (Input.GetKeyDown(KeyCode.Tab))
            IntentarCurar();
    }

    public void TomarDaño(int daño)
    {
        if (esInvulnerable || estaMuerto) return;

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
        if (estaMuerto) return;

        estaMuerto = true;

        if (animator != null)
            animator.SetTrigger("Morir");
    }

    // Este método se llama desde el evento en la animación "Muerte"
    public void EventoFinAnimacionMuerte()
    {
        transform.position = ControladorJuego.instance.ObtenerCheckpoint();

        vidaActual = vidaMaxima;
        usosCuracionRestantes = usosCuracionMaximos;
        estaMuerto = false;

        if (uiCuraciones != null)
            uiCuraciones.ActualizarUI(usosCuracionRestantes);

        if (barraDeVIdaUI != null)
            barraDeVIdaUI.CambiarBarraDeVida(vidaActual);

        GetComponent<CombateJugador>()?.RecargarProyectiles();

        // ✅ Forzar estado "Movimiento" del Blend Tree
        if (animator != null)
            animator.Play("Movimiento");
    }



    public bool EstaMuerto() => estaMuerto;

    private void IntentarCurar()
    {
        if (usosCuracionRestantes <= 0 || vidaActual >= vidaMaxima)
        {
            Debug.Log("¡No quedan curaciones o ya tienes vida completa!");
            return;
        }

        MovimientoPlayer movimiento = GetComponent<MovimientoPlayer>();
        if (movimiento != null)
            movimiento.congelado = true;

        int vidaPorCurar = Mathf.RoundToInt(vidaMaxima * 0.7f);
        vidaActual = Mathf.Clamp(vidaActual + vidaPorCurar, 0, vidaMaxima);
        usosCuracionRestantes--;

        if (animator != null)
            animator.SetTrigger("Curar");

        if (uiCuraciones != null)
            uiCuraciones.ActualizarUI(usosCuracionRestantes);

        StartCoroutine(FinCuracion());
    }

    private IEnumerator FinCuracion()
    {
        yield return new WaitForSeconds(1f);

        MovimientoPlayer movimiento = GetComponent<MovimientoPlayer>();
        if (movimiento != null)
        {
            movimiento.congelado = false;
            movimiento.ForzarMovimientoTrasCuracion();
        }
    }

    public void FinAnimacionCurar()
    {
        MovimientoPlayer movimiento = GetComponent<MovimientoPlayer>();
        if (movimiento != null)
            movimiento.congelado = false;
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

    public void ReiniciarDesdeCheckpoint()
    {
        transform.position = ControladorJuego.instance.ObtenerCheckpoint();
    }
}
