using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class TankController_Net : NetworkBehaviour
{
    public AudioSource movementAudio; //Faz referência ao audio source usado para tocar o som do motor. Nota: Diferente do audio source dos tiros (shooting)
    public AudioClip engineIdling; //Audio para ser tocado quando o tanque não está em movimento
    public AudioClip engineDriving; //Audio para ser tocado quando o tanque está em movimento
    public float pitchRange = 0.2f; //A quantidade pela qual o passo dos ruídos do motor pode variar

    private float m_OriginalPitch; //O tom da fonte de áudio no início da cena.
    private float m_MovementInputValue; //O valor atual do movimento de entrada
    private float turnInputValue; //O valor atual da rotação de entrada

    [Header("Movimentos Variáveis")]
    [SerializeField]
    float movementSpeed = 10.0f; //A velocidade do tanque
    [SerializeField]
    float turnSpeed = 45.0f; //A velocidade de rotação do tanque

    [Header("Variáveis de posição da Câmera")]
    [SerializeField]
    float cameraDistance = 16f; //Distância que a câmera deve ficar do tanque
    [SerializeField]
    float cameraHeight = 16f; //Altura que a câmera deve estar do chão

    Rigidbody localRigidBody; //Cache da referência para o RigidBody
    Transform mainCamera; // Referência para cena da Main Camera
    Vector3 cameraOffset; //Um Vector3 que contém informação quão atrás e ao alto a câmera deve manter do tanque 

    void Start()
    {
        //Se o tank não for o Local Player
        if (!isLocalPlayer)
        {
            //Este tanque não precisa controlar a Camera, nem qualquer outra intereção
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
        m_OriginalPitch = movementAudio.pitch;
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
        mainCamera.position = transform.position; //...posição da camera no tanque
        mainCamera.rotation = transform.rotation; //...alinha a camera com o tanque
        mainCamera.Translate(cameraOffset); //move a camera para cima e fora do tanque
        mainCamera.LookAt(transform); //...move a camera sobre o tanque
    }

    private void EngineAudio()
    {
        // If there is no input (the tank is stationary)...
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(turnInputValue) < 0.1f)
        {
            // ... and if the audio source is currently playing the driving clip...
            if (movementAudio.clip == engineDriving)
            {
                // ... change the clip to idling and play it.
                movementAudio.clip = engineIdling;
                movementAudio.pitch = Random.Range(m_OriginalPitch - pitchRange, m_OriginalPitch + pitchRange);
                movementAudio.Play();
            }
        }
        else
        {
            // Otherwise if the tank is moving and if the idling clip is currently playing...
            if (movementAudio.clip == engineIdling)
            {
                // ... change the clip to driving and play.
                movementAudio.clip = engineDriving;
                movementAudio.pitch = Random.Range(m_OriginalPitch - pitchRange, m_OriginalPitch + pitchRange);
                movementAudio.Play();
            }
        }
    }
}


//[Header("Movimentos Variáveis")]
//[SerializeField]
//float movementSpeed = 5.0f; //A velocidade do Objeto
//[SerializeField] float turnSpeed = 45.0f; //A velocidade de giro do objecto
//[Header("Variáveis de posição da Câmera")]
//[SerializeField]
//float cameraDistance = 16f; //Distância do tanque que a câmera deve estar
//[SerializeField] float cameraHeight = 16f; //A Altura do chão que a câmera deve estar

//Rigidbody localRigidBody; //Cache da referência para o RigidBody
//Transform mainCamera; // Referência para cena da Main Camera
//Vector3 cameraOffset; //Um Vector3 que contém informação quão atrás e ao alto a câmera deve manter do tanque 

//void Start()
//{
//    //Se o tank não for o Local Player
//    if (!isLocalPlayer)
//    {
//        //Este tanque não precisa controlar a Camera, nem qualquer outra intereção
//        Destroy(this);
//        return;
//    } 

//    //Pegar a referencia para o objeto RigidBody desde que nós estaremos usando-o 
//    localRigidBody = GetComponent<Rigidbody>();

//    //Setar o offset da camera para uso futuro
//    cameraOffset = new Vector3(0f, cameraHeight, -cameraDistance);

//    //Localizar a cena da main Camera e movê-la dentro da posição correta
//    mainCamera = Camera.main.transform;
//    MoveCamera();
//}

//void FixedUpdate()
//{
//    //Pegar o input Vertical e Horizontal. Note que podemos pegar o input de
//    //qualquer plataforma usando a classe CrossPlatformInput
//    float turnAmount = CrossPlatformInputManager.GetAxis("Horizontal");
//    float moveAmount = CrossPlatformInputManager.GetAxis("Vertical");

//    //Calcular e aplicar a nova posição
//    Vector3 deltaTranslation = transform.position +
//        transform.forward * movementSpeed * moveAmount * Time.deltaTime;
//    localRigidBody.MovePosition(deltaTranslation);

//    //Calcular e aplicar a nova rotação
//    Quaternion deltaRotation = Quaternion.Euler(
//        turnSpeed * new Vector3(0, turnAmount, 0) * Time.deltaTime);
//    localRigidBody.MoveRotation(localRigidBody.rotation * deltaRotation);

//    //Atualizar a posição da câmera
//    MoveCamera();
//}

////Este método move a câmera para o correto ponto atrás do jogador
//void MoveCamera()
//{
//    mainCamera.position = transform.position; //...posição da camera no tanque
//    mainCamera.rotation = transform.rotation; //...alinha a camera com o tanque
//    mainCamera.Translate(cameraOffset); //move a camera para cima e fora do tanque
//    mainCamera.LookAt(transform); //...move a camera sobre o tanque
//}





//public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
//public float m_Speed = 12f;                 // How fast the tank moves forward and back.
//public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.
//public AudioSource movementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
//public AudioClip engineIdling;            // Audio to play when the tank isn't moving.
//public AudioClip engineDriving;           // Audio to play when the tank is moving.
//public float pitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

//private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
//private string m_TurnAxisName;              // The name of the input axis for turning.
//private Rigidbody m_Rigidbody;              // Reference used to move the tank.
//private float m_MovementInputValue;         // The current value of the movement input.
//private float m_TurnInputValue;             // The current value of the turn input.
//private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
//private ParticleSystem[] m_particleSystems; // References to all the particles systems used by the Tanks

//private void Awake()
//{
//    m_Rigidbody = GetComponent<Rigidbody>();
//}


//private void OnEnable()
//{
//    // When the tank is turned on, make sure it's not kinematic.
//    m_Rigidbody.isKinematic = false;

//    // Also reset the input values.
//    m_MovementInputValue = 0f;
//    m_TurnInputValue = 0f;

//    // We grab all the Particle systems child of that Tank to be able to Stop/Play them on Deactivate/Activate
//    // It is needed because we move the Tank when spawning it, and if the Particle System is playing while we do that
//    // it "think" it move from (0,0,0) to the spawn point, creating a huge trail of smoke
//    m_particleSystems = GetComponentsInChildren<ParticleSystem>();
//    for (int i = 0; i < m_particleSystems.Length; ++i)
//    {
//        m_particleSystems[i].Play();
//    }
//}


//private void OnDisable()
//{
//    // When the tank is turned off, set it to kinematic so it stops moving.
//    m_Rigidbody.isKinematic = true;

//    // Stop all particle system so it "reset" it's position to the actual one instead of thinking we moved when spawning
//    for (int i = 0; i < m_particleSystems.Length; ++i)
//    {
//        m_particleSystems[i].Stop();
//    }
//}


//private void Start()
//{
//    //Se o tank não for o Local Player
//    if (!isLocalPlayer)
//    {
//        //Este tanque não precisa controlar a Camera, nem qualquer outra intereção
//        Destroy(this);
//        return;
//    }

//    //// The axes names are based on player number.
//    //m_MovementAxisName = "Vertical" + m_PlayerNumber;
//    //m_TurnAxisName = "Horizontal" + m_PlayerNumber;

//    // Store the original pitch of the audio source.
//    m_OriginalPitch = movementAudio.pitch;
//}


//private void Update()
//{
//    //// Store the value of both input axes.
//    //m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
//    //m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

//    // Store the value of both input axes.
//    m_MovementInputValue = CrossPlatformInputManager.GetAxis(m_MovementAxisName);
//    m_TurnInputValue = CrossPlatformInputManager.GetAxis(m_TurnAxisName);


//    EngineAudio();
//}


//private void EngineAudio()
//{
//    // If there is no input (the tank is stationary)...
//    if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
//    {
//        // ... and if the audio source is currently playing the driving clip...
//        if (movementAudio.clip == engineDriving)
//        {
//            // ... change the clip to idling and play it.
//            movementAudio.clip = engineIdling;
//            movementAudio.pitch = Random.Range(m_OriginalPitch - pitchRange, m_OriginalPitch + pitchRange);
//            movementAudio.Play();
//        }
//    }
//    else
//    {
//        // Otherwise if the tank is moving and if the idling clip is currently playing...
//        if (movementAudio.clip == engineIdling)
//        {
//            // ... change the clip to driving and play.
//            movementAudio.clip = engineDriving;
//            movementAudio.pitch = Random.Range(m_OriginalPitch - pitchRange, m_OriginalPitch + pitchRange);
//            movementAudio.Play();
//        }
//    }
//}


//private void FixedUpdate()
//{
//    // Adjust the rigidbodies position and orientation in FixedUpdate.
//    Move();
//    Turn();
//}


//private void Move()
//{
//    // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
//    Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

//    // Apply this movement to the rigidbody's position.
//    m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
//}


//private void Turn()
//{
//    // Determine the number of degrees to be turned based on the input, speed and time between frames.
//    float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

//    // Make this into a rotation in the y axis.
//    Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

//    // Apply this rotation to the rigidbody's rotation.
//    m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
//}