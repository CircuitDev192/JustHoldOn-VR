using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class TaclightLasersightCombo : Attachment
    {

        [SerializeField] protected Light flashlight;
        [SerializeField] protected Transform dot;

        [SerializeField] protected MeshRenderer lightMaterial;
        protected LineRenderer laser;

        protected override void Start()
        {
            base.Start();
            laser = GetComponentInChildren<LineRenderer>();
            if (!lightMaterial) lightMaterial = GetComponentInChildren<MeshRenderer>();
        }

        public override void Use()
        {
            base.Use();

            if (dot.gameObject.activeSelf)
                Taclight();

            Lasersight();
        }

        public override void Pose()
        {
            base.Pose();

            if (!laser)
                return;

            if (!laser.enabled)
                return;

            RaycastHit hit;

            if (Physics.Raycast(laser.transform.position, laser.transform.forward, out hit, 50))
            {
                if (!dot.gameObject.activeSelf)
                    dot.gameObject.SetActive(true);

                dot.transform.position = hit.point + (hit.normal * 0.01f);
                dot.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                laser.SetPosition(1, new Vector3(0, 0, hit.distance));
            }
            else if (dot.gameObject.activeSelf)
            {
                dot.gameObject.SetActive(false);
                laser.SetPosition(1, new Vector3(0, 0, 50));
            }
        }

        void Taclight()
        {
            flashlight.enabled = !flashlight.enabled;
            lightMaterial.enabled = !lightMaterial.enabled;
        }

        void Lasersight()
        {
            dot.gameObject.SetActive(!dot.gameObject.activeSelf);
            laser.enabled = !laser.enabled;
        }
    }
}