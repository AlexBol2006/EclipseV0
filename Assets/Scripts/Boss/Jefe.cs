//using System.Collections;
//using UnityEngine;

//public class Jefe : MonoBehaviour
//{
//    private Animator animator;

//    public Rigidbody2D rb;
        
//    public Transform jugador;

//    private bool mirandoDerecha;

//    [Header("Vida")]

//    [SerializeField] private float vida;

//    //[SerializeField] private BarradeVida barradeVida;


//    private void Start()
//    {
//        animator = GetComponent<Animator>();
//        rb = GetComponent<Rigidbody2D>();
//        barradeVida.InicializarBarraDeVida(vida);
//        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
//    }
//    public void TomarDaño(float daño)
//    {
//        vida -= daño;

//        barradeVida.CambiarVidaActual(vida);
        
//        if(vida <= 0)
//        {
//            animator.SetTrigger("Morir ");
//        }
//    }
//    private void Muerte()
//    {
//        Destroy(gameObject);
//    }

//}
