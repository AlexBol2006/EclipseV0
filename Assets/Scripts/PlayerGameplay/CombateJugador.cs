using Unity.VisualScripting;
using UnityEngine;

public class CombateJugador : MonoBehaviour
{
    [SerializeField] private Transform controladorAtaque;
    [SerializeField] private float radioAtaque;
    private void Update()
    {
        if (Input.GetButtonDown("Firel"))
        {
            Atacar();

        }
    }
    private void Atacar()
    {
      Collider2D[] objetosTocados =  Physics2D.OverlapCircleAll(controladorAtaque.position,radioAtaque);
        foreach(Collider2D objeto in objetosTocados)
        {
            
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorAtaque.position,radioAtaque);
    }
}
