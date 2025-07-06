using UnityEngine;

public class ParalaxMovement : MonoBehaviour
{
    Transform cam;
    Vector3 camStartPos;
    float distance;

    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;

    float farthestback;

    [Range(0.01f, 1f)]
    public float parallaxSpeed;

    private void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; ++i)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        for (int i = 0; i < backCount; i++)
        {
            float distanceToCam = backgrounds[i].transform.position.z - cam.position.z;
            if (distanceToCam > farthestback)
            {
                farthestback = distanceToCam;
            }
        }

        for (int i = 0; i < backCount; i++)
        {
            float distanceToCam = backgrounds[i].transform.position.z - cam.position.z;
            backSpeed[i] = 1 - (distanceToCam / farthestback);
        }
    }

    private void LateUpdate()
    {
        // Mover el objeto del parallax (el contenedor de fondos) en X y Y
        transform.position = new Vector3(cam.position.x - 1, cam.position.y, -1.09f);

        // Aplicar offset de parallax a las texturas de los materiales en X y Y
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;

            Vector2 offset = new Vector2(
                (cam.position.x - camStartPos.x) * speed,
                (cam.position.y - camStartPos.y) * speed
            );

            mat[i].SetTextureOffset("_MainTex", offset);
        }
    }
}
