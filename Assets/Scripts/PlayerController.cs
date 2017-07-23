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
    public float extraGravity = 0.4f;
    public float cooldownFire = 0.3f;

    private float jumpTime = 0.0f; // How long you can still hold jump.
    private bool jumping = false;
    private bool canDoubleJump = true;
    private bool hasDoubleJumped = false;
    private bool hasStoppedJumping = false;
    private Vector3? lastRotation = null;
    private Transform m_Cam; // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward; // The current forward direction of the camera
    private Vector3 m_move;
    private bool m_Jump; // the world-relative desired move direction, calculated from the camForward and user input.
    private bool firePressed;
    private bool temperatureGunPressed;
    private Rigidbody rigidBody;
    private PlayerAttributes playerAttributes = new PlayerAttributes();
    private float nextTimeCanFire = 0.0f;

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
        firePressed = CrossPlatformInputManager.GetButton("Fire1");
        temperatureGunPressed = CrossPlatformInputManager.GetButton("TemperatureGun");
    }

    // Fixed update is called in sync with physics
    private void FixedUpdate() {
        // read inputs
        var Camera = GameObject.Find("Camera");
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        float angle = Mathf.Atan2(v, h) * -180 / Mathf.PI + Camera.transform.eulerAngles.y + 90;
        float magnitude = Mathf.Sqrt(h*h + v*v);

        handleJumping();
        handleFiring();
        handleTemperatureGun();

        if (magnitude > 0.1) {
            transform.eulerAngles = new Vector3(0f, angle, 0f);
            lastRotation = null;
        } else if (!lastRotation.HasValue) {
            lastRotation = transform.eulerAngles;
        } else {
            // If the user isn't actively setting the rotation, keep it locked to whatever it was last
            // to avoid getting spun around by stuff that hits us.
            transform.eulerAngles = lastRotation.Value;
        }

        // Move forward
        m_move = new Vector3(0, 0, 1) * movementSpeed * magnitude * Time.deltaTime;

        // pass all parameters to the character control script
        transform.Translate (m_move, Space.Self);
    }

    void handleFiring() {
        if (firePressed && Time.time > nextTimeCanFire) {
            // Set Firing Cooldown
            nextTimeCanFire = Time.time + cooldownFire;

            // Shoot projectile.
            GameObject projectile = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), transform.position+transform.forward, Quaternion.identity) as GameObject;
            projectile.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            projectile.tag = "enemy_damaging";
            projectile.AddComponent<Rigidbody>();
            projectile.GetComponent<Rigidbody>().useGravity = false;
            projectile.GetComponent<Rigidbody>().isKinematic = false;
            projectile.GetComponent<Rigidbody>().velocity = transform.forward * 10.0f;
            Destroy(projectile, 5.0f);
        }
    }

    void handleTemperatureGun() {
        if (temperatureGunPressed && Time.time > nextTimeCanFire) {
            nextTimeCanFire = Time.time + cooldownFire;
             Debug.DrawRay(transform.position, transform.forward, Color.green);
            if (temperatureGunPressed) {
                RaycastHit objectHit;
                /* bool hitSomething = Physics.Raycast (transform.position, transform.forward, out objectHit, 15.00f); */
                bool hitSomething = Physics.Raycast (transform.position, new Vector3 (0, -1.0f, 0), out objectHit, 15.00f);
                if (hitSomething) {
                    if (objectHit.transform.CompareTag("ice")) {
                        objectHit.transform.tag = "water";
                        objectHit.transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
                        objectHit.transform.gameObject.GetComponent<MeshCollider>().isTrigger = true;
                        GameObject fireEffect = Instantiate(Resources.Load("Prefabs/HeatEffect"), transform.position+transform.forward, Quaternion.identity) as GameObject;
                        Destroy(fireEffect, 3.0f);
                    } else if (objectHit.transform.CompareTag("water")) {
                        // Ice to meet you
                        objectHit.transform.tag = "ice";
                        objectHit.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        objectHit.transform.gameObject.GetComponent<MeshCollider>().isTrigger = false;
                        GameObject iceEffect = Instantiate(Resources.Load("Prefabs/IceEffect"), transform.position+transform.forward, Quaternion.identity) as GameObject;
                        Destroy(iceEffect, 3.0f);
                    }
                }
            }
        }
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
        if(collision.gameObject.tag.Equals("player_damaging")) {
            // Calculate Angle Between the collision point and the player
            Vector3 forceDirection = collision.contacts[0].point - transform.position;
            forceDirection = -forceDirection.normalized;

            damagePlayer(3);
            /* pushPlayer(forceDirection*1000.0f); */

            Destroy(collision.gameObject);
        }
    }
}
