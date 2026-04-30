using UnityEngine;

public class PlayerAnimationCon : MonoBehaviour
{

    [Header("Ref")]
    [SerializeField] private PlayerInputReader playerInputReader;
    [SerializeField] private PlayerMove2D playerMove2D;
    [SerializeField] private Animator targetAnimator;

    [Header("Animator Parameters")]
    [SerializeField] private string moveXParameterName = "MoveX";
    [SerializeField] private string moveYParameterName = "MoveY";
    [SerializeField] private string isMovingParameterName = "IsMoving";


    [Header("Tuning")]
    private float inputDeadZone = 0.1f;
    private bool keepLastDirectionOnIdle = true;


    public Vector2 CurrentInputDirecion { get; private set; }
    public Vector2 CurrentFacingDirecion { get; private set; } = Vector2.down;

    public bool IsMoving { get; private set; }

    private int moveXHash;
    private int moveYHash;
    private int isMovingHash;


    private void Awake()
    {
        moveXHash = Animator.StringToHash(moveXParameterName);
        moveYHash = Animator.StringToHash(moveYParameterName);
        isMovingHash = Animator.StringToHash(isMovingParameterName);
    }


    private void Update()
    {
        if(playerInputReader ==  null || targetAnimator == null) return;

        Vector2 rawInput = playerInputReader.MoveVector;
        CurrentInputDirecion = rawInput;

        IsMoving = rawInput.sqrMagnitude >= inputDeadZone * inputDeadZone;


        if(playerMove2D != null )
        {
            CurrentFacingDirecion = Direction8.ToVector2(playerMove2D.Currnentdirection);
        }
        else if (IsMoving)
        {
            CurrentFacingDirecion = rawInput.normalized;
        }
        else if(!keepLastDirectionOnIdle)
        {
            CurrentFacingDirecion = Vector2.zero;
        }


        ApplyDirectionToAnimator(CurrentFacingDirecion, IsMoving);
    }


    private void ApplyDirectionToAnimator(Vector2 Direction, bool isMoving)
    {

        targetAnimator.SetFloat(moveXHash, Direction.x);
        targetAnimator.SetFloat(moveYHash, Direction.y);
        targetAnimator.SetBool(isMovingHash, isMoving);
    }
}



