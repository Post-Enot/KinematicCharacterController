using UnityEngine;

namespace PostEnot.Toolkits
{
    public static class MovementUtility
    {
        public static bool IsFacingTo(Vector3 normal, Vector3 forwardDirection, float maxAngle)
        {
            float angle = Vector3.Angle(forwardDirection, -normal.normalized);
            return angle <= maxAngle;
        }

        /// <summary>
        /// Calculates the velocity required to move the character to the target position over a specific deltaTime.
        /// Useful for when you wish to work with positions rather than velocities in the UpdateVelocity callback 
        /// </summary>
        public static Vector3 GetVelocityForMovePosition(Vector3 fromPosition, Vector3 toPosition, float deltaTime)
            => GetVelocityFromMovement(toPosition - fromPosition, deltaTime);

        public static Vector3 GetVelocityFromMovement(Vector3 movement, float deltaTime)
            => deltaTime <= 0.0f ? Vector3.zero : movement / deltaTime;

        public static float ApplySpeedStep(int speedStep, int stepsCount, float maxSpeed, float startFrom)
        {
            float minSpeed = maxSpeed * startFrom;
            float t = speedStep / (float)stepsCount;
            return Mathf.Lerp(minSpeed, maxSpeed, t);
        }

        public static Vector3 CalculateSlidingVelocity(Vector3 slidingVelocity, Vector2 movement, Vector3 surfaceNormal)
        {
            Vector3 movement3 = new(movement.x, 0.0f, movement.y);
            Vector3 movementProjection = ProjectionWithMagnitudeSaving(movement3, surfaceNormal);
            movementProjection -= Vector3.Project(movementProjection, slidingVelocity);
            return slidingVelocity + movementProjection;
        }

        public static Vector3 ProjectMovementOnSurface(Vector2 movement, Vector3 surfaceNormal)
        {
            Vector3 movement3 = new(movement.x, 0.0f, movement.y);
            return ProjectionWithMagnitudeSaving(movement3, surfaceNormal);
        }

        public static Vector2 InputToMovement(
            Vector2 input,
            Vector3 forward,
            float factor,
            float speed)
        {
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            Vector3 movement3 = (forward * input.y) + (right * input.x);
            Vector2 movement2 = new(movement3.x, movement3.z);
            Vector2 forward2 = new(forward.x, forward.z);
            return ApplyBackwardMovementFactor(movement2, forward2, factor, speed);
        }

        public static Vector2 ApplyBackwardMovementFactor(
            Vector2 movementDirection,
            Vector2 forward,
            float factor,
            float speed)
        {
            float dot = Vector2.Dot(forward, movementDirection);
            dot = (dot + 1) / 2;
            float mult = 1 - dot;
            speed -= speed * mult * factor;
            return movementDirection * speed;
        }

        public static Vector3 ProjectionWithMagnitudeSaving(Vector3 vector, Vector3 normal)
        {
            float num = Vector3.Dot(in normal, in normal);
            if (num < Mathf.Epsilon)
            {
                return vector;
            }

            float num2 = Vector3.Dot(in vector, in normal) / num;
            Vector3 projection = default;
            projection.x = vector.x - (normal.x * num2);
            projection.y = vector.y - (normal.y * num2);
            projection.z = vector.z - (normal.z * num2);

            float vectorMagnitude = vector.magnitude;
            float projectionMagnitude = projection.magnitude;
            if (projectionMagnitude < Mathf.Epsilon)
            {
                return Vector3.zero;
            }
            float scale = vectorMagnitude / projectionMagnitude;
            return projection * scale;
        }
    }
}
