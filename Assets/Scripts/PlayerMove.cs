using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
//using Random = UnityEngine.Random;

public class PlayerMove : NetworkBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;

    //[SyncVar(hook = "DefinirCor")]
    //private Color cor = Color.white;

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            //Pegar os inputs do jogador através do CrossPlatformInputManager
            float translation = CrossPlatformInputManager.GetAxis("Vertical") * speed;
            float rotation = CrossPlatformInputManager.GetAxis("Horizontal") * rotationSpeed;

            //Eliminar o FPS da máquina local, multiplicando por Time.deltaTime
            translation *= Time.deltaTime;
            rotation *= Time.deltaTime;

            //Realizar o transform
            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    float r = Random.Range(0f, 1f);
            //    float g = Random.Range(0f, 1f);
            //    float b = Random.Range(0f, 1f);
            //    CmdModificaCor(r, g, b);
            //}
        }
    }

    //[Command]
    //private void CmdModificaCor(float r, float g, float b)
    //{
    //    this.cor = new Color(r, g, b);
    //}

    //public void DifinirCor(Color c)
    //{
    //    Renderer renderer = GetComponent<Renderer>();
    //    renderer.material.color = c;
    //}
}
