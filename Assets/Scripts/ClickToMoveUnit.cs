using UnityEngine;

public class ClickToMoveUnit : Unit
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;   
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("RAY");
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
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