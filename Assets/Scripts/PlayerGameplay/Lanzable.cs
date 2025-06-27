using UnityEngine;

public class Lanzable : MonoBehaviour
{
    [SerializeField] private Transform controladorLanzable;
    [SerializeField] private GameObject kunai;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Lanzar();
        }
    }
    private void Lanzar()
    {
        Instantiate(kunai, controladorLanzable.position, controladorLanzable.rotation );
    }
}
