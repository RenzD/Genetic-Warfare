using UnityEngine;

namespace UnityMovementAI
{
    public class ArriveUnit : MonoBehaviour
    {

        public Vector3 targetPosition;
        public GameObject target;

        SteeringBasics steeringBasics;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
        }

        void FixedUpdate()
        {
            Vector3 accel = steeringBasics.Arrive(target.transform.position);

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}