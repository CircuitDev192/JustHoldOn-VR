using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VArmory;

public class TestDummyJoint : MonoBehaviour
{
    public Rigidbody anchorRb;
    //public Rigidbody AnchorRb { get { return anchorRb; } }

    protected ConfigurableJoint dummyJoint;
    protected Rigidbody rb;
    protected Collider col;
    [SerializeField] protected InteractionVolume iv;
    public InteractionVolume Iv { get { return iv; } }

    public ConfigurableJoint DummyJoint { get { return dummyJoint; } }
    public Rigidbody DummyRigidbody { get { return rb; } }
    public Collider DummyCollider { get { return col; } }

    protected TestDummyJoint rootBody;
    public TestDummyJoint RootBody { get { return rootBody; } }

    [SerializeField] public Transform defaultPinTransform;
    [SerializeField] public Transform pinTransform;

    public Transform PinTransform { get { return pinTransform ? pinTransform : defaultPinTransform; } }

    Vector3 initialRotation;

    public delegate void DamagedEvent();

    public DamagedEvent _OnDamaged;
    public DamagedEvent _OnKilled;

    public delegate void CollisionEvent(float magnitude);

    public CollisionEvent _OnCollision;

    protected float defaultPinWeight;
    protected float defaultMuscleWeight;

    protected virtual void Awake()
    {
        dummyJoint = GetComponent<ConfigurableJoint>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        iv = GetComponent<InteractionVolume>();

        if(dummyJoint)
        if(dummyJoint.connectedBody)
        initialRotation = dummyJoint.connectedBody.transform.eulerAngles - transform.eulerAngles;

        if (dummyJoint)
        {
            if(dummyJoint.connectedBody)
            rootBody = dummyJoint.connectedBody.GetComponent<TestDummyJoint>();

            angularXDrive = dummyJoint.angularXDrive.positionSpring;
            angularYZDrive = dummyJoint.angularYZDrive.positionSpring;

            if (rootBody == null)
            {
                anchorRb = dummyJoint.connectedBody;
            }
        }

        TwitchExtension.SetLayerRecursivly(gameObject, "Item");

        iv._StartInteraction += Grab;
        iv._EndInteraction += Release;

        rb.maxAngularVelocity = Mathf.Infinity;


        InitialAttachPoint = new GameObject(string.Format("InitialAttachPoint", this.gameObject.name)).transform;

        if (PinTransform)
        {
            Grab();
        }

        health = GetComponent<Health>();

        if (health)
        {
            health._OnDamaged += LoosenJoint;
            health._OnDamaged += OnDamaged;
        }
    }

    void OnDamaged(Health health)
    {
        if (!health) return;

        if(_OnDamaged != null)
            _OnDamaged();

        if (health.CurrentHealth <= 0)
            if(_OnKilled != null)
                _OnKilled();
    }

    void IgnoreCol()
    {
        if (rootBody)
            Physics.IgnoreCollision(col, rootBody.col);
    }

    public Transform InitialAttachPoint;
    Transform InitialRotationPoint;

    [SerializeField] protected float grabForce;
    [SerializeField] protected float grabAngularForce;
    [SerializeField] protected float twoHandGrabAngularForce;
    [SerializeField] protected float grabDrag;
    [SerializeField] protected float grabAngularDrag;

    [SerializeField] protected float releaseDrag;
    [SerializeField] protected float releaseAngularDrag;


    [SerializeField] protected Health health;

    public bool killRagdollOnDeath;

    [SerializeField] protected float loosenJointHealthThreshold;
    [SerializeField] protected float freeJointHealthThreshold;

    [SerializeField] protected float looseJointMultiplier;
    protected float angularXDrive;
    protected float angularYZDrive;

    [SerializeField] protected float dismemberPullDistance;
    [SerializeField] protected float dismemberPullTime;

    [SerializeField] protected float cutVelocity = 1f;
    [SerializeField] protected float cutDistance = 0.25f;
    [SerializeField] protected float cutDot = 0.25f;
    [SerializeField] protected Vector3 cutVector = new Vector3(0,1,0);

    public float CutVelocity { get { return cutVelocity; } }
    public float CutDistance { get { return cutDistance; } }
    public float CutDot { get { return cutDot; } }
    public Vector3 CutVector { get { return cutVector; } }

    public float twistDuration = 0.1f;

    void FixedUpdate()
    {
        Hold();
    }

    void Grab()
    {
        if(!pinTransform && iv.Hand) pinTransform = iv.Hand.transform;

        InitialAttachPoint.position = PinTransform.position;
        InitialAttachPoint.rotation = PinTransform.rotation;

        InitialAttachPoint.SetParent(transform, true);

        //rb.drag = grabDrag;
        //rb.angularDrag = grabAngularDrag;
        //rb.useGravity = false;

       // if(iv.Hand) transform.parent = iv.Hand.transform.root;
    }

    void Hold()
    {
        if (PinTransform)
        {
            Vector3 positionDelta = (PinTransform.position - InitialAttachPoint.position) * grabForce;

            rb.velocity = positionDelta * Time.deltaTime;

            rb.angularVelocity = VectorHistoryAverage.GetAngularVelocityAngleAxis(InitialAttachPoint.rotation, PinTransform.rotation) * grabAngularForce;

            Dismember();

            Twist();
        }
    }

    void Release()
    {
        //rb.drag = releaseDrag;
        //rb.angularDrag = releaseAngularDrag;
        //rb.useGravity = true;

        transform.parent = null;
        pinTransform = null;

        if (PinTransform)
        {
            InitialAttachPoint.position = transform.position;
            InitialAttachPoint.rotation = transform.rotation;
        }

    }

