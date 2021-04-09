using UnityEngine;

namespace DialogSystem.Scripts
{
    public class DialogData : UnityEngine.MonoBehaviour
    {
        public Vector3 offect=new Vector3(-1,1.58f,0);
        public float yWeight = 0.3f;



        public Vector3 GetSuitPos(Transform targetPos)
        {
            return
                targetPos.position+new Vector3(this.offect.x,this.offect.y + targetPos.transform.localScale.y*yWeight,0);
        }

        public Vector3 GetSuitPos()
        {            
            return transform.position;
        }
    }
}