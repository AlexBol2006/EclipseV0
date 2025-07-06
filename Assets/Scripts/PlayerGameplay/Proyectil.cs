using UnityEngine;

public class Proyectil : MonoBehaviour
{
    private Vector2 direccion;
    private int daño;
    public float velocidad = 10f;
    public float tiempoVida = 5f;

    public void Inicializar(Vector2 direccion, int daño)
    {
        this.direccion = direccion.normalized;
        this.daño = daño;
        Destroy(gameObject, tiempoVida);
    }

    private void Update()
    {
        transform.Translate(direccion * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
