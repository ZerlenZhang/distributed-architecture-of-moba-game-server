using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class UVOffset : MonoBehaviour {
    public float scrollSpeed = 0.5f;
    public bool scrollY = true;
    private new MeshRenderer renderer;

    void Start() {
        renderer = GetComponent<MeshRenderer>();
    }

    void Update() {
        float offset = Time.time * scrollSpeed;
        renderer.material.SetTextureOffset("_MainTex", scrollY ?   new Vector2(offset, 0) : new Vector2(0,offset));
    }
}
