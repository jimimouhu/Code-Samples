using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyListener : MonoBehaviour
{
    private InputManager inputManager;
    public ActiveShoulderCannon activeShoulderCannon;
    public ActiveBackCannon activeBackCannon;
    public BlackHole bh;
    public MagicBall mb;
    public CastBallLightning cbl;
    public GrapplingGun gg;
    public PlayerMovementScript player;
    public Animator animation;
    public HealthManager playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = InputManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Action buttons
        if (!GameStatus.global.UI_ON)
        {
            if (!gg.shooting && !playerHealth.isDead)
            {
                // Offensives
                if (inputManager.GetKeyDown(KeyActions.Melee)) animation.SetTrigger("Melee");

                if (inputManager.GetKeyDown(KeyActions.Shoot)) activeShoulderCannon.Shoot();
                if (inputManager.GetKeyUp(KeyActions.Shoot)) activeShoulderCannon.StopShoot();
                
                if (inputManager.GetKeyDown(KeyActions.Shoot2)) activeBackCannon.Aim();
                if (inputManager.GetKeyUp(KeyActions.Shoot2)) activeBackCannon.Shoot();
                
                if (inputManager.GetKeyDown(KeyActions.Magic1))
                {
                    if (GameStatus.global.saveData.magicSlot1 != null)
                        switch (GameStatus.global.saveData.magicSlot1.blueprint.magicWandSubClass.type)
                        {
                            case MagicWandUpgradesSubClass.MagicType.MagicBall:
                                mb.StartManeuver();
                                break;
                            case MagicWandUpgradesSubClass.MagicType.Lightning:
                                cbl.StartManeuver();
                                break;
                            case MagicWandUpgradesSubClass.MagicType.BlackHole:
                                bh.AimSpell();
                                break;
                        }
                }
                if (inputManager.GetKeyDown(KeyActions.Magic2))
                {
                    if (GameStatus.global.saveData.magicSlot2 != null)
                        switch (GameStatus.global.saveData.magicSlot2.blueprint.magicWandSubClass.type)
                        {
                            case MagicWandUpgradesSubClass.MagicType.MagicBall:
                                mb.StartManeuver();
                                break;
                            case MagicWandUpgradesSubClass.MagicType.Lightning:
                                cbl.StartManeuver();
                                break;
                            case MagicWandUpgradesSubClass.MagicType.BlackHole:
                                bh.AimSpell();
                                break;
                        }
                }

                if (inputManager.GetKeyDown(KeyActions.Magic3))
                {
                    if (GameStatus.global.saveData.magicSlot3 != null)
                        switch (GameStatus.global.saveData.magicSlot3.blueprint.magicWandSubClass.type)
                        {
                            case MagicWandUpgradesSubClass.MagicType.MagicBall:
                                mb.StartManeuver();
                                break;
                            case MagicWandUpgradesSubClass.MagicType.Lightning:
                                cbl.StartManeuver();
                                break;
                            case MagicWandUpgradesSubClass.MagicType.BlackHole:
                                bh.AimSpell();
                                break;
                        }
                }

                // For BlackHole only
                if (inputManager.GetKeyUp(KeyActions.Magic1) 
                    && GameStatus.global.saveData.magicSlot1.blueprint.magicWandSubClass.type == MagicWandUpgradesSubClass.MagicType.BlackHole)
                    bh.ShootSpell();

                if (inputManager.GetKeyUp(KeyActions.Magic2) 
                    && GameStatus.global.saveData.magicSlot2.blueprint.magicWandSubClass.type == MagicWandUpgradesSubClass.MagicType.BlackHole)
                    bh.ShootSpell();

                if (inputManager.GetKeyUp(KeyActions.Magic3) 
                    && GameStatus.global.saveData.magicSlot3.blueprint.magicWandSubClass.type == MagicWandUpgradesSubClass.MagicType.BlackHole)
                    bh.ShootSpell();

            }


            // Jump
            if (inputManager.GetKeyDown(KeyActions.Jump))
            {
                player.jumpButtonDown = true;
                player.Jump();
            }
            else if (inputManager.GetKeyUp(KeyActions.Jump)) player.jumpButtonDown = false;

            // Check if grapple unlocked
            if(player.grappleAvailable)
            {
                // Grappling
                if (inputManager.GetKey(KeyActions.Grapple))
                {
                    gg.Shoot();
                   
                }
                else if (inputManager.GetKeyUp(KeyActions.Grapple))
                {
                    gg.StopShoot();
                }
            }
        }

        // Menu buttons
        if (inputManager.GetKeyDown(KeyActions.Inventory))
            GameStatus.global.toggleUI.Invoke(ActivateUI.Window.Inventory);

        if (inputManager.GetKeyDown(KeyActions.QuestLog))
            GameStatus.global.toggleUI.Invoke(ActivateUI.Window.QuestLog);
    }

}