    protected bool free;

    public virtual void LoosenJoint(Health health)
    {
        if (!dummyJoint) return;

        if (!health) return;

        if (free) return;

        dummyJoint.angularXDrive = SetJointDrive(dummyJoint.angularXDrive, angularXDrive);
        dummyJoint.angularYZDrive = SetJointDrive(dummyJoint.angularYZDrive, angularYZDrive);

        if (health.CurrentHealth <= health.MaxHealth * freeJointHealthThreshold)
        {
            dummyJoint.angularXDrive = DampJointDrive(dummyJoint.angularXDrive, 0);
            dummyJoint.angularYZDrive = DampJointDrive(dummyJoint.angularYZDrive, 0);
        }
        else if(health.CurrentHealth <= health.MaxHealth * loosenJointHealthThreshold)
        {
            dummyJoint.angularXDrive = DampJointDrive(dummyJoint.angularXDrive, looseJointMultiplier);
            dummyJoint.angularYZDrive = DampJointDrive(dummyJoint.angularYZDrive, looseJointMultiplier);
        }
    }

    public virtual void FreeJoint(float damp)
    {
        if (!dummyJoint) return;

        dummyJoint.angularXDrive = DampJointDrive(dummyJoint.angularXDrive, 0);
        dummyJoint.angularYZDrive = DampJointDrive(dummyJoint.angularYZDrive, 0);

        free = true;
    }

    public void UnfreeJoint()
    {
        free = false;
        LoosenJoint(health);
    }

    private float dismemberTime;

    void Dismember()
    {
        if (dismemberPullDistance <= 0) return;

        if (iv.Hand)
            if (Vector3.Distance(PinTransform.position, InitialAttachPoint.position) > dismemberPullDistance)
            {
                if (dismemberTime == 0)
                    dismemberTime = Time.time;

                if (Time.time - dismemberTime >= dismemberPullTime)
                    DisconnectJoint();
            }
            else
            {
                dismemberTime = 0;
            }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.attachedRigidbody != anchorRb)
        {
            if (_OnCollision != null)
                _OnCollision(collision.relativeVelocity.magnitude);
        }
    }

    int twistedFrames;

    void Twist()
    {
        if (!dummyJoint) return;

        if (!dummyJoint.connectedBody) return;

        if (twistDuration == 0) return;

        if (!dummyJoint) return;

        Vector3 twist = initialRotation - (dummyJoint.connectedBody.transform.eulerAngles - transform.eulerAngles);

        if (twist.x > 180f)
           twist.x -= 360f;
        if (twist.x < -180f)
            twist.x += 360f;
        if (twist.y > 180f)
            twist.y -= 360f;
        if (twist.y < -180f)
            twist.y += 360f;
        if (twist.z > 180f)
            twist.z -= 360f;
        if (twist.z < -180f)
            twist.z += 360f;

        if (Overtwist(dummyJoint.lowAngularXLimit.limit, dummyJoint.highAngularXLimit.limit, twist.x, 5))
            Debug.Log("Twist x");

        if(Overtwist(-dummyJoint.angularYLimit.limit, dummyJoint.angularYLimit.limit, twist.y, 5))
            Debug.Log("Twist y");

        if(Overtwist(-dummyJoint.angularZLimit.limit, dummyJoint.angularZLimit.limit, twist.z, 5))
            Debug.Log("Twist Z");

        if (Overtwist(dummyJoint.lowAngularXLimit.limit, dummyJoint.highAngularXLimit.limit, twist.x, 5)
        || Overtwist(-dummyJoint.angularYLimit.limit, dummyJoint.angularYLimit.limit, twist.y, 5)
        || Overtwist(-dummyJoint.angularZLimit.limit, dummyJoint.angularZLimit.limit, twist.z, 5))
        {
            twistedFrames++;

            if(twistedFrames > twistDuration * 100)
                if (health)
                    health.Kill();
        }
        else
        {
            twistedFrames = 0;
        }
    }

    bool Overtwist(float min, float max, float current, float hysteresis)
    {
        if (min >= 0 || max == 0) return false;

        if (current <= min
        || current >= max)
        {
            return true;
        }

        return false;
    }

    protected JointDrive DampJointDrive(JointDrive jointDrive, float damp)
    {
        return SetJointDrive(jointDrive, jointDrive.positionSpring * damp, jointDrive.positionDamper, jointDrive.maximumForce);
    }

    protected JointDrive SetJointDrive(JointDrive jointDrive, float positionSpring)
    {
        return SetJointDrive(jointDrive, positionSpring, jointDrive.positionDamper, jointDrive.maximumForce);
    }

    protected virtual JointDrive SetJointDrive(JointDrive jointDrive, float spring, float damper, float maximumForce)
    {
        JointDrive newDrive = new JointDrive();
        newDrive.positionSpring = spring;
        newDrive.positionDamper = damper;
        newDrive.maximumForce = maximumForce;
        return newDrive;
    }

    public virtual void DisconnectJoint()
    {
        if (dummyJoint)
        {
            health.Kill();

            dummyJoint.connectedBody = null;

            dummyJoint.xMotion = ConfigurableJointMotion.Free;
            dummyJoint.yMotion = ConfigurableJointMotion.Free;
            dummyJoint.zMotion = ConfigurableJointMotion.Free;

            dummyJoint.angularXMotion = ConfigurableJointMotion.Free;
            dummyJoint.angularYMotion = ConfigurableJointMotion.Free;
            dummyJoint.angularZMotion = ConfigurableJointMotion.Free;
        }
    }
}