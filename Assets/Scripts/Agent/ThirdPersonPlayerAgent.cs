using Fusion;
using Fusion.KCC;
using System;
using UnityEngine;

public class ThirdPersonPlayerAgent : PlayerAgent
{

    [SerializeField]
    [Tooltip("Visual should always face player move direction.")]
    private bool _faceMoveDirection;
    [SerializeField]
    [Tooltip("Visual always facing forward direction if the player holds right mouse button, ignoring Face Move Direction.")]
    private bool _mouseHoldRotationPriority;
    [SerializeField]
    [Tooltip("Events which trigger look rotation update of KCC.")]
    private ELookRotationUpdateSource _lookRotationUpdateSource = ELookRotationUpdateSource.Jump | ELookRotationUpdateSource.Movement | ELookRotationUpdateSource.MouseHold;


    [Networked]
    [Accuracy(0.00001f)]
    private Vector2 _pendingLookRotationDelta { get; set; }
    [Networked]
    [Accuracy(0.00001f)]
    private float _facingMoveRotation { get; set; }

    private Vector2 _renderLookRotationDelta;
    private Interpolator<float> _facingMoveRotationInterpolator;

    // Agent INTERFACE
       
    protected override void OnSpawned() {
        _facingMoveRotationInterpolator = GetInterpolator<float>(nameof(_facingMoveRotation));
    }

    protected override void ProcessEarlyFixedInput() {
        // Here we process input and set properties related to movement / look.
        // For following lines, we should use Input.FixedInput only. This property holds input for fixed updates.

        var input = Owner.Input.FixedInput;

        // Clamp input look rotation delta. Instead of applying immediately to KCC, we store it locally as pending and defer application to a point where conditions for application are met.
        // This allows us to rotate with camera around character standing still.
        Vector2 lookRotation = KCC.FixedData.GetLookRotation(true, true);
        _pendingLookRotationDelta = KCCUtility.GetClampedLookRotationDelta(lookRotation, _pendingLookRotationDelta + input.LookRotationDelta, -MaxCameraAngle, MaxCameraAngle);

        bool updateLookRotation = default;
        Quaternion facingRotation = default; // default is invalid (not set)
        Quaternion jumpRotation = default; // default is invalid (not set)

        // Checking look rotation update conditions
        if (HasLookRotationUpdateSource(ELookRotationUpdateSource.Jump) == true)            {       updateLookRotation |= Owner.Input.WasPressed(EInputButtons.Jump); }
        if (HasLookRotationUpdateSource(ELookRotationUpdateSource.Movement) == true)        {       updateLookRotation |= input.MoveDirection.IsZero() == false; }
        if (HasLookRotationUpdateSource(ELookRotationUpdateSource.MouseHold) == true)       {       updateLookRotation |= Owner.Input.IsSet(EInputButtons.AltFire); }
        if (HasLookRotationUpdateSource(ELookRotationUpdateSource.MouseMovement) == true)   {       updateLookRotation |= input.LookRotationDelta.IsZero() == false; }

        if (updateLookRotation == true) {
            // Some conditions are met, we can apply pending look rotation delta to KCC
            if (_pendingLookRotationDelta.IsZero() == false) {
                KCC.AddLookRotation(_pendingLookRotationDelta);
                _pendingLookRotationDelta = default;
            }
        }

        if (updateLookRotation == true || _faceMoveDirection == false) {
            // Setting base facing and jump rotation
            facingRotation = KCC.FixedData.TransformRotation;
            jumpRotation = KCC.FixedData.TransformRotation;
        }

        Vector3 inputDirection = default;
        bool hasInputDirection = default;
        bool faceOnMouseHold = _faceMoveDirection;

        Vector3 moveDirection = input.MoveDirection.X0Y();
        if (moveDirection.IsZero() == false) {
            // Calculating world space input direction for KCC, update facing and jump rotation based on configuration.

            hasInputDirection = true;
            inputDirection = KCC.FixedData.TransformRotation * moveDirection;
        }

        if (hasInputDirection == true) {
            Quaternion inputRotation = Quaternion.LookRotation(inputDirection);

            // We are moving in certain direction, we'll use it also for jump.
            jumpRotation = inputRotation;

            // Facing move direction enabled and right mouse button rotation lock disabled? Treat input rotation as facing as well.
            if (faceOnMouseHold == true && (_mouseHoldRotationPriority == false || Owner.Input.IsSet(EInputButtons.AltFire) == false)) {
                facingRotation = inputRotation;
            }
        }

        KCC.SetInputDirection(inputDirection);

        if (Owner.Input.WasPressed(EInputButtons.Jump) == true) {
            // Is jump rotation invalid (not set)? Get it from other source.
            if (jumpRotation.IsZero() == true) {
                // Is facing rotation valid? Use it.
                if (facingRotation.IsZero() == false) {
                    jumpRotation = facingRotation;
                }
                else {
                    // Otherwise just jump forward.
                    jumpRotation = KCC.FixedData.TransformRotation;
                }
            }

            // Is facing rotation invalid (not set)? Set it to the same rotation as jump.
            if (facingRotation.IsZero() == true) {
                facingRotation = jumpRotation;
            }

            KCC.Jump(jumpRotation * JumpImpulse);
        }


        //SPRINT SPRINT SPRINT!!!!!
        // Notice we are checking KCC.FixedData because we are in fixed update code path (render update uses KCC.RenderData)
        //if (KCC.FixedData.IsGrounded == true) {
        //    // Sprint is updated only when grounded
        //    KCC.SetSprint(input.Sprint);
        //}


        // Another movement related actions here (crouch, ...)

        // Is facing rotation set? Apply to the visual and store it.
        if (facingRotation.IsZero() == false) {

            if (_faceMoveDirection == true) {
                KCCUtility.GetLookRotationAngles(facingRotation, out float facingPitch, out float facingYaw);
                _facingMoveRotation = facingYaw;
            }
        }
    }


