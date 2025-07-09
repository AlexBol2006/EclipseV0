using UnityEngine;

public class IntroAutoClick : MonoBehaviour
{
    public CambiarMenus cambiarMenus;

    private bool yaCambio = false;

    void Update()
    {
        if (!yaCambio && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            yaCambio = true;
            cambiarMenus.IrAlMenuPrincipal();
        }
    }
}
