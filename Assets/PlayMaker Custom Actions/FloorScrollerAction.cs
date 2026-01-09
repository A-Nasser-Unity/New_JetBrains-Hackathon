using UnityEngine;
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

[ActionCategory("Material")]
[Tooltip("Scrolls a material's texture with acceleration from 0 to max speed over time.")]
public class FloorScrollerAction : FsmStateAction
{
    [RequiredField]
    [CheckForComponent(typeof(Renderer))]
    public FsmOwnerDefault gameObject;

    [Tooltip("Maximum scroll speed (use negative value for reverse direction)")]
    public FsmFloat maxScrollSpeed;

    [Tooltip("Time in seconds to reach max speed")]
    public FsmFloat accelerationTime;

    [Tooltip("Execute every frame")]
    public bool everyFrame;

    private Renderer rend;
    private Vector2 offset = Vector2.zero;
    private float currentSpeed = 0f;
    private float acceleration;

    public override void Reset()
    {
        gameObject = null;
        maxScrollSpeed = -0.5f;
        accelerationTime = 2f;
        everyFrame = true;
    }

    public override void OnEnter()
    {
        var go = Fsm.GetOwnerDefaultTarget(gameObject);
        if (go != null)
        {
            rend = go.GetComponent<Renderer>();
        }

        currentSpeed = 0f;
        offset = Vector2.zero;

        // Calculate acceleration
        if (accelerationTime.Value > 0)
        {
            acceleration = maxScrollSpeed.Value / accelerationTime.Value;
        }
        else
        {
            currentSpeed = maxScrollSpeed.Value;
        }

        DoScroll();

        if (!everyFrame)
        {
            Finish();
        }
    }

    public override void OnUpdate()
    {
        DoScroll();
    }

    void DoScroll()
    {
        if (rend == null) return;

        // Gradually increase speed until reaching max speed
        if (Mathf.Abs(currentSpeed) < Mathf.Abs(maxScrollSpeed.Value))
        {
            currentSpeed += acceleration * Time.deltaTime;

            // Clamp to max speed
            if (maxScrollSpeed.Value < 0)
            {
                currentSpeed = Mathf.Max(currentSpeed, maxScrollSpeed.Value);
            }
            else
            {
                currentSpeed = Mathf.Min(currentSpeed, maxScrollSpeed.Value);
            }
        }

        // Apply current speed to texture offset
        offset.y += currentSpeed * Time.deltaTime;
        rend.material.mainTextureOffset = offset;
    }
}