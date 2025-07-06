using UnityEngine;
using UnityEngine.UI;

public class BarraDeVIdaUI : MonoBehaviour
{
    [SerializeField] private Slider sliderBarraDeVida;

    public void InicairBarraDeVida(int vidaMaxima, int vidaActual)
    {
        sliderBarraDeVida.maxValue = vidaMaxima;
        sliderBarraDeVida.value = vidaActual;
    }

    public void CambiarBarraDeVida(int vidaActual)
    {
        sliderBarraDeVida.value = vidaActual;
    }
}
    