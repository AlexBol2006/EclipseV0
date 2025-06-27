using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiControlador : MonoBehaviour
{
    [Header("ReferenciasE")]

    private Rigidbody2D rb;

    [Header("MovimientoE")]
    [SerializeField]private Transform[] waypoints;
    private int waypointActual;
    [SerializeField] private float speed;
    [SerializeField] private float tiempoEspera;
    private bool estaEsperando;
    

    private void Update()
    {
        if(transform.position != waypoints[waypointActual].position)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointActual].position,speed * Time.deltaTime);
        }
        else if(!estaEsperando)
        {
            StartCoroutine(wait());
        }
    }
    IEnumerator wait()
    {
        estaEsperando = true;
        yield return new WaitForSeconds(tiempoEspera);
        waypointActual++;
        if (waypointActual == waypoints.Length)
        {
            waypointActual = 0; 
        }
        estaEsperando = false;
        Girar();

    }
    private void Girar()
    {
        if(transform.position.x > waypoints[waypointActual].position.x)
        {
            transform.rotation = Quaternion.Euler(0f,180f,0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    
}
