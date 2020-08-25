using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VArmory;


public class BodyManager : MonoBehaviour
{
    public bool randomizeMaterial;
    public List<Material> materials = new List<Material>();

    public PhysicsGrab headGrabPoint;
    public PhysicsGrab torsoGrabPoint;
    public PhysicsGrab legsGrabPoint;

    public delegate void Ragdoll();
    public event Ragdoll _Ragdoll;

    public delegate void Unragdoll();
    public event Unragdoll _Unragdoll;

    Rigidbody rb;
    [SerializeField] public BodyPortion head;
    [SerializeField] public BodyPortion torso;
    [SerializeField] public BodyPortion legs;

    [SerializeField] protected float rotationSpeed = 3f;

    public bool canRagdoll;
    public float unragdollVelocityThreshold = 1.5f;
    public float minRagdollVelocityThreshold = 1f;
    public float ragdollVelocityThreshold = 10f;
    public float unragdollVelocityTime = 1f;
    public float ragdollDuration;
    protected float ragdollTime;

    public float armorDamp;
    public float disabledSpeedScale;

    [Range(0, 100)] [SerializeField] protected int decapitateChance;
    [Range(0, 100)] [SerializeField] protected int crippleChance;

    [SerializeField] protected Transform agent;

    public float Speed
    {
        get
        {
            return speedOverHealthForLegs.Evaluate(legs.health.CurrentHealth / legs.health.MaxHealth)
                  * (legs.joint == null ? disabledSpeedScale : 1);
        }
    }

    public AnimationCurve speedOverHealthForLegs;

    [System.Serializable]
    public struct BodyPortion
    {
        public Vector3 initialRotation;
        public Vector3 initialPosition;

        public Health health;
        public Health armor;
        public Rigidbody rb;

        public ConfigurableJoint joint;

        public float undamped;
        public float damped;
        public float loosenMultiplier;

        public bool Attached { get { return health.CurrentHealth > 0; } }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        Transform tempHead = transform.Find("Head");
        Transform tempTorso = transform.Find("Torso");
        Transform tempLegs = transform.Find("Legs");

        if (tempHead)
            SetBodyPortion(ref head, tempHead);

        if (tempTorso)
            SetBodyPortion(ref torso, tempTorso);

        if (tempLegs)
            SetBodyPortion(ref legs, tempLegs);

        head.health._OnEffect += Trauma;
        torso.health._OnEffect += Trauma;
        legs.health._OnEffect += Trauma;

        head.health._OnDamaged += OnDamage;
        torso.health._OnDamaged += OnDamage;
        legs.health._OnDamaged += OnLegsDamaged;

        head.health._OnKilled += Undamp;
        torso.health._OnKilled += Undamp;
        legs.health._OnKilled += Undamp;

