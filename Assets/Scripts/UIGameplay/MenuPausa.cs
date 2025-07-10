using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelPausa;
    public GameObject panelOpciones;

    private bool estaPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AlternarPausa();
        }
    }

    public void AlternarPausa()
    {
        estaPausado = !estaPausado;

        Time.timeScale = estaPausado ? 0 : 1;
        panelPausa.SetActive(estaPausado);
        panelOpciones.SetActive(false); // siempre ocultar opciones al abrir pausa
    }

    public void ReanudarJuego()
    {
        estaPausado = false;
        Time.timeScale = 1;
        panelPausa.SetActive(false);
        panelOpciones.SetActive(false);
    }

    public void AbrirOpciones()
    {
        panelOpciones.SetActive(true);
        panelPausa.SetActive(false);
    }

    public void VolverAPausa()
    {
        panelOpciones.SetActive(false);
        panelPausa.SetActive(true);
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SalirAMenuPrincipal(string nombreEscenaMenu)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Intro");
    }

    // Este lo usas con el botón de UI (OnClick)
    public void BotonPausaClick()
    {
        AlternarPausa();
    }

}
