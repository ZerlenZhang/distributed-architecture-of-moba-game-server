using UnityEngine;

namespace ReadyGamerOne.Common
{
    public interface IUnityGameObject
    {
        GameObject gameObject { get; }
        Transform transform { get; }
        Vector3 position { get; set; }        
        Vector3 localPosition { get; set; }
    }
}