using Unity.Mathematics;
using UnityEngine;

public class VidaJugador : MonoBehaviour
{
    [Header("Curacion")]

    [SerializeField] private int usosCuracionMaximos ;
    private int usosCuracionRestantes;

    [SerializeField] private int vidaMaxima;
    [SerializeField] private int vidaActual;
    private bool esInvulnerable = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            IntentarCurar();
        }
    }
    private void Awake()
    {
        vidaActual = vidaMaxima;
        usosCuracionRestantes = usosCuracionMaximos;
    }
    public void TomarDaño(int daño)
    {
        if (esInvulnerable) return;

        vidaActual -= daño;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

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
        transform.position = ControladorJuego.instance.ObtenerCheckpoint();
        vidaActual = vidaMaxima;

        // Resetear usos de curación
        usosCuracionRestantes = usosCuracionMaximos;

        // Resetear proyectiles si quieres
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
    }
    private void IntentarCurar()
    {
        if (usosCuracionRestantes <= 0)
        {
            Debug.Log("¡No quedan curaciones!");
            return;
        }

        int vidaPorCurar = Mathf.RoundToInt(vidaMaxima * 0.7f);
        vidaActual = Mathf.Clamp(vidaActual + vidaPorCurar, 0, vidaMaxima);
        usosCuracionRestantes--;

        Debug.Log($"Curado +{vidaPorCurar}. Usos restantes: {usosCuracionRestantes}");
    }

    public void ReiniciarDesdeCheckpoint()
    {
        Vector2 posicion = ControladorJuego.instance.ObtenerCheckpoint();
        transform.position = posicion;
        // Resetear vida, estado, animación, etc. si hace falta
    }
}

