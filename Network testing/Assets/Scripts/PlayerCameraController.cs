using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private Vector2 max_follow_offset_ = new Vector2(-1f, 6f);
    [SerializeField] private Vector2 camera_velocity_ = new Vector2(4f, 0.25f);
    [SerializeField] private Transform player_transform_ = null;
    [SerializeField] private CinemachineVirtualCamera virtual_camera_ = null;

    private Controls controls_;
    private Controls controls
    {
        get 
        {
            if (controls_ != null) { return controls_; }
            return controls_ = new Controls();
        }
    }
    private CinemachineTransposer transposer_;

    public override void OnStartAuthority()
    {
        transposer_ = virtual_camera_.GetCinemachineComponent<CinemachineTransposer>();

        virtual_camera_.gameObject.SetActive(true);

        enabled = true;

        controls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
    }

    [ClientCallback]
    private void OnEnable() => controls.Enable();

    [ClientCallback]
    private void OnDisable() => controls.Disable();

    private void Look(Vector2 look_axis)
    {
        float delta_time = Time.deltaTime;

        float follow_offset = Mathf.Clamp(
            transposer_.m_FollowOffset.y - (look_axis.y * camera_velocity_.y * delta_time),
            max_follow_offset_.x,
            max_follow_offset_.y
            );

        transposer_.m_FollowOffset.y = follow_offset;

        player_transform_.Rotate(0f, look_axis.x * camera_velocity_.x * delta_time, 0f);
    }
}
