using UnityEngine;

namespace BlazeAISpace
{
    public class SetWayPointToPosition : MonoBehaviour
    {
        BlazeAI blaze;

        // Start is called before the first frame update
        void Start()
        {
            blaze = GetComponent<BlazeAI>();
        }

        // Update is called once per frame
        void Update()
        {
            blaze.waypoints.waypoints[0] = transform.position;
        }
    }
}

