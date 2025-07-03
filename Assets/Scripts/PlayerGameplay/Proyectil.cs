using UnityEngine;

public class Proyectil : MonoBehaviour
{
    [SerializeField] private float velocidad ;
    [SerializeField] private int daño ;
    private Vector2 direccion;

    public void Inicializar(Vector2 direccionLanzamiento, int dañoEntrada)
    {
        direccion = direccionLanzamiento.normalized;
        daño = dañoEntrada;
        Destroy(gameObject, 5f); // Se destruye si no impacta en 5s
    }

    void Update()
    {
        transform.Translate(direccion * 10f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out VidaEnemigo enemigo))
        {
            enemigo.TomarDaño(daño);
        }

        Destroy(gameObject);
    }
}
