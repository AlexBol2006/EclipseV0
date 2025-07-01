using System;
using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    [SerializeField] private int vidaMaxima;
    [SerializeField] private int vidaActual;
    private void Awake()
    {
        vidaActual = vidaMaxima; 
    }
    public void TomarDaño(int cantidadDaño)
    {
        int cantidadDeVidaTemporal = vidaActual - cantidadDaño;
        
        cantidadDeVidaTemporal = Mathf.Clamp(cantidadDeVidaTemporal, 0, vidaMaxima);

        vidaActual = cantidadDeVidaTemporal;

        if (vidaActual ==0 )
        {
            Destroy(gameObject);
        }

    }
}
