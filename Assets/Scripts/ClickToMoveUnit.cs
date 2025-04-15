using UnityEngine;

public class ClickToMoveUnit : Unit
{
    private Camera mainCamera;
    [SerializeField] private LayerMask groundLayer; 

    void Start()
    {
        mainCamera = Camera.main;   
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                if (target != null && target.name == "TempTarget")
                    Destroy(target.gameObject);

                GameObject targetObj = new GameObject("TempTarget");
                targetObj.transform.position = hit.point;
                target = targetObj.transform;
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            }
        }
    }

    void OnDestroy()
    {
        if (target != null && target.name == "TempTarget")
        {
            Destroy(target.gameObject);
        }
    }
}