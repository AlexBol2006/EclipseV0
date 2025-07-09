using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class ReproducirVideoAlInicio : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private void Start()
    {
        StartCoroutine(ReproducirVideo());
    }

    private IEnumerator ReproducirVideo()
    {
        Time.timeScale = 0f; // Congela el juego

        videoPlayer.Play();

        // Esperar hasta que termine el video o se presione cualquier tecla o clic
        while (videoPlayer.isPlaying)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                videoPlayer.Stop();
                break;
            }

            yield return null;
        }

        videoPlayer.gameObject.SetActive(false);
        Time.timeScale = 1f; // Reactiva el juego
    }
}