        if (randomizeMaterial)
            if (materials.Count > 0)
            {
                Material tempMaterial = materials[Random.Range(0, materials.Count)];
                MeshRenderer[] tempMeshRenderers = GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer meshRenderer in tempMeshRenderers)
                    if (!meshRenderer.transform.name.Contains("Eye"))
                        meshRenderer.sharedMaterial = tempMaterial;
            }
    }

    void SetBodyPortion(ref BodyPortion portion, Transform part)
    {
        portion.initialRotation = part.localRotation.eulerAngles;
        portion.initialPosition = part.localPosition;

        portion.health = part.GetComponent<Health>();

        if (part.GetChild(0).childCount > 0)
            portion.armor = part.GetChild(0).GetComponent<Health>();

        portion.rb = part.GetComponent<Rigidbody>();
        portion.joint = part.GetComponent<ConfigurableJoint>();

        if (portion.joint)
        {
            portion.undamped = portion.joint.angularXDrive.positionSpring;
            portion.damped = portion.undamped * armorDamp;

            JointDrive jointDrive = portion.joint.angularXDrive;

            jointDrive.positionSpring = portion.armor != null ? portion.damped : portion.undamped;

            portion.joint.angularXDrive = jointDrive;
            portion.joint.angularYZDrive = jointDrive;
        }
    }

    Vector3 offset;
    float velocityRagdollTime;

    public float CurrentVelocity;

    void FixedUpdate()
    {
        trauma = Mathf.Clamp(trauma - (recovery * Time.fixedDeltaTime), 0, maxTrauma);

        if (rb.isKinematic)
        {
            Vector3 newPosition = agent.transform.position + offset;

            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 3);
            transform.rotation = Quaternion.Slerp(transform.rotation, agent.transform.rotation, Time.deltaTime * rotationSpeed);

            CurrentVelocity = torso.rb.velocity.magnitude;

            if (CurrentVelocity >= ragdollVelocityThreshold - trauma && canRagdoll)
                if (Vector3.Dot(torsoGrabPoint.transform.up, Vector3.down) > -0.7f)
                    if (_Ragdoll != null)
                        _Ragdoll();

            velocityRagdollTime = 0;
        }
        else
        {
            if (rb.velocity.magnitude <= unragdollVelocityThreshold && Time.time - ragdollTime >= ragdollDuration)
            {
                bool held = headGrabPoint.IV.Hand ?? torsoGrabPoint.IV.Hand ?? legsGrabPoint.IV.Hand;

                if (!held)
                {
                    velocityRagdollTime = velocityRagdollTime == 0 ? Time.time : velocityRagdollTime;

                    if (_Unragdoll != null && Time.time - velocityRagdollTime >= unragdollVelocityTime)
                    {
                        _Unragdoll();
                    }
                }
            }

            if (DismemberBody(headGrabPoint, head.health))
                StartCoroutine(Decapitate());

            if (DismemberBody(legsGrabPoint, legs.health))
                StartCoroutine(Cripple());
        }

        if (Vector3.Dot(headGrabPoint.transform.up, torsoGrabPoint.transform.right) > 0.75f && headGrabPoint.IV.Hand)
        {
            head.health.ApplyDamage(Mathf.Infinity);
        }
    }

    public float trauma;
    public float maxTrauma;

    public float recovery;

    void Trauma(float trauma, float impactForce)
    {
        this.trauma = Mathf.Clamp(this.trauma + trauma, 0, maxTrauma);

        if (rb.isKinematic)
            this.trauma = Mathf.Clamp(this.trauma + impactForce, 0, maxTrauma);
    }

    bool ragdolled;

    public bool Ragdolled { get { return ragdolled; } }

    protected float headDrag = 0;
    protected float torsoDrag = 0;
    protected float legDrag = 0;

    public void ManualRagdoll()
    {
        if (!ragdolled)
        {
            if (torsoGrabPoint.IV.Hand)
                torsoGrabPoint.ForcePullToFixedJoint();
            else if (headGrabPoint.IV.Hand)
                headGrabPoint.ForcePullToFixedJoint();
            else if (legsGrabPoint)
                legsGrabPoint.ForcePullToFixedJoint();
        }

        //if (agent.enabled)
        //    agent.isStopped = true;

        ragdolled = true;

        //if (agent.isOnOffMeshLink)
        //    agent.Warp(transform.position);

        rb.isKinematic = false;
        ragdollTime = Time.time;

        head.rb.drag = 0;
        torso.rb.drag = 0;
        legs.rb.drag = 0;
    }

    public void ManualUnragdoll()
    {
        ragdolled = false;

        rb.isKinematic = true;

        head.rb.drag = headDrag;
        torso.rb.drag = torsoDrag;
        legs.rb.drag = legDrag;
    }

    int frames;

    bool DismemberBody(PhysicsGrab physicsGrab, Health health)
    {
        //kinematic is false

        if (physicsGrab.IV.Hand)
        {
            if(physicsGrab.FixedJoint)
                if (physicsGrab.FixedJoint.connectedBody == physicsGrab.IV.Hand.OffsetRigidbody)
                    frames = 0;

            if (physicsGrab.AttachPosition != Vector3.zero)
                if (Vector3.Distance(physicsGrab.AttachPosition, physicsGrab.IV.Hand.transform.position) > 0.35f)
                {
                    frames += 1;

                    if (torsoGrabPoint.IV.Hand && frames > 10)
                    {
                        frames = 0;
                        health.ApplyDamage(Mathf.Infinity);

                        physicsGrab.ForcePullToFixedJoint();
                        torsoGrabPoint.ForcePullToFixedJoint();

                        return true;
                    }
                }
                else
                    frames = 0;
        }

        return false;
    }

    public IEnumerator Decapitate()
    {
        //if (torsoGrabPoint.IV.Hand)
        //   torsoGrabPoint.AttachPointToConnectedBody(false);

        //if (legsGrabPoint.IV.Hand)
        //  legsGrabPoint.AttachPointToConnectedBody(true);

        if (!head.joint)
            yield break;

        head.health.DeathSFX(headGrabPoint.transform.position, -headGrabPoint.transform.up * 1.25f);
        torso.health.DeathSFX(torsoGrabPoint.transform.position + (torsoGrabPoint.transform.up * 1), torsoGrabPoint.transform.up);

        if (dismemberAudio.Length > 0)
            AudioSource.PlayClipAtPoint(dismemberAudio[Random.Range(0, dismemberAudio.Length)], headGrabPoint.transform.position);

        if (dismemberPainAudio.Length > 0)
            AudioSource.PlayClipAtPoint(dismemberPainAudio[Random.Range(0, dismemberPainAudio.Length)], headGrabPoint.transform.position);

        headGrabPoint.RemoveConnectedBodies();
        torsoGrabPoint.RemoveConnectedBody(headGrabPoint);
        legsGrabPoint.RemoveConnectedBody(headGrabPoint);

        head.rb.transform.parent = null;
        Destroy(head.joint);

        head.rb.velocity *= 0.25f;
        head.rb.drag = 0;
        head.rb.angularDrag = 0.05f;

        if (legs.joint == null)
        {
            torso.rb.transform.parent = null;
            Destroy(torso.joint);
        }

        if (headGrabPoint.IV.Hand)
            headGrabPoint.ForcePullToFixedJoint();

        yield return null;
    }

    [SerializeField] protected AudioClip[] dismemberAudio;
    [SerializeField] protected AudioClip[] dismemberPainAudio;

    public IEnumerator Cripple()
    {
        if (!legs.joint)
            yield break;

        if (dismemberAudio.Length > 0)
            AudioSource.PlayClipAtPoint(dismemberAudio[Random.Range(0, dismemberAudio.Length)], headGrabPoint.transform.position);
        if (dismemberPainAudio.Length > 0)
            AudioSource.PlayClipAtPoint(dismemberPainAudio[Random.Range(0, dismemberPainAudio.Length)], headGrabPoint.transform.position);

        headGrabPoint.RemoveConnectedBody(legsGrabPoint);
        legsGrabPoint.RemoveConnectedBodies();
        torsoGrabPoint.RemoveConnectedBody(legsGrabPoint);

        legs.health.DeathSFX(legsGrabPoint.transform.position + (transform.up * 1), legsGrabPoint.transform.up);

        ManualRagdoll();

        Rigidbody temp = rb;
        legs.joint.transform.parent = null;

        Destroy(legs.joint);

        transform.parent = torso.rb.transform;

        offset = new Vector3(0, -0.5f, 0);
        transform.localPosition = offset;
        transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));
        transform.parent = null;

        torso.joint.connectedBody = temp;

        legs.rb.velocity *= 0.2f;
        legs.rb.drag = 0;
        legs.rb.angularDrag = 0.05f;

        if (head.joint == null)
        {
            torso.rb.transform.parent = null;
            Destroy(torso.joint);
        }

        //if (headGrabPoint.IV.Hand)
        //   headGrabPoint.ForcePullToFixedJoint(true);

        yield return null;
    }

    void OnDamage(Health health)
    {
        if (health.CurrentHealth <= 0)
        {
            if (health == head.health)
                if (Random.Range(0, 100) < decapitateChance)
                    StartCoroutine(Decapitate());

            //agent.enabled = false;
            ragdollDuration = Mathf.Infinity;
            ManualRagdoll();

            Undamp(legs.health);
            Undamp(torso.health);
            Undamp(head.health);
        }
    }

    void OnLegsDamaged(Health health)
    {
        //agent.speed = Speed;

        if (health.CurrentHealth <= 0)
        {
            legs.health._OnDamaged -= OnLegsDamaged;

            if (Random.Range(0, 100) < crippleChance)
                StartCoroutine(Cripple());
        }
    }

    void Undamp(Health health)
    {
        BodyManager.BodyPortion portion = new BodyManager.BodyPortion();

        if (health == head.health)
            portion = head;
        else if (health == torso.health)
            portion = torso;
        else if (health == legs.health)
            portion = legs;

        if (portion.joint == null)
            return;

        JointDrive jointDrive = portion.joint.angularXDrive;

        jointDrive.positionSpring = portion.undamped * portion.loosenMultiplier;

        portion.joint.angularXDrive = jointDrive;
        portion.joint.angularYZDrive = jointDrive;

        if (health == head.armor)
            head = portion;
        else if (health == torso.armor)
            torso = portion;
        else if (health == legs.armor)
            legs = portion;
    }
}
