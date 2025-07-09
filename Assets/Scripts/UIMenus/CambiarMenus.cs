using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarMenus : MonoBehaviour
{
    public TransicionMenus transicion;

    [Header("Referencias a paneles")]
    public GameObject menuPrincipal;
    public GameObject menuOpciones;
    public GameObject menuCreditos;

    [Header("Nombre de la escena para jugar")]
    public string nombreEscenaJuego = "Nivel1"; // Cambia este nombre por tu escena real

    public void IrAlMenuPrincipal()
    {
        transicion.CambiarMenu(menuPrincipal);
    }

    public void IrAOpciones()
    {
        transicion.CambiarMenu(menuOpciones);
    }

    public void IrACreditos()
    {
        transicion.CambiarMenu(menuCreditos);
    }

    public void Jugar()
    {
        StartCoroutine(transicion.FadeYLoadScene(nombreEscenaJuego));
    }
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

}
