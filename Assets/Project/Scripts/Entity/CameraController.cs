using UnityEngine;
using Memoria.Constants;

namespace Memoria.Entity
{
    public class CameraController : MonoBehaviour
    {
        // Inspector から参照を設定
        [SerializeField] private GameObject player;
        private CameraStates.State currentState = CameraStates.State.ThirdPerson;

        private Vector3 distance;
        public float followSpeed = 5f;

        void Awake()
        {
            distance = player.transform.position - this.transform.position;
        }

        void LateUpdate()
        {
            switch (currentState)
            {
                case CameraStates.State.FirstPerson:
                    FirstPersonCamera();
                    break;
                case CameraStates.State.ThirdPerson:
                    ThirdPersonCamera();
                    break;
                case CameraStates.State.FreeCamera:
                    FreeCamera();
                    break;
            }
        }

        private void FirstPersonCamera()
        {

        }

        private void ThirdPersonCamera()
        {
            transform.position = Vector3.Lerp(this.transform.position, player.transform.position - distance, Time.deltaTime * followSpeed);
        }

        private void FreeCamera()
        {

        }
    }
}
