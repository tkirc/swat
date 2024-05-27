using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

// Authors: Marc Federspiel
public class PlayerInput : NetworkBehaviour
{

    private WeaponHolder weaponHolder;
    public delegate void UseCallback();

    private UseCallback onUse;



    private NetworkVariable<Vector2> moveInput =
        new NetworkVariable<Vector2>(Vector2.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    private NetworkVariable<Vector2> rotationInput =
        new NetworkVariable<Vector2>(Vector2.zero,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

    private NetworkVariable<Vector2> rotationControllerInput =
        new NetworkVariable<Vector2>(Vector2.zero,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);


    PlayerControls controls;

    void Awake()
    {

        controls = new PlayerControls();
        weaponHolder = GetComponent<WeaponHolder>();
    }

    void OnEnable()
    {


        controls.BattleControls.Move.performed += onMovePerformed;
        controls.BattleControls.Move.canceled += onMoveCanceled;
        controls.BattleControls.Shoot.performed += onShootPerformed;
        controls.BattleControls.Throw.performed += onThrowPerformed;
        controls.BattleControls.Use.performed += usePerformed;
        controls.BattleControls.LookRotation.performed += onLookRotationPerformed;
        controls.BattleControls.LookRotationController.performed += onLookRotationControllerPerformed;
        controls.Enable();

        controls.BattleControls.Disable();
    }

    void OnDisable()
    {

        controls.BattleControls.Move.performed -= onMovePerformed;
        controls.BattleControls.Move.canceled -= onMoveCanceled;
        controls.BattleControls.Shoot.performed -= onShootPerformed;
        controls.BattleControls.Throw.performed -= onThrowPerformed;
        controls.BattleControls.Use.performed -= usePerformed;
        controls.BattleControls.LookRotation.performed -= onLookRotationPerformed;
        controls.BattleControls.LookRotationController.performed -= onLookRotationControllerPerformed;
        controls.Disable();
    }

    public void enableBattleControls()
    {

        enableBattleControlsClientRpc();
    }

    [ClientRpc]
    public void enableBattleControlsClientRpc()
    {

        controls.BattleControls.Enable();
    }

    public void disableBattleControls()
    {

        disableBattleControlsClientRpc();
    }

    [ClientRpc]
    public void disableBattleControlsClientRpc()
    {

        controls.BattleControls.Disable();
    }

    public void onMovePerformed(InputAction.CallbackContext c)
    {

        if (!IsOwner) return;

        moveInput.Value = c.ReadValue<Vector2>();
    }

    public void onMoveCanceled(InputAction.CallbackContext c)
    {

        if (!IsOwner) return;

        moveInput.Value = c.ReadValue<Vector2>();
    }

    public void onLookRotationPerformed(InputAction.CallbackContext c)
    {
        if (!IsOwner) return;

        rotationInput.Value = c.ReadValue<Vector2>();
    }

    public void onLookRotationControllerPerformed(InputAction.CallbackContext c)
    {
        if (!IsOwner) return;

        rotationControllerInput.Value = c.ReadValue<Vector2>();
    }

    public void onShootPerformed(InputAction.CallbackContext c)
    {


        if (!IsOwner) return;

        onShootServerRpc();
    }

    [ServerRpc]
    public void onShootServerRpc()
    {


        weaponHolder.shoot();
    }

    public void onThrowPerformed(InputAction.CallbackContext c)
    {

        /*
        if (molotov == null) return;

        molotov.transform.SetParent(null);
        molotov.shoot();

        molotov = null;
        */
    }

    public void usePerformed(InputAction.CallbackContext c)
    {

        onUse?.Invoke();
    }

    public void registerOnUseCallback(UseCallback callback)
    {

        onUse += callback;
    }

    public void unregisterOnUseCallback(UseCallback callback)
    {

        onUse -= callback;
    }

    public Vector2 getMoveInput()
    {

        return moveInput.Value;
    }

    public Vector2 getRotationInput()
    {
        return rotationInput.Value;
    }

    public Vector2 getRotationInputController()
    {
        return rotationControllerInput.Value;
    }
}
