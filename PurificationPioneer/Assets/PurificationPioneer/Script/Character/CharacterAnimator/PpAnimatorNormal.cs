using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    /// <summary>
    /// 各个动画状态直接跳转的动画控制逻辑
    /// </summary>
    public class PpAnimatorNormal:DefaultAnimator
    {
        [SerializeField]private Animator animator;
        [SerializeField]private string dieTriggerKey="die";
        [SerializeField]private string runTriggerKey="run";
        [SerializeField]private string idleTriggerKey="idle";
        protected override void Start()
        {
            base.Start();
            Assert.IsNotNull(animator);
            Assert.IsFalse(string.IsNullOrEmpty(dieTriggerKey));
            Assert.IsFalse(string.IsNullOrEmpty(runTriggerKey));
            Assert.IsFalse(string.IsNullOrEmpty(idleTriggerKey));
        }
        public override void ToIdle()
        {
            base.ToIdle();
            Debug.LogWarning($"[PpAnimatorNormal] ToIdle");
            animator.SetTrigger(idleTriggerKey);
        }

        public override void ToDie()
        {
            base.ToDie();
            animator.SetTrigger(dieTriggerKey);
        }

        public override void ToMove()
        {
            base.ToMove();
            Debug.LogWarning($"[PpAnimatorNormal] ToMove");
            animator.SetTrigger(runTriggerKey);
        }
    }
}