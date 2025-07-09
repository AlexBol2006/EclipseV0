using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BotonParpadeante : MonoBehaviour
{
    public CanvasGroup canvasGroup;         // Referencia al CanvasGroup del botón o texto
    public float velocidadParpadeo = 2f;    // Velocidad del parpadeo
    public KeyCode teclaIniciar = KeyCode.Return; // Tecla que oculta el botón (Enter por defecto)
    public string escenaAScargar = "";      // Nombre de la escena a cargar (opcional)

    private bool activo = true;

    void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (!activo || canvasGroup == null) return;

        // Oscilación del alpha (fade in/out)
        canvasGroup.alpha = Mathf.PingPong(Time.time * velocidadParpadeo, 1f);

        // Al presionar la tecla, oculta y (opcionalmente) cambia de escena
        if (Input.GetKeyDown(teclaIniciar))
        {
            activo = false;
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // Opcional: cargar escena si se definió un nombre
            if (!string.IsNullOrEmpty(escenaAScargar))
            {
                SceneManager.LoadScene(escenaAScargar);
            }
        }
    }
}