    protected override void OnFixedUpdate() {
        // Regular fixed update for Player/AdvancedPlayer class.
        // Executed after all player KCC updates and before HitboxManager.

        // Setting camera pivot location
        // In this case we have to apply pending look rotation delta (cached locally) on top of current KCC look rotation.

        Vector2 pitchRotation = KCC.FixedData.GetLookRotation(true, false);
        Vector2 clampedCameraRotation = KCCUtility.GetClampedLookRotation(pitchRotation + _pendingLookRotationDelta, -MaxCameraAngle, MaxCameraAngle);

        CameraPivot.rotation = KCC.FixedData.TransformRotation * Quaternion.Euler(clampedCameraRotation);

        if (_faceMoveDirection == true && Object.IsProxy == true) {
            // Facing rotation for visual is already set on input and state authority, here we update proxies based on [Networked] property.
            //Visual.Root.rotation = Quaternion.Euler(0.0f, _facingMoveRotation, 0.0f);
        }
    }

    protected override void ProcessLateFixedInput() {
        // Executed after HitboxManager. Process other non-movement actions like shooting.

        Weapons.ProcessInput(Owner.Input);

        if (Owner.Input.WasPressed(EInputButtons.Fire) == true) {
            // Left mouse button action
        }

        if (Owner.Input.WasPressed(EInputButtons.AltFire) == true) {
            // Right mouse button action
        }

        if (Owner.Input.WasPressed(EInputButtons.Reload) == true) {
            // Reload button action
        }
    }

    protected override void OnLateFixedUpdate() {
        base.OnLateFixedUpdate();

        Weapons.OnLateFixedUpdate();
    }
 
