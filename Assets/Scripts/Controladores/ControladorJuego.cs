using UnityEngine;

public class ControladorJuego : MonoBehaviour
{
    public static ControladorJuego instance;

    private Vector3 ultimoCheckpoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // se mantiene entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivarCheckpoint(Vector3 pos)
    {
        ultimoCheckpoint = pos;
    }

    public Vector3 ObtenerCheckpoint()
    {
        return ultimoCheckpoint;
    }
}
