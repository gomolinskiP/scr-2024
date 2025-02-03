using UnityEngine;

public class followPlayer : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float MouseSpeed = 600;
    [SerializeField] float orbitDamping = 10;
    Vector3 localRotation;
    Camera cam;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float scrollZoom = Input.mouseScrollDelta.y;
        cam.orthographicSize += scrollZoom;
        localRotation.y += scrollZoom;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 2.5f, 40f);

        transform.position = Player.position;
        // localRotation.x += Input.GetAxis("Mouse X") * MouseSpeed;
        // localRotation.y += Input.GetAxis("Mouse Y") * MouseSpeed;

        if(Input.GetKey(KeyCode.Comma)){
            localRotation.x -= 1f;
        }
        else if(Input.GetKey(KeyCode.Period)){
            localRotation.x += 1f;
        }

        //slowly rotate anyway
        localRotation.x += 0.01f;

        localRotation.y = Mathf.Clamp(localRotation.y, -30f, 5f);
        
        Quaternion QT = Quaternion.Euler(localRotation.y, localRotation.x, localRotation.y);
        transform.rotation = Quaternion.Lerp(transform.rotation, QT, Time.deltaTime * orbitDamping);
    }
}
