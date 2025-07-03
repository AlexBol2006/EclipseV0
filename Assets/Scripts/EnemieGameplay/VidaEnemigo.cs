using System;
using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    [SerializeField] private int vidaMaxima;
    [SerializeField] private int vidaActual;

    [Header("Animación de muerte")]
    [SerializeField] private Animator animator;
    [SerializeField] private float tiempoAntesDeDestruir = 1.2f;

    private bool estaMuerto = false;

    private void Awake()
    {
        vidaActual = vidaMaxima;

        // Si no se asignó desde el inspector, lo busca automáticamente
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void TomarDaño(int cantidadDaño)
    {
        if (estaMuerto) return;

        vidaActual -= cantidadDaño;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        // 🔴 Activar animación de daño inmediatamente
        if (animator != null)
        {
            animator.SetTrigger("Daño");
        }

        if (vidaActual == 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        estaMuerto = true;

        if (animator != null)
        {
            animator.SetTrigger("Muerte");
        }

        // Desactivar colisión y scripts de movimiento
        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this) script.enabled = false;
        }

        // Destruir después del tiempo que dura la animación
        Destroy(gameObject, tiempoAntesDeDestruir);
    }
}
