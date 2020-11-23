using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class TankController : MonoBehaviour
{
    [Header("Movimentos Variáveis")]
    [SerializeField]
    float movementSpeed = 10.0f; //Velocidade do movimento
    [SerializeField]
    float turnSpeed = 100.0f; //Velocidade de rotação

    [Header("Variáveis de posicionamento da cãmera")]
    [SerializeField]
    float cameraDistance = 16f; //Distância do Player que a câmera deverá estar
    [SerializeField]
    float cameraHeight = 16f; //A altura que a cãmera deve estar do chão

    Rigidbody localRigidBody; //Cache de referência para o RigidBody
    Transform mainCamera; //Referencia para a cena da main camera
    Vector3 cameraOffset; //Um vetor 3D que contém informações de quão atrás e ao alto a câmera deverá manter-se em relação ao Player que

    void Start()
    {
        //Pegar a referência para o obj RB
        localRigidBody = GetComponent<Rigidbody>();

        //Setar o offset da camera para uso futuro
        cameraOffset = new Vector3(0f,cameraHeight,-cameraDistance);

        //Localizar a cena da main camera e movê-la para dentro da posição correta
        mainCamera = Camera.main.transform;
        MoveCamera();
    }

    void FixedUpdate()
    {
        //Pegar o input Vertical e Horizontal. Note que podemos pegar o Input de qqulaquer plataforma
        float turnAmount = CrossPlatformInputManager.GetAxis("Horizontal");
        float moveAmount = CrossPlatformInputManager.GetAxis("Vertical");

        //Calcular e aplicar a nova posição
        Vector3 deltaTranslation = transform.position + transform.forward *
                                    movementSpeed * moveAmount * Time.deltaTime;
        localRigidBody.MovePosition(deltaTranslation);

        //Calcular e aplicar a nova rotação
        Quaternion deltaRotation = Quaternion.Euler(turnSpeed * new Vector3(0,turnAmount,0) *
                                        Time.deltaTime);
        localRigidBody.MoveRotation(localRigidBody.rotation * deltaRotation);

        //Atualizar a posição da câmera
        MoveCamera();
    }

    private void MoveCamera()
    {
        //Este método move a câmera para o ponto exato atrás do jogador
        mainCamera.position = transform.position; //posição da camera no Player
        mainCamera.rotation = transform.rotation; //alinha a camera com o Player

        mainCamera.Translate(cameraOffset); //move a camera para cima e para fora do Player
        mainCamera.LookAt(transform); //move a camera sobre o Player
    }
}
