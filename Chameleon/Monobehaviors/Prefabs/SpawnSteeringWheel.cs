using Chameleon.Monobehaviors.Cyclops;
using UnityEngine;

namespace Chameleon.Monobehaviors.Prefabs
{
    internal class SpawnSteeringWheel : MonoBehaviour, ICyclopsReferencer
    {
        public void OnCyclopsReferenceFinished(GameObject cyclops)
        {
            GameObject steeringWheel = cyclops.transform.Find("CyclopsMeshAnimated/Submarine_Steering_Console").gameObject;
            GameObject instantiatedWheel = GameObject.Instantiate(steeringWheel, transform);
            instantiatedWheel.transform.localPosition = Vector3.zero;
            instantiatedWheel.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);

            PilotingChair chair = instantiatedWheel.GetComponent<PilotingChair>();
            chair.subRoot = gameObject.GetComponentInParent<SubRoot>(true);

            //Top 10 micro optimizations no one cares about
            chair.subRoot.GetComponent<SubControl>().mainAnimator = gameObject.GetComponentInChildren<Animator>();
        }
    }
}
