using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetUpLocalPlayer : NetworkBehaviour
{
    [SyncVar]
    public string pname = "player";

    void OnGUI()
    {
        if (isLocalPlayer)
        {
            Debug.Log("entrei nessa porra");
            pname = GUI.TextField(new Rect (25, Screen.height - 40, 100, 30), pname);
            if (GUI.Button(new Rect(130, Screen.height - 40, 80, 30),"Change"))
            {
                CmdChangeName(pname);
            }
        }
    }

    [Command]
    public void CmdChangeName(string newName)
    {
        pname = newName;
        this.GetComponentInChildren<TextMesh>().text = pname;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            GetComponent<PlayerController_Net>().enabled = true;
            //Camera.main.transform.position = this.transform.position - this.transform.forward * 10 +
            //                                 this.transform.up * 5;
            //Camera.main.transform.LookAt(this.transform.position);
            //Camera.main.transform.parent = this.transform;

            //SmoothCameraFollow.target = this.transform;
        } 
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponentInChildren<TextMesh>().text = pname;
    }
}
