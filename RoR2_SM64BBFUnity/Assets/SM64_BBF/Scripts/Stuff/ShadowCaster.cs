using UnityEngine;

namespace SM64BBF.Stuff
{
    public class ShadowCaster : MonoBehaviour
    {
        public GameObject shadow;
        public LayerMask mask;

        public float floorMargin;
        public bool alignWithNormal;
        Renderer shadowRenderer;

        // Use this for initialization
        void Start()
        {
            shadowRenderer = shadow.GetComponent<Renderer>();
            RenderShadow();
        }

        //private void Update()
        //{
        //    RenderShadow();
        //}

        private void RenderShadow()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100, mask.value))
            {

                shadow.transform.position = hit.point + (Vector3.up * floorMargin);
                shadow.transform.position = new Vector3(shadow.transform.position.x, shadow.transform.position.y + 0.2f, shadow.transform.position.z);

                if (alignWithNormal)
                {
                    transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }
                else
                {
                    transform.rotation = Quaternion.identity;
                }

            }
        }
    }
}
