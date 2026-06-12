using UnityEngine;

public class Mining : MonoBehaviour
{
    public float Range = 5f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Range))
            {
                Mineable mineable = hit.collider.GetComponentInParent<Mineable>();

                if (mineable != null)
                {
                    mineable.Mine();
                }
            }
            
            
        }
    }
}