    protected override void ProcessRenderInput() {
        // Here we process input and set properties related to movement / look.
        // For following lines, we should use Input.RenderInput and Input.CachedInput only. These properties hold input for render updates.
        // Input.RenderInput holds input for current render frame.
        // Input.CachedInput holds combined input for all render frames from last fixed update. This property will be used to set input for next fixed update (polled by Fusion).

        PlayerInput input = Owner.Input;

        // Get look rotation from last fixed update (not last render!)
        Vector2 lookRotation = KCC.FixedData.GetLookRotation(true, true);

        // For correct look rotation, we have to apply deltas from all render frames since last fixed update => stored in Input.CachedInput
        // Additionally we have to apply pending look rotation delta maintained in fixed update, resulting in pending look rotation delta dedicated to render update.
        _renderLookRotationDelta = KCCUtility.GetClampedLookRotationDelta(lookRotation, _pendingLookRotationDelta + input.CachedInput.LookRotationDelta, -MaxCameraAngle, MaxCameraAngle);

        bool updateLookRotation = default;
        Quaternion facingRotation = default;
        Quaternion jumpRotation = default;

        // Checking look rotation update conditions. These check are done agains Input.CachedInput, because any render input accumulated since last fixed update will trigger look rotation update in next fixed udpate.
        if (HasLookRotationUpdateSource(ELookRotationUpdateSource.Jump) == true) { updateLookRotation |= input.WasPressed(EInputButtons.Jump) == true; }
        if (HasLookRotationUpdateSource(ELookRotationUpdateSource.Movement) == true) { updateLookRotation |= input.CachedInput.MoveDirection.IsZero() == false; }
        if (HasLookRotationUpdateSource(ELookRotationUpdateSource.MouseHold) == true) { updateLookRotation |= input.IsSet(EInputButtons.AltFire) == true; }
        if (HasLookRotationUpdateSource(ELookRotationUpdateSource.MouseMovement) == true) { updateLookRotation |= input.CachedInput.LookRotationDelta.IsZero() == false; }

        if (updateLookRotation == true) {
            // Some conditions are met, we can apply pending render look rotation delta to KCC
            if (_renderLookRotationDelta.IsZero() == false) {
                KCC.SetLookRotation(lookRotation + _renderLookRotationDelta);
            }
        }

        if (updateLookRotation == true || _faceMoveDirection == false) {
            // Setting base facing and jump rotation
            facingRotation = KCC.RenderData.TransformRotation;
            jumpRotation = KCC.RenderData.TransformRotation;
        }

        Vector3 inputDirection = default;
        bool hasInputDirection = default;
        bool faceOnMouseHold = _faceMoveDirection;

        // Do we have move direction for this render frame? Use it.
        Vector3 moveDirection = input.RenderInput.MoveDirection.X0Y();
        if (moveDirection.IsZero() == false) {
            hasInputDirection = true;
            inputDirection = KCC.RenderData.TransformRotation * moveDirection;
        }

        KCC.SetInputDirection(inputDirection);

        // There is no move direction for current render input. Do we have cached move direction (accumulated in frames since last fixed update)? Then use it. It will be used next fixed update after Fusion polls new input.
        Vector3 cachedMoveDirection = input.CachedInput.MoveDirection.X0Y();
        if (hasInputDirection == false && cachedMoveDirection.IsZero() == false) {
            hasInputDirection = true;
            inputDirection = KCC.RenderData.TransformRotation * cachedMoveDirection;
        }

        // Do we have any input direction (from this frame or all frames since last fixed update)? Use it.
        if (hasInputDirection == true) {
            Quaternion inputRotation = Quaternion.LookRotation(inputDirection);

            // We are moving in certain direction, we'll use it also for jump.
            jumpRotation = inputRotation;

            // Facing move direction enabled and right mouse button rotation lock disabled? Treat input rotation as facing as well.
            if (faceOnMouseHold == true && (_mouseHoldRotationPriority == false || input.IsSet(EInputButtons.AltFire) == false)) {
                facingRotation = inputRotation;
            }
        }

        // Jump is extrapolated for render as well.
        // Checking Input.CachedInput here. Jump accumulated from render inputs since last fixed update will trigger similar code next fixed update.
        // We have to keep the visual to face the direction if there is a jump pending execution in fixed update.
        if (input.WasPressed(EInputButtons.Jump) == true) {
            // Is jump rotation invalid (not set)? Get it from other source.
            if (jumpRotation.IsZero() == true) {
                // Is facing rotation valid? Use it.
                if (facingRotation.IsZero() == false) {
                    jumpRotation = facingRotation;
                }
                else {
                    // Otherwise just jump forward.
                    jumpRotation = KCC.RenderData.TransformRotation;
                }
            }

            // Is facing rotation invalid (not set)? Set it to the same rotation as jump.
            if (facingRotation.IsZero() == true) {
                facingRotation = jumpRotation;
            }

            if (input.WasPressed(EInputButtons.Jump) == true) {
                KCC.Jump(jumpRotation * JumpImpulse);
            }
        }


        //SPRINT PSRINT PSRINT!!!!
        // Notice we are checking KCC.RenderData because we are in render update code path (fixed update uses KCC.FixedData)
        //if (KCC.RenderData.IsGrounded == true) {
        //    // Sprint is updated only when grounded
        //    KCC.SetSprint(Input.CachedInput.Sprint);
        //}

        // Is facing rotation set? Apply to the visual.
        if (facingRotation.IsZero() == false) {
            //Visual.Root.rotation = facingRotation;
        }

        // At his point, KCC haven't been updated yet (except look rotation, which propagates immediately) so camera have to be synced later.
        // Because this is advanced implementation, base class triggers manual KCC update immediately after this method.
        // This allows us to synchronize camera in OnEarlyRender(). To keep consistency with fixed update, camera related properties are updated in regular render update - OnRender().
    }

   
    protected override void OnRender() {
        // Regular render update

        // For render we care only about input authority.
        // This can be extended to state authority if needed (inner code won't be executed on host for other players, having camera pivots to be set only from fixed update, causing jitter if spectating that player)
        if (Object.HasInputAuthority == true) {
            Vector2 pitchRotation = KCC.FixedData.GetLookRotation(true, false);
            Vector2 clampedCameraRotation = KCCUtility.GetClampedLookRotation(pitchRotation + _renderLookRotationDelta, -MaxCameraAngle, MaxCameraAngle);

            CameraPivot.rotation = KCC.FixedData.TransformRotation * Quaternion.Euler(clampedCameraRotation);
        }

        if (_faceMoveDirection == true && Object.HasInputAuthority == false) {
            // Facing rotation for visual is already set on input authority, here we update proxies and state authority based on [Networked] property.

            float interpolatedFacingMoveRotation = _facingMoveRotation;

            if (_facingMoveRotationInterpolator.TryGetValues(out float fromFacingMoveRotation, out float toFacingMoveRotation, out float alpha) == true) {
                // Interpolation which correctly handles circular range (-180 => 180)
                interpolatedFacingMoveRotation = KCCMathUtility.InterpolateRange(fromFacingMoveRotation, toFacingMoveRotation, -180.0f, 180.0f, alpha);
            }

            //Visual.Root.rotation = Quaternion.Euler(0.0f, interpolatedFacingMoveRotation, 0.0f);
        }

        Weapons.OnRender();
    }

