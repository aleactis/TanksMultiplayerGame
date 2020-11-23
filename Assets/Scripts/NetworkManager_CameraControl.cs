using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager_CameraControl : NetworkManager {

    [Header("Propriedades da Cena da Camera")]
    [SerializeField] Transform sceneCamera; //A cena da câmera
    [SerializeField] float cameraRotationRadius = 24f; //O raio da rotação da câmera
    [SerializeField] float cameraRotationSpeed = 3f; //A velocidade da rotação da câmerea
    [SerializeField] bool canRotate = true; //A câmera pode ser rotacionada?

    float rotation; //Rotação da câmera atual

    public override void OnStartClient(NetworkClient client)
    {
        canRotate = false;
    }

    public override void OnStartHost()
    {
        canRotate = false;
    }

    public override void OnStopClient()
    {
        canRotate = true;
    }

    public override void OnStopHost()
    {
        canRotate = true;
    }

    void Update()
    {
        //Não podemos rotacionar... Deixe-a...
        if (!canRotate)
        {
            return;
        }

        //Calcular nova rotação e assegurar ela não é maior que 360 graus
        rotation += cameraRotationSpeed * Time.deltaTime;
        if (rotation >= 360f)
            rotation -= 360f;

        //Rotacionar a câmera em torno do centro da cena
        sceneCamera.position = Vector3.zero;
        sceneCamera.rotation = Quaternion.Euler(0f, rotation, 0f);
        sceneCamera.Translate(0f, cameraRotationRadius, -cameraRotationRadius);
        sceneCamera.LookAt(Vector3.zero);
    }

}
