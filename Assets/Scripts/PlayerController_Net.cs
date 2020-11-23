using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class PlayerController_Net : NetworkBehaviour
{
    //public AudioSource movementAudio; //Faz referência ao audio source usado para tocar o som do motor. Nota: Diferente do audio source dos tiros (shooting)
    //public AudioClip engineIdling; //Audio para ser tocado quando o Player não está em movimento
    //public AudioClip engineDriving; //Audio para ser tocado quando o Player está em movimento
    public float pitchRange = 0.2f; //A quantidade pela qual o passo dos ruídos do motor pode variar

    private float m_OriginalPitch; //O tom da fonte de áudio no início da cena.
    private float m_MovementInputValue; //O valor atual do movimento de entrada
    private float turnInputValue; //O valor atual da rotação de entrada

    [Header("Movimentos Variáveis")]
    [SerializeField]
    float movementSpeed = 10.0f; //A velocidade do Player
    [SerializeField]
    float turnSpeed = 45.0f; //A velocidade de rotação do Player

    [Header("Variáveis de posição da Câmera")]
    [SerializeField]
    float cameraDistance = 16f; //Distância que a câmera deve ficar do Player
    [SerializeField]
    float cameraHeight = 16f; //Altura que a câmera deve estar do chão

    Rigidbody localRigidBody; //Cache da referência para o RigidBody
    Transform mainCamera; // Referência para cena da Main Camera
    Vector3 cameraOffset; //Um Vector3 que contém informação quão atrás e ao alto a câmera deve manter do Player 

    void Start()
    {
        //Se o Player não for o Local Player
        if (!isLocalPlayer)
        {
            //Este Player não precisa controlar a Camera, nem qualquer outra intereção
            Destroy(this);
            return;
        }

        //Pegar a referencia para o objeto RigidBody desde que nós estaremos usando-o 
        localRigidBody = GetComponent<Rigidbody>();

        //Setar o offset da camera para uso futuro
        cameraOffset = new Vector3(0f, cameraHeight, -cameraDistance);

        //Localizar a cena da main Camera e movê-la dentro da posição correta
        mainCamera = Camera.main.transform;
        MoveCamera();

        // Store the original pitch of the audio source.
        //m_OriginalPitch = movementAudio.pitch;
    }

    void FixedUpdate()
    {
        //Pegar o input Vertical e Horizontal. Note que podemos pegar o input de
        //qualquer plataforma usando a classe CrossPlatformInput
        m_MovementInputValue = CrossPlatformInputManager.GetAxis("Vertical");
        turnInputValue = CrossPlatformInputManager.GetAxis("Horizontal");

        //Calcular e aplicar a nova posição
        Vector3 deltaTranslation = transform.position +
            transform.forward * movementSpeed * m_MovementInputValue * Time.deltaTime;
        localRigidBody.MovePosition(deltaTranslation);

        //Calcular e aplicar a nova rotação
        Quaternion deltaRotation = Quaternion.Euler(
            turnSpeed * new Vector3(0, turnInputValue, 0) * Time.deltaTime);
        localRigidBody.MoveRotation(localRigidBody.rotation * deltaRotation);

        //Atualizar a posição da câmera
        MoveCamera();

        //Som do motor
        EngineAudio();
    }

    //Este método move a câmera para o correto ponto atrás do jogador
    void MoveCamera()
    {
        mainCamera.position = transform.position; //...posição da camera no Player
        mainCamera.rotation = transform.rotation; //...alinha a camera com o Player
        mainCamera.Translate(cameraOffset); //move a camera para cima e fora do Player
        mainCamera.LookAt(transform); //...move a camera sobre o Player
    }

    private void EngineAudio()
    {
        // If there is no input (the Player is stationary)...
        //if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(turnInputValue) < 0.1f)
        //{
        //    // ... and if the audio source is currently playing the driving clip...
        //    if (movementAudio.clip == engineDriving)
        //    {
        //        // ... change the clip to idling and play it.
        //        movementAudio.clip = engineIdling;
        //        movementAudio.pitch = Random.Range(m_OriginalPitch - pitchRange, m_OriginalPitch + pitchRange);
        //        movementAudio.Play();
        //    }
        //}
        //else
        //{
        //    // Otherwise if the Player is moving and if the idling clip is currently playing...
        //    if (movementAudio.clip == engineIdling)
        //    {
        //        // ... change the clip to driving and play.
        //        movementAudio.clip = engineDriving;
        //        movementAudio.pitch = Random.Range(m_OriginalPitch - pitchRange, m_OriginalPitch + pitchRange);
        //        movementAudio.Play();
        //    }
        //}
    }
}