using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class TransicionMenus : MonoBehaviour
{
    public CanvasGroup pantallaNegra;
    public float duracion = 1f;

    [Header("Todos los paneles de menú")]
    public List<GameObject> menus = new List<GameObject>();

    void Start()
    {
        if (pantallaNegra != null)
        {
            pantallaNegra.alpha = 1f;
            pantallaNegra.blocksRaycasts = true;
            StartCoroutine(Fade(1, 0));
        }
    }

    public void CambiarMenu(GameObject nuevoMenu)
    {
        StartCoroutine(Transicion(nuevoMenu));
    }

    private IEnumerator Transicion(GameObject nuevoMenu)
    {
        Debug.Log("Transición a: " + nuevoMenu.name);
        yield return StartCoroutine(Fade(0, 1)); // Fade-out

        // Desactivar todos los menús
        foreach (GameObject menu in menus)
        {
            if (menu != null)
                menu.SetActive(false);
        }

        // Activar nuevo menú
        if (nuevoMenu != null)
            nuevoMenu.SetActive(true);

        yield return StartCoroutine(Fade(1, 0)); // Fade-in
    }

    private IEnumerator Fade(float desde, float hasta)
    {
        if (pantallaNegra != null)
            pantallaNegra.blocksRaycasts = true;

        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(desde, hasta, tiempo / duracion);
            pantallaNegra.alpha = alpha;
            yield return null;
        }

        pantallaNegra.alpha = hasta;

        if (pantallaNegra != null && hasta == 0f)
            pantallaNegra.blocksRaycasts = false;
    }

    public IEnumerator FadeYLoadScene(string nombreEscena)
    {
        yield return StartCoroutine(Fade(0, 1)); // Fade-out
        SceneManager.LoadScene(nombreEscena);    // Cargar escena
    }
}
