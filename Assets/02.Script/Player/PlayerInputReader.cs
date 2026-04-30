using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputReader : MonoBehaviour
{
    private PlayerInput playerInput;

    private InputAction moveAction;

    public Vector2 MoveVector { get; private set; }

    [Header("Action Names")]
    [SerializeField] private string moveActionName = "Move";


    private void Awake()
    {
        if(playerInput == null) playerInput = GetComponent<PlayerInput>();
        ResolveActions();

    }

    private void Update()
    {
        MoveVector = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
    }

    private void ResolveActions()
    {
        if (playerInput == null || playerInput.actions == null)
        {
            Debug.LogWarning("[Player InputActinon] 확인");
            return;
        }

        moveAction = FindAction(moveActionName);
    }

    private InputAction FindAction(string ActionName)
    {
        if(string.IsNullOrWhiteSpace(ActionName)) return null;

        InputAction action = playerInput.actions.FindAction(ActionName, false);

        if (action == null)
        {
            Debug.LogWarning($"Action 못 찾음 {action}");
        }
        return action;
    }
}
