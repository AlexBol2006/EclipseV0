using UnityEngine;

public class Proyectil : MonoBehaviour
{
    private Vector2 direccion = Vector2.zero;
    private int daño = 0;
    public float velocidad = 10f;
    public float tiempoVida = 5f;
    private bool inicializado = false;

    public void Inicializar(Vector2 direccion, int daño)
    {
        this.direccion = direccion.normalized;
        this.daño = daño;
        inicializado = true;
        Destroy(gameObject, tiempoVida);
    }

    private void Update()
    {
        if (!inicializado) return;
        transform.Translate(direccion * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!inicializado) return;

        if (collision.TryGetComponent(out VidaEnemigo enemigo))
        {
            enemigo.TomarDaño(daño);
            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}