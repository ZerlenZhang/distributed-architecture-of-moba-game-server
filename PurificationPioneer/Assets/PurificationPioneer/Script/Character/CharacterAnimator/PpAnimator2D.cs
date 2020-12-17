using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class PpAnimator2D:DefaultAnimator
    {
        [SerializeField]private Animator animator;
        [SerializeField]private string horizontalFloatKey;
        [SerializeField]private string verticalFloatKey;
        [SerializeField]private string moveSpeedKey;
        [SerializeField]private string toDieTrigger;

        protected override void Start()
        {
            base.Start();
            Assert.IsNotNull(animator);
            Assert.IsFalse(string.IsNullOrEmpty(horizontalFloatKey));
            Assert.IsFalse(string.IsNullOrEmpty(verticalFloatKey));
            Assert.IsFalse(string.IsNullOrEmpty(moveSpeedKey));
            Assert.IsFalse(string.IsNullOrEmpty(toDieTrigger));
        }

        public override void ToDie()
        {
            base.ToDie();
            animator.SetTrigger(toDieTrigger);
        }

        public override void ToIdle()
        {
            base.ToIdle();
            animator.SetFloat(horizontalFloatKey, 0);
            animator.SetFloat(verticalFloatKey, 0);
        }

        public override void ToMove(float x, float y, float speed)
        {
            base.ToMove(x, y, speed);
            animator.SetFloat(horizontalFloatKey, x);
            animator.SetFloat(verticalFloatKey, y);
            animator.SetFloat(moveSpeedKey, speed);
        }
    }
}