using UnityEngine;

public class GrapplingGun : MonoBehaviour {

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, cameras, player;
    private float maxDistance = 50f;
    private SpringJoint joint;
    public bool leftMouse, rightMouse;

    void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && leftMouse) {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0) && leftMouse) {
            StopGrapple();
        }
        if (Input.GetMouseButtonDown(1) && rightMouse)
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(1) && rightMouse)
        {
            StopGrapple();
        }
    }

    //Called after Update
    void LateUpdate() {
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(cameras.position, cameras.forward, out hit, maxDistance, whatIsGrappleable)) {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.75f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 8f;
            joint.damper = 5f;
            //Adjust these values to fit your game.
            if (Input.GetKey(KeyCode.LeftShift))
            {
                joint.maxDistance = distanceFromPoint * 0.2f;
                joint.minDistance = distanceFromPoint * 0.1f;
                joint.spring = 24f;
                joint.damper = 15f;
            }
            
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple() {
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;
    
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 4f);
        
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}
