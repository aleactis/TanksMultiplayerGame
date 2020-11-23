using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShooting_Net : NetworkBehaviour
{
    [SerializeField]
    float power = 800f;
    [SerializeField]
    Transform gunBarrel; //Onde as balas são disparadas
    [SerializeField]
    public GameObject shell; //Prefab da shell

    [SerializeField]
    public AudioSource m_ShootingAudio; //Referência ao áudio source usado para o dispsro das balas. N
                                        //Nota: Diferente do Audio Source do movimento do tanque
    [SerializeField]
    public AudioClip m_FireClip; //Audio que tocará quando cada tiro for disparado

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Se não for o local player, retornar. 
        //Note que não podemos remover este script assim como fizemos no script TankController_Net, a razão
        //para isso é que o script tem um comando (Cmd) que precisa existir neste objeto mesmo que ele não
        // seja o local player.
        if (!isLocalPlayer)
            return;

        //Se clicarmos no mouse ou apertar o spacebar ou outros inputs...
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
        {
            //Roda o comando no servidor para gerar um projétil
            CmdSpawnShell();
        }
    }

    [Command]
    private void CmdSpawnShell()
    {
        //Este comando é chamado do Local Player e executado no servidor.
        //Instanciar uma arma no cano do tanque com a posição e rotação corretas
        GameObject instance = Instantiate(shell, gunBarrel.position, gunBarrel.rotation) as GameObject;
        //Localizar o componente RigidBody da arma e adicionarmos uma força física à ele
        instance.GetComponent<Rigidbody>().AddForce(gunBarrel.forward * power);
        //Instanciar este objeto na rede para que todos os players o vejam
        NetworkServer.Spawn(instance);

        //Mudar o Audio Clip para o tiro e tocá-lo
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }

    void Reset()
    {
        //Pega a localização do canhão (usamos um slow code)
        //transform.Find("GunBarrel");

        //Pega a localização do canhão (código recomendado pela Unity)
        gunBarrel = GameObject.FindGameObjectWithTag("GunBarrel").transform;
    }

}
