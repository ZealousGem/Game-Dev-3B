using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CaermaMovemtn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Camera cam;

    float MaxZoom = 8.07f;

    float MinZoom = 13.73183f;

    Vector3 origin;

    float plain;

    Vector3 diff;

   

    public float speed = 10;

  public float minX = 0, minZ = 0, MaxX = 70, MaxZ = 40;
    
    bool dragging = false;

    void Start()
    {
        cam = Camera.main;
        plain = 0;


    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                origin = FindWorldPosition(Mouse.current.position.ReadValue());
                dragging = true;
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                dragging = false;
            }
        }

        else
        {
            dragging = false;
        }
    }



    void LateUpdate()
    {



        if (dragging)
        {
            Vector3 world = FindWorldPosition(Mouse.current.position.ReadValue());
            Vector3 delta = origin - world;

            transform.position += delta;
        }

        ZoominCamera();
        Vector3 curpos = transform.position;
        curpos.x = Mathf.Clamp(curpos.x, minX, MaxX);
        curpos.z = Mathf.Clamp(curpos.z, minZ, MaxZ);
        transform.position = curpos;

        
       
    }

    Vector3 FindWorldPosition(Vector3 scr)
    {

        Ray ray = cam.ScreenPointToRay(scr);
        Plane grdPlane = new Plane(Vector3.up, new Vector3(0, plain, 0));
        float dist;

        if (grdPlane.Raycast(ray, out dist))
        {

            return ray.GetPoint(dist);
        }
        return transform.position;

    }

    void ZoominCamera()
    {
        float scrollInput = Mouse.current.scroll.ReadValue().y;

        if (scrollInput != 0) {
            cam.orthographicSize = cam.orthographicSize - scrollInput * speed * Time.deltaTime * 5;
        }
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, MaxZoom,  MinZoom);
    }
    
 

}
