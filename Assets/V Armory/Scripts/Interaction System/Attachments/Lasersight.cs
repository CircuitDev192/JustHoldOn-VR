using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Lasersight : Attachment
    {
        protected LineRenderer laser;
        [SerializeField] protected Transform dot;

        protected override void Start()
        {
            base.Start();
            laser = GetComponentInChildren<LineRenderer>();
        }

        public override void Use()
        {
            base.Use();

            bool tempEnable = !laser.enabled;

            laser.enabled = tempEnable;
            dot.gameObject.SetActive(tempEnable);
        }

        protected override void PrimaryGrasp()
        {
            base.PrimaryGrasp();
        }

        protected override void Update()
        {
            base.Update();

            Laser();
        }

        void Laser()
        {
            if (!laser.enabled)
                return;

            RaycastHit hit;

            if (Physics.Raycast(laser.transform.position, laser.transform.forward, out hit, 50))
            {
                if (!dot.gameObject.activeSelf)
                    dot.gameObject.SetActive(true);

                dot.transform.position = hit.point + (hit.normal * 0.01f);
                dot.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                laser.SetPosition(1, new Vector3(0, 0, Vector3.Distance(laser.transform.position, hit.point)));
            }
            else if (dot.gameObject.activeSelf)
            {
                dot.gameObject.SetActive(false);
                laser.SetPosition(1, new Vector3(0, 0, 50));
            }
        }
    }
}