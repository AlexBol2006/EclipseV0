using System.Collections;
using UnityEngine;

public class HInvisibilidad : MonoBehaviour
{
    [Header("Invisibilidad")]
    [SerializeField] private float duracionInvisibilidad = 3f;
    [SerializeField] private float cooldownInvisibilidad = 5f;

    [Header("Transparencia")]
    [Range(0f, 1f)]
    [SerializeField] private float nivelTransparencia = 0.3f;
    [SerializeField] private SpriteRenderer cuerpoSprite;

    private bool estaInvisible = false;
    private float tiempoUltimoUso;
    private MovimientoPlayer movimiento;
    private Color colorOriginal;

    private void Start()
    {
        movimiento = GetComponent<MovimientoPlayer>();

        // Obtener el SpriteRenderer si no se asignó manualmente
        if (cuerpoSprite == null)
            cuerpoSprite = GetComponent<SpriteRenderer>();

        if (cuerpoSprite != null)
            colorOriginal = cuerpoSprite.color;
    }

    public void IntentarActivarInvisibilidad()
    {
        if (estaInvisible || Time.time < tiempoUltimoUso + cooldownInvisibilidad)
            return;

        ActivarInvisibilidad();
    }

    private void ActivarInvisibilidad()
    {
        estaInvisible = true;
        tiempoUltimoUso = Time.time;

        if (cuerpoSprite != null)
        {
            Color c = cuerpoSprite.color;
            c.a = nivelTransparencia;
            cuerpoSprite.color = c;
        }

        StartCoroutine(DesactivarDespuesDeTiempo());
    }

    private IEnumerator DesactivarDespuesDeTiempo()
    {
        yield return new WaitForSeconds(duracionInvisibilidad);
        CancelarInvisibilidad();
    }

    public void CancelarInvisibilidad()
    {
        if (!estaInvisible) return;

        estaInvisible = false;

        if (cuerpoSprite != null)
        {
            Color c = cuerpoSprite.color;
            c.a = colorOriginal.a;
            cuerpoSprite.color = c;
        }

        if (movimiento != null)
            movimiento.ForzarCorrer();
    }

    public bool EstaInvisible() => estaInvisible;
    public bool PuedeActivar() => Time.time >= tiempoUltimoUso + cooldownInvisibilidad;
    public float GetUltimoUso() => tiempoUltimoUso;
    public float GetCooldown() => cooldownInvisibilidad;
}
