using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoManager : MonoBehaviour
{
    [Header("Elementos asignables")]
    public VideoPlayer videoPlayer;        // El componente VideoPlayer
    public GameObject videoScreen;         // El Raw Image que muestra el video
    public GameObject menuUI;              // El panel del menú
    public string nombreEscenaGameplay;    // Nombre exacto de la escena de gameplay

    private bool videoIniciado = false;

    public void ReproducirVideoYComenzar()
    {
        if (videoIniciado) return;

        videoIniciado = true;
        menuUI.SetActive(false);               // Ocultar el menú
        videoScreen.SetActive(true);           // Mostrar el video
        videoPlayer.loopPointReached += VideoTerminado;
        videoPlayer.Play();
    }

    void Update()
    {
        if (videoIniciado && Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.Stop();
            CargarEscena();
        }
    }

    void VideoTerminado(VideoPlayer vp)
    {
        CargarEscena();
    }

    void CargarEscena()
    {
        videoScreen.SetActive(false);         // Ocultar la pantalla de video
        SceneManager.LoadScene(nombreEscenaGameplay);
    }
}
