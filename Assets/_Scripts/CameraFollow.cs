//This code was originally written by Jared Slawski 
//on Oct 5th, 2020.  It has been repurposed for use
//in this project.
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;

    [HideInInspector]
    public Player playerCreature;
   
    private Camera thisCamera;
    private Transform cameraTransform;
    private float cameraDistance;

    [SerializeField, Range(0, 1)]
    public float upperVerticalViewportThreshold = 0.8f;
    [SerializeField, Range(0, 1)]
    public float lowerVerticalViewportThreshold = 0.3f;
    [SerializeField, Range(0, 1)]
    public float horizontalViewportThreshold = 0.5f;

    private Vector3 targetPosition = Vector3.zero;

    void Awake()
    {
        CameraFollow.instance = this;
        this.thisCamera = this.gameObject.GetComponent<Camera>();
        this.cameraTransform = this.gameObject.transform;
        this.cameraDistance = this.cameraTransform.position.z;
    }

    private bool IsPlayerPastHorizontalThreshold(float playerViewportXPosition)
    {
        return (playerViewportXPosition >= (1.0f - this.horizontalViewportThreshold)) ||
            (playerViewportXPosition <= (0.0f + this.horizontalViewportThreshold));
    }

    private bool IsPlayerPastVerticalThreshold(float playerViewportYPosition)
    {
        return (playerViewportYPosition >= this.upperVerticalViewportThreshold) ||
            (playerViewportYPosition <= this.lowerVerticalViewportThreshold);
    }

    void Update()
    {
        if (this.playerCreature == null)
        {
            return;
        }

        this.targetPosition = 
            new Vector3(this.playerCreature.transform.position.x,
            this.playerCreature.transform.position.y,
            -10.0f);
        
        /*
        Vector3 playerViewportPosition = thisCamera.WorldToViewportPoint(this.playerCreature.gameObject.transform.position);

        if (this.playerCreature != null && this.IsPlayerPastVerticalThreshold(playerViewportPosition.y))
        {
            this.UpdateCameraVerticalPosition(playerViewportPosition);
        }

        if (this.IsPlayerPastHorizontalThreshold(playerViewportPosition.x))
        {
            this.UpdateCameraHorizontalPosition();
        }
        */
    }

    private void LateUpdate()
    {
        this.cameraTransform.position = this.targetPosition;
    }

    private void UpdateCameraVerticalPosition(Vector3 playerViewportPosition)
    {
        Vector3 worldSpaceCenteredPosition;

        if (playerViewportPosition.y >= 0.5f)
        {
            worldSpaceCenteredPosition = this.thisCamera.ViewportToWorldPoint(new Vector3(0.5f, this.upperVerticalViewportThreshold, this.cameraDistance));
        }
        else
        {
            worldSpaceCenteredPosition = this.thisCamera.ViewportToWorldPoint(new Vector3(0.5f, this.lowerVerticalViewportThreshold, this.cameraDistance));
        }

        Vector3 shiftVector = new Vector3(0, this.playerCreature.transform.position.y - worldSpaceCenteredPosition.y, 0);

        this.targetPosition = (shiftVector.normalized * Mathf.Abs(this.playerCreature.creatureRb.velocity.y) * Time.fixedDeltaTime);
    }

    private void UpdateCameraHorizontalPosition()
    {
        Vector3 worldSpaceCenteredPosition = this.thisCamera.ViewportToWorldPoint(new Vector3(0.5f, this.upperVerticalViewportThreshold, this.cameraDistance));

        Vector3 shiftVector = new Vector3(this.playerCreature.transform.position.x - worldSpaceCenteredPosition.x, 0, 0);

        this.targetPosition = (shiftVector.normalized * Mathf.Abs(this.playerCreature.creatureRb.velocity.x) * Time.fixedDeltaTime);
    }
}