using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace VArmory
{
    public class Magazine : Item
    {
        [SerializeField] protected List<AudioClip> loadBulletSound = new List<AudioClip>();
        [SerializeField] protected List<AudioClip> ejectBulletSound = new List<AudioClip>();

        [SerializeField] protected Transform eject;
        [SerializeField] protected List<Renderer> bullets = new List<Renderer>();

        [SerializeField] protected string bulletTag;
        [SerializeField] protected Bullet bulletClone;

        [SerializeField] protected float swipeToEjectDistance = 0.25f;

        protected List<Bullet> savedBullets = new List<Bullet>();
        public List<Bullet> SavedBullets { get { return savedBullets; } }

        [SerializeField] protected int maxRounds;
        [SerializeField] protected int currentRounds;

        [SerializeField] protected bool requireHeldBullets = true;

        [SerializeField] protected SteamVR_Action_Boolean ejectBulletInput;

        public int MaxRounds { get { return maxRounds; } }

        public int CurrentRounds
        {
            get { return currentRounds; }
            set
            {
                currentRounds = Mathf.Clamp(value, 0, maxRounds);
                UpdateBulletRenderers();
            }
        }

        public bool Full { get { return CurrentRounds >= maxRounds; } }
        public bool Empty { get { return CurrentRounds <= 0; } }

        protected override void Start()
        {
            base.Start();

            if (eject == null)
                if ((eject = transform.Find("Eject") ?? transform.Find("eject")) == null)
                    eject = transform;

            if (!bulletClone)
            {
                Debug.Log(name + " does not have a bullet clone");
                return;
            }

            while (savedBullets.Count < currentRounds)
            {
                Bullet bullet = Instantiate(bulletClone, transform.position, transform.rotation);

                bullet.GetComponentInChildren<MeshRenderer>().enabled = false;
                bullet.Restrained = true;
                bullet.SetKinematic();
                bullet.Rb.useGravity = false;
                bullet.Col.enabled = false;

                savedBullets.Add(bullet);
            }

            UpdateBulletRenderers();
        }

        Vector2 initialPress;

        protected override void Update()
        {
            base.Update();

            if (!PrimaryHand)
                return;

            Vector2 tempSwipe = PrimaryHand.TouchpadAxis;

            LocalInputDown(EjectBullet, ejectBulletInput);

            if(LocalInputDown(null, PrimaryHand.TouchpadInput))
                initialPress = tempSwipe;

            if (tempSwipe != Vector2.zero
                && tempSwipe.y - initialPress.y >= swipeToEjectDistance
                && initialPress != Vector2.zero)
            {
                initialPress = new Vector2(100, 100);
                EjectBullet();
            }
        }

        void EnableBullets()
        {
            for (int i = 0; i < savedBullets.Count; i++)
                savedBullets[i].enabled = true;
        }

        void EnableBullets(bool enable)
        {
            for (int i = 0; i < savedBullets.Count; i++)
                savedBullets[i].enabled = enable;
        }

        void EnableBulletRenderers()
        {
            for (int i = 0; i < bullets.Count; i++)
                bullets[i].enabled = true;
        }

        void EnableBulletRenderers(bool enable)
        {
            for (int i = 0; i < bullets.Count; i++)
                bullets[i].enabled = enable;
        }

        public void UpdateBulletRenderers()
        {
            for (int i = 0; i < bullets.Count; i++)
                bullets[i].enabled = i < currentRounds;
        }

        public void LoadBullet(Item round)
        {
            if (Restrained)
                return;

            if (round.GetType() != typeof(Bullet))
                return;

            Bullet tempRound = round as Bullet;

            LoadBullet(tempRound);
        }

        public void LoadBullet(Bullet bullet)
        {
            if (CurrentRounds >= MaxRounds)
                return;

            Hand tempHand = bullet.PrimaryHand;

            bullet.dropStack = false;

            savedBullets.Add(bullet);

            bullet.transform.SetParent(transform, true);
            bullet.transform.localPosition = Vector3.zero;
            bullet.transform.rotation = Quaternion.identity;

            bullet.DetachWithOutStoring();
            bullet.Restrained = true;
            bullet.GetComponentInChildren<MeshRenderer>().enabled = false;
            bullet.SetPhysics(true, false, true, true);

            CurrentRounds += 1;

            TwitchExtension.PlayRandomAudioClip(loadBulletSound, eject.position);

            if (tempHand)
            {
                tempHand.GrabFromStack();
                bullet.dropStack = true;
            }
        }

        void EjectBullet()
        {
            if (Empty)
                return;

            Bullet bullet = savedBullets[savedBullets.Count - 1]; //Instantiate(BulletPrefab, eject.position, eject.rotation);
            savedBullets.Remove(bullet);

            bullet.transform.position = eject.position;
            bullet.transform.rotation = eject.rotation;

            bullet.DetachWithOutStoring();
            bullet.SetPhysics(OnDetach);
            bullet.Rb.velocity = velocityHistory._ReleaseVelocity;
            bullet.transform.SetParent(null, true);
            bullet.Restrained = false;
            bullet.GetComponentInChildren<MeshRenderer>().enabled = true;

            CurrentRounds -= 1;

            TwitchExtension.PlayRandomAudioClip(ejectBulletSound, eject.position);

            if (PrimaryHand.Sibling.StackableStoredItem)
                PrimaryHand.Sibling.AddToStack(bullet);
        }

        public void RemoveBullet()
        {
            Bullet tempBullet = SavedBullets[SavedBullets.Count - 1];
            SavedBullets.Remove(tempBullet);
            CurrentRounds -= 1;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!PrimaryHand && !SecondaryHand)
                return;

            if (Restrained)
                return;

            if (other.gameObject.tag != "Interactable")
                return;

            Bullet tempBullet = other.GetComponent<Bullet>();

            if (!tempBullet)
                return;

            if (!tempBullet.HasTag(bulletTag))
                return;

            if (tempBullet.Spent)
                return;

            if (requireHeldBullets)
                if (!(tempBullet.PrimaryHand ^ tempBullet.SecondaryHand))
                    return;

            LoadBullet(tempBullet);
        }


        [SerializeField] protected Magazine tapedMagazine;

        public Magazine TapedMagazine { get { return tapedMagazine; } }

        protected override void PrimaryGrasp()
        {
            base.PrimaryGrasp();

            if (tapedMagazine)
            {
                tapedMagazine.Detach();
                tapedMagazine.DetachSlot();
                tapedMagazine.transform.SetParent(transform, true);
                tapedMagazine.SetPhysics(true, false, true, false);
                tapedMagazine.slot = PrimaryHand;

                EnableBullets();
            }
        }

        protected override void PrimaryDrop()
        {
            bool tempyes = PrimaryHand;

            base.PrimaryDrop();

            if (tapedMagazine)
            {
                if (tempyes)
                    tapedMagazine.SetPhysics(true, false, true, true);

                tapedMagazine.slot = null;
            }

            if (transform.parent == null)
                EnableBullets(false);
        }
    }
}