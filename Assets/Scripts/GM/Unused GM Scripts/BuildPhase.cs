using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPhase : IState {

    // want to turn off/on things and define what the player can do within this round

    // constructor for the build phase?
    //private bool playerCanBuild = false;
    //private static PlayerControllerScript thePlayerInstance;



    public void Execute()
    {
        // check the build of the towers
        // have we met the no. of towers
        // what comes next?
    }


    public void Enter()
    {
        // when we enter find the player and turn on the building ability.
        //GetPlayer();
        // set build to true next;
        // thePlayerInstance.canBuild = true;
    }

    public void Exit()
    {
        // player cant build
        // thePlayerInstance.canBuild
    }

    //private void GetPlayer()
    //{
    //    if (thePlayerInstance == null)
    //    {
    //        thePlayerInstance = GameObject.FindObjectOfType<PlayerControllerScript>();
    //    }
    //    else
    //    {
    //        return;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        // will be checking for things the player decides
    }
}
