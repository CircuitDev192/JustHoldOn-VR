/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class PuppetMasterJoint : TestDummyJoint
{
    public PuppetMaster puppet;
    public Muscle muscle;

    protected override void Awake()
    {
        base.Awake();

        if (puppet)
            foreach (Muscle muscle in puppet.muscles)
                if (muscle.joint == dummyJoint)
                    this.muscle = muscle;

        if (dummyJoint)
        {
            defaultPinWeight = muscle.props.pinWeight;
            defaultMuscleWeight = muscle.props.muscleWeight;
        }

        if (health)
        {
            health._OnKilled += KillPuppet;
        }
    }

    void KillPuppet(Health health)
    {
        if (killRagdollOnDeath && puppet)
            puppet.Kill();
    }

    public override void FreeJoint(float damp)
    {
        if (!dummyJoint) return;

        muscle.props.pinWeight = 0;
        muscle.props.muscleWeight = defaultMuscleWeight * 0.01f;

        free = true;
    }

    public override void LoosenJoint(Health health)
    {
        if (!dummyJoint) return;

        if (!health) return;

        if (free) return;

        muscle.props.pinWeight = defaultPinWeight;
        muscle.props.muscleWeight = defaultMuscleWeight;

        if (health.CurrentHealth <= health.MaxHealth * freeJointHealthThreshold)
        {
            muscle.props.pinWeight = 0;
            muscle.props.muscleWeight *= 0.01f;
        }
        else if (health.CurrentHealth <= health.MaxHealth * loosenJointHealthThreshold)
        {
            muscle.props.pinWeight = defaultPinWeight * looseJointMultiplier;
            muscle.props.muscleWeight *= looseJointMultiplier;
        }
    }

    public override void DisconnectJoint()
    {
        if (dummyJoint)
        {
            health.Kill();
            DisconnectMuscleRecursiveByJoint(dummyJoint);
        }
    }

    public void DisconnectMuscleRecursiveByJoint(ConfigurableJoint joint, MuscleDisconnectMode disconnectMode = MuscleDisconnectMode.Sever, bool deactivate = false)
    {
        for (int i = puppet.muscles.Length - 1; i > -1; i--)
        {
            if (joint == puppet.muscles[i].joint)
            {
                puppet.DisconnectMuscleRecursive(i, disconnectMode, deactivate);
                return;
            }
        }
    }
}
*/