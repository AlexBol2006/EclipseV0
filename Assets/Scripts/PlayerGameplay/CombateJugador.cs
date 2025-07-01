using Unity.VisualScripting;
using UnityEngine;

public class CombateJugador : MonoBehaviour
{
    [SerializeField] private Transform controladorAtaque;
    [SerializeField] private float radioAtaque;
    [SerializeField] private int dañoAtaque;
    [SerializeField] private Animator animator;
    [SerializeField] private float tiempoEntreAtaques;
    [SerializeField] private float tiempoUltimoAtaques;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            IntentarAtacar();

        }
      
    }
    private void IntentarAtacar()
    {
        if (Time.time < tiempoUltimoAtaques + tiempoEntreAtaques)
        {
            return;
        }
        Atacar();
    }
    private void Atacar()
    {
        animator.SetTrigger("Atacar");

        tiempoUltimoAtaques = Time.deltaTime;

      Collider2D[] objetosTocados =  Physics2D.OverlapCircleAll(controladorAtaque.position,radioAtaque);
        foreach(Collider2D objeto in objetosTocados)
        {
            if (objeto.TryGetComponent(out VidaEnemigo vidaEnemigo))
            {
                vidaEnemigo.TomarDaño(dañoAtaque);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorAtaque.position,radioAtaque);
    }
}
