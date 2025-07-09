using UnityEngine;
using UnityEngine.UI;

public class UIConsumibleUnico : MonoBehaviour
{
    [Header("Sprites desde lleno hasta vacío")]
    public Sprite[] spritesCuraciones; // Orden: 3, 2, 1, 0 curaciones
    public Image imagen; // Referencia a la UI de la imagen

    public void ActualizarUI(int cantidadRestante)
    {
        int index = Mathf.Clamp(cantidadRestante, 0, spritesCuraciones.Length - 1);
        if (imagen != null && spritesCuraciones.Length > 0)
            imagen.sprite = spritesCuraciones[index];
    }
}