    protected override void OnLateRender() {
        // Setting base camera transform based on handle

        // For render we care only about input authority.
        // This can be extended to state authority if needed (inner code won't be executed on host for other agents, having camera pivots to be set only from fixed update, causing jitter if spectating that player)
        if (HasInputAuthority == true) {
            Vector2 pitchRotation = KCC.RenderData.GetLookRotation(true, false);
            CameraPivot.localRotation = Quaternion.Euler(pitchRotation);

            var cameraTransform = MainCamera.transform; //ATENTIE AICI! DACA NU SE MISCA CAMERA, APPLICA DIRECT PE MAIN CAMERA MODIFICARILE!

            cameraTransform.position = CameraHandle.position;
            cameraTransform.rotation = CameraHandle.rotation;
        }
    }
    // PRIVATE METHODS

    private bool HasLookRotationUpdateSource(ELookRotationUpdateSource source) {
        return (_lookRotationUpdateSource & source) == source;
    }

    // DATA STRUCTURES

    [Flags]
    private enum ELookRotationUpdateSource {
        Jump = 1 << 0, // Look rotation is updated on jump
        Movement = 1 << 1, // Look rotation is updated on character movement
        MouseHold = 1 << 2, // Look rotation is updated while holding right mouse button
        MouseMovement = 1 << 3, // Look rotation is updated on mouse move
        Dash = 1 << 4, // Look rotation is updated on dash
    }

}
