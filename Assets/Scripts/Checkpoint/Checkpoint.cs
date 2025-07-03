using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Guardar checkpoint
            ControladorJuego.instance.ActivarCheckpoint(transform.position);

            // Restaurar vida
            VidaJugador vida = other.GetComponent<VidaJugador>();
            if (vida != null)
            {
                vida.RestaurarVidaTotal();     // ✅ Vida completa
                vida.ResetearCuraciones();     // ✅ Curaciones restauradas (si lo usas)
            }

            // Restaurar proyectiles (opcional)
            CombateJugador combate = other.GetComponent<CombateJugador>();
            if (combate != null)
            {
                combate.RecargarProyectiles(); // ✅ Proyectiles recargados
            }
        }
    }
}
