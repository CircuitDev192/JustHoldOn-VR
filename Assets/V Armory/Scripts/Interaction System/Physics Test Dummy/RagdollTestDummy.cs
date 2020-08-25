using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollTestDummy : MonoBehaviour
{
    TestDummyJoint head;

    TestDummyJoint testDummy;

    [SerializeField] protected bool selfCollision;

    [SerializeField] protected float ragdollVelocity;
    [SerializeField] protected float maxRagdollVelocity;
    [SerializeField] protected float ragdollDot;
    [SerializeField] protected float maxRagdollDot;
    [SerializeField] protected float unragdollVelocity;
    protected float unragdollTime;

    [SerializeField] protected float ragdollDamp;

    public TestDummyJoint[] tempTestDummyJoints;

    public bool _Ragdoll;

    public AnimationCurve unconciousDurationOverCollisionMagnitude;

    private void Awake()
    {
        head = transform.parent.Find("Head").GetComponent<TestDummyJoint>();
        tempTestDummyJoints = transform.parent.GetComponentsInChildren<TestDummyJoint>();

        foreach (TestDummyJoint testDummyJoint in tempTestDummyJoints)
        {
            if (testDummyJoint.killRagdollOnDeath) testDummyJoint._OnKilled += PermanentRagdoll;
        }

        if(head)
        head._OnCollision += Knockout;

        if(!selfCollision)
            Invoke("IgnoreCol", 0.5f);
    }

    void Knockout(float magnitude)
    {
        float duration = unconciousDurationOverCollisionMagnitude.Evaluate(magnitude);

        if (Time.time - unragdolledTimeStamp > 5)
        {
            if (duration > 1f)
            {
                SetRagdoll(duration);
            }
        }
        else if(duration > unragdollTime)
        {
            unragdollTime = duration;
        }
    }

    void IgnoreCol()
    {
        foreach (TestDummyJoint testDummyJointA in tempTestDummyJoints)
        {
            foreach (TestDummyJoint testDummyJointB in tempTestDummyJoints)
            {
                Physics.IgnoreCollision(testDummyJointA.DummyCollider, testDummyJointB.DummyCollider, true);
            }
        }
    }

    void FreeJointsOnRagdoll()
    {
        foreach(TestDummyJoint testDummyJoint in tempTestDummyJoints)
        {
            if (testDummyJoint == null) continue;

            testDummyJoint.FreeJoint(0);
        }
    }

    void RagdollJointUnDamp()
    {
        foreach (TestDummyJoint testDummyJoint in tempTestDummyJoints)
        {
            if (testDummyJoint == null) continue;

            testDummyJoint.UnfreeJoint();
        }
    }

    private void Start()
    {
        testDummy = GetComponent<TestDummyJoint>();
        savedPin = testDummy.defaultPinTransform;
    }

    private void Update()
    {
        if (_Ragdoll)
        {
            SetRagdoll();
            _Ragdoll = false;
        }

        if (ragdolled)
        {
            if (getBackUp)
                Unragdoll();
        }
        else
            Ragdoll();
    }

    void Ragdoll()
    {
        if (Time.time - unragdolledTimeStamp > 5)
        {
            if (testDummy.DummyRigidbody.velocity.magnitude >= ragdollVelocity)
                if (Vector3.Dot(transform.up, Vector3.up) <= ragdollDot)
                {
                    SetRagdoll();
                }

            if (Vector3.Dot(transform.up, Vector3.up) <= maxRagdollDot)
            {
                SetRagdoll();
            }

            if (testDummy.DummyRigidbody.velocity.magnitude >= maxRagdollVelocity)
            {
                SetRagdoll();
            }
        }
    }

    bool ragdolled;
    bool getBackUp = true;

    public Transform savedPin;

    void SetRagdoll()
    {
        SetRagdoll(5);
    }

    void SetRagdoll(float duration)
    {
        unragdollTime = duration;

        testDummy.defaultPinTransform = null;
        
        if(!ragdolled)
            FreeJointsOnRagdoll();

        ragdolled = true;
    }

    void PermanentRagdoll()
    {
        SetRagdoll();
        getBackUp = false;
    }

    float unragdollTimeStamp;
    float unragdolledTimeStamp;

    void Unragdoll()
    {
        if(testDummy.DummyRigidbody.velocity.magnitude <= unragdollVelocity)
        {
            if(unragdollTimeStamp == 0)
                unragdollTimeStamp = Time.time;

            if(unragdollTimeStamp != 0)
                if(Time.time - unragdollTimeStamp >= unragdollTime)
                {
                    if(setPinPosition != null) StopCoroutine(setPinPosition);

                    if (savedPin != null)
                    {
                        testDummy.defaultPinTransform = savedPin;
                        testDummy.InitialAttachPoint.position = testDummy.transform.position;
                        testDummy.InitialAttachPoint.rotation = testDummy.transform.rotation;
                    }

                    StartCoroutine(setPinPosition = SetPinPosition());

                    unragdolledTimeStamp = Time.time;

                    RagdollJointUnDamp();
                    unragdollTimeStamp = 0;
                    ragdolled = false;
                }
        }
        else
        {
            unragdollTimeStamp = 0;
        }
    }

    IEnumerator setPinPosition;

    IEnumerator SetPinPosition()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        Vector3 initialPosition = testDummy.transform.position;

        if (testDummy.defaultPinTransform)
            testDummy.defaultPinTransform.position = initialPosition;

        Vector3 newPosition = initialPosition + new Vector3(0, 0.5f, 0);

        float time = 0;

        while (Vector3.Distance(testDummy.transform.position, newPosition) > 0.1f)
        {
            if(testDummy.defaultPinTransform)
                testDummy.defaultPinTransform.position = Vector3.Lerp(initialPosition, newPosition, time);

            time += Time.fixedDeltaTime;

            yield return wait;
        }
    }
}
