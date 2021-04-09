using UnityEngine;

namespace UnityTemplateProjects.Test
{
    public enum ComponentType
    {
        None,
        Rigidbody,
        Character
    }
    public enum MoveType
    {
        RigidbodyPos,
        RigidbodyMovePosition,
        CharacterMove,
        CharacterSimpleMove,
    }
    public class TestPhysics : MonoBehaviour
    {
        [Header("Basic")]
        public Rigidbody player;
        public CharacterController characterController;
        public float deltaTime = 0.05f;
        public bool enableLog = true;

        [Header("Rigidbody MoveRight")] 
        public ComponentType componentType;
        public float speed = 2f;
        public MoveType moveType;

        private float lastTime;
        private void Start()
        {
            lastTime = Time.timeSinceLevelLoad;
            InvokeRepeating("LogicUpdate", deltaTime, deltaTime);
        }

        public void LogicUpdate()
        {
            var delta = Time.timeSinceLevelLoad - lastTime;
            if (enableLog)
            {
                Debug.Log($"[Time-{delta}");
            }
            if (componentType==ComponentType.Rigidbody && player)
            {
                var targetPos=player.position+speed * deltaTime * Vector3.right;
                switch (moveType)
                {
                    case MoveType.RigidbodyMovePosition:
                        player.MovePosition(targetPos);
                        break;
                    case MoveType.RigidbodyPos:
                        player.position = targetPos;
                        break;
                }
            }
            else if (componentType == ComponentType.Character && characterController)
            {
                var motion = speed * deltaTime * Vector3.right;
                switch (moveType)
                {
                    case MoveType.CharacterMove:
                        characterController.Move(motion);
                        break;
                    case MoveType.CharacterSimpleMove:
                        characterController.SimpleMove(speed * Vector3.right);
                        break;
                }
            }
        }
    }
}