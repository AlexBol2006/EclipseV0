using Unity.Cinemachine;
using UnityEngine;

public class Kunai : MonoBehaviour
{
    [SerializeField] private float velocidad;
    [SerializeField] private float daño;

    private void Update()
    {
        transform.Translate(Vector2.right*velocidad*Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
       
        
    }

}
