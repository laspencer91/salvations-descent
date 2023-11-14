using UnityEngine;

public class FPSMouseLook : MonoBehaviour 
{
    [Header("Mouse Look")]
    [SerializeField] float m_LookSensitivity = 5.0f;
    [SerializeField] float m_LookSmoothing = 2.0f;
    [SerializeField] float m_PitchMin = -90;
    [SerializeField] float m_PitchMax = 90;

    // Private Mouse Look
    private Vector2 smoothV;
    private Vector2 mouseLook;

    // Components
    private Camera _camera;

    [HideInInspector] public Quaternion cachedRotation;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _camera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (GameManager.GetGameState() != GameState.GameOver)
            MouseLook();
    }

    public void MouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        var mouseChange = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        var smoothedSens = m_LookSensitivity * m_LookSmoothing;

        mouseChange = Vector2.Scale(mouseChange, new Vector2(smoothedSens, smoothedSens));

        smoothV.x = Mathf.Lerp(smoothV.x, mouseChange.x, 1f / m_LookSmoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, mouseChange.y, 1f / m_LookSmoothing);

        mouseLook  += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, m_PitchMin, m_PitchMax);

        // Horizontal Rotation: Rotate body. Not Camera
        transform.localRotation = Quaternion.AngleAxis(mouseLook.x, Vector3.up);
        cachedRotation = transform.localRotation;

        // Vertical Rotation. Camera Only
        Vector3 camEuler = _camera.transform.localEulerAngles;
        _camera.transform.localEulerAngles = new Vector3(-mouseLook.y, camEuler.y, 0);
    }
}