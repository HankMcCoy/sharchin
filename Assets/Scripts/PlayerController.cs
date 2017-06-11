using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/**
 * Controller for player movement and actions.
 */
public class PlayerController : MonoBehaviour {
    public float rotationSpeed = 100.0f;
    public float movementSpeed = 6.0f;
    public Vector3 jumpVelocity = new Vector3(0, 60.0f, 0);
    public float maxJumpTime = 0.10f; // How long maximum jump hold time.
    public float minJumpTime = 0.05f; // How long maximum jump hold time.
    public float extraGravity = 0.25f;

    private float jumpTime = 0.0f; // How long you can still hold jump.
    private bool jumping = false;
    private bool canDoubleJump = true;
    private bool hasDoubleJumped = false;
    private bool hasStoppedJumping = false;
    private Transform m_Cam; // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward; // The current forward direction of the camera
    private Vector3 m_Move;
    private bool m_Jump; // the world-relative desired move direction, calculated from the camForward and user input.
    private Rigidbody rigidBody;
    private PlayerAttributes playerAttributes = new PlayerAttributes();

    public bool CanDoubleJump {
        get { return this.canDoubleJump; }
        set { this.canDoubleJump = value; }
    }

    public void damagePlayer(float damage) {
        // TODO: Add some sort of UI indication for taking damage.
        playerAttributes.decreaseHealth(damage);
    }

    public void pushPlayer(Vector3 force) {
        rigidBody.AddForce(force);
    }


    private void Start() {
        rigidBody = GetComponent<Rigidbody> ();
        // get the transform of the main camera
        if (Camera.main != null) {
            m_Cam = Camera.main.transform;
        }
        else {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }
    }


    private void Update() {
        m_Jump = CrossPlatformInputManager.GetButton("Jump");
    }

    // Fixed update is called in sync with physics
    private void FixedUpdate() {
        // read inputs
        float hRotation = CrossPlatformInputManager.GetAxis("Joystick X");
        float vRotation = CrossPlatformInputManager.GetAxis("Joystick Y");
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");

		handleJumping();

        // Move forward
        transform.Rotate(Vector3.up * rotationSpeed * vRotation * Time.deltaTime);
        m_Move = new Vector3(h, 0, v) * movementSpeed * Time.deltaTime;
        transform.Translate(m_Move, Space.Self);
    }

	void handleJumping() {
        RaycastHit groundHit;
        bool onGround = Physics.Raycast (transform.position, new Vector3 (0, -1.0f, 0), out groundHit, 1.01f);

		 if (onGround) {
            hasDoubleJumped = false;
            hasStoppedJumping = false;

            if (groundHit.transform.parent != null) {
                Transform parent = groundHit.transform.parent;
                transform.SetParent(parent);
            }
        } else {
            transform.SetParent(null);
        }

        // Jump if on ground.
        if (m_Jump && onGround) {
            jumpTime = 0;
            jumping = true;
        } else if (jumpTime > minJumpTime && !m_Jump) { // no longer jumping and min jump period is over.
            jumping = false;
        } else if (jumpTime > maxJumpTime) {
            jumping = false;
        }

        if (!onGround && !m_Jump) {
            hasStoppedJumping = true;
        }

        if(hasStoppedJumping && !hasDoubleJumped && m_Jump && !jumping) {
            rigidBody.velocity += new Vector3(0, -rigidBody.velocity.y, 0);
            hasDoubleJumped = true;
            jumping = true;
            jumpTime = 0;
        }

        if (jumping) {
            jumpTime += Time.deltaTime;
            rigidBody.velocity += jumpVelocity * Time.deltaTime;
        }

        // Add magic downward velocity if stopped jumping. To make you fall faster than you go up.
        if (rigidBody.velocity.y < 0) {
            rigidBody.velocity += new Vector3(0f, -extraGravity, 0f);
        }
	}

	/** Trigger on collision with player. */
    void OnCollisionEnter (Collision collision) {
		/* Console.log(col.gameObject.tag); */
        if(collision.gameObject.tag.Equals("damaging")) {
			// Calculate Angle Between the collision point and the player
			Vector3 forceDirection = collision.contacts[0].point - transform.position;
			forceDirection = -forceDirection.normalized;

			damagePlayer(3);
			pushPlayer(forceDirection*1000.0f);

            Destroy(collision.gameObject);
        }
    }
}
