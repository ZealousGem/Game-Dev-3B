using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CaermaMovemtn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Camera cam;

    float MaxZoom = 8.07f; // allows camera to zoom 

    float MinZoom = 13.73183f;

    Vector3 origin; // mouse input location 

    float plain;  // speed of movement 

    public float speed = 10;

    public float minX = 0, minZ = 0, MaxX = 70, MaxZ = 40;  // borders to constraint camera so it does not go to edge of the terrain map 
    
    bool dragging = false;

    bool isGameEnd = false;

    void Start()
    {
        cam = Camera.main;
        plain = 0;


    }

    void OnEnable()
    {
         EventBus.Subscribe<EndGameEvent>(getEndDate);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<EndGameEvent>(getEndDate);
    }

    void getEndDate(EndGameEvent data)
    {

        switch (data.type)
        {

            case StatsChange.EndGame: EndGame(); break;
            case StatsChange.PausedGame: EndGame(); break;
            case StatsChange.UnPausedGame: UnPause(); break;

        }
       
            
    }

    void EndGame() // makes the camera stop mivng 
    {
        isGameEnd = true;
       
    }

    void UnPause() // lets camera move again 
    {
        isGameEnd = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && !isGameEnd) // gets mouse postion to allow drag 
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



        if (dragging) // caluclates the mouses location moves the camera based on the mouses movement and changed location  
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

    Vector3 FindWorldPosition(Vector3 scr) // finds the position of the mouse 
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

    void ZoominCamera() // allows the camera to zoom in and zoom out based on the max and min zoom contrains 
    {
        float scrollInput = Mouse.current.scroll.ReadValue().y;

        if (scrollInput != 0) {
            cam.orthographicSize = cam.orthographicSize - scrollInput * speed * Time.deltaTime * 5;
        }
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, MaxZoom,  MinZoom);
    }
    
 

}
