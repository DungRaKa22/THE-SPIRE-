using UnityEngine;

namespace JumpDummy
{
    // Implemented by anything that carries the player while it moves: pistons, conveyors,
    // drones. DummyController measures "not falling" as velocity RELATIVE to the surface
    // (Docs/TheSpire-Level-Sector2.md risk #1), so an ascending piston must report its
    // upward velocity here or charging on it would stay impossible.
    //
    // The implementing component must sit on (or above) the collider the player stands on,
    // and the platform root must carry a kinematic Rigidbody2D driven by MovePosition —
    // a static collider teleported by transform alone is invisible to ground casts.
    public interface IMovingSurface
    {
        // Velocity of the contact surface at Time.time.
        Vector2 SurfaceVelocity { get; }
    }
}
