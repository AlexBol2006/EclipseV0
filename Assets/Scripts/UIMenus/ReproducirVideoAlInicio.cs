using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReproducirVideoAlInicio : MonoBehaviour
{
    [Header("Referencias")]
    public VideoPlayer videoPlayer;
    public GameObject videoScreen;
    public GameObject menuUI;
    public CanvasGroup transitionPanel;
    public string nombreEscenaGameplay;

    private bool videoIniciado = false;

    public void ReproducirVideoYComenzar()
    {
        if (videoIniciado) return;

        videoIniciado = true;
        menuUI.SetActive(false);
        transitionPanel.gameObject.SetActive(true);
        transitionPanel.alpha = 1f;

        videoScreen.SetActive(false);
        StartCoroutine(PrepararYReproducir());
    }

    private IEnumerator PrepararYReproducir()
    {
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoScreen.SetActive(true); // Mostrar justo antes de reproducir
        videoPlayer.Play();
        videoPlayer.loopPointReached += VideoTerminado;

        // Transición negro → video
        yield return FadeCanvas(1f, 0f, 1f);
    }

    private void Update()
    {
        if (videoIniciado && Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.Stop();
            StartCoroutine(FinalizarConTransicion());
        }
    }

    private void VideoTerminado(VideoPlayer vp)
    {
        StartCoroutine(FinalizarConTransicion());
    }

    private IEnumerator FinalizarConTransicion()
    {
        // Transición video → negro
        yield return FadeCanvas(0f, 1f, 1f);

        videoScreen.SetActive(false);
        SceneManager.LoadScene(nombreEscenaGameplay);
    }

    private IEnumerator FadeCanvas(float from, float to, float duration, System.Action onComplete = null)
    {
        float tiempo = 0f;
        transitionPanel.alpha = from;

        while (tiempo < duration)
        {
            transitionPanel.alpha = Mathf.Lerp(from, to, tiempo / duration);
            tiempo += Time.deltaTime;
            yield return null;
        }

        transitionPanel.alpha = to;
        onComplete?.Invoke();
    }
}
