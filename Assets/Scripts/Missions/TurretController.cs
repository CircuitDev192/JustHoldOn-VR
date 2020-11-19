using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private Camera turretCamera;
    [SerializeField] private Light turretLight;
    private Camera playerCamera;
    private bool playerInVehicle = false;

    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;
    public bool lockCursor = true;


    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private bool m_cursorIsLocked = true;

    // Start is called before the first frame update
    void Start()
    {
        turretCamera.gameObject.GetComponent<AudioListener>().enabled = false;

        turretLight.enabled = false;

        playerCamera = PlayerManager.instance.player.GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("TurretController :: Player camera is null.");
        }

        EventManager.PlayerEnteredMissionVehicle += PlayerEnteredMissionVehicle;

        Init(this.gameObject.transform, turretCamera.transform);
    }

    private void PlayerEnteredMissionVehicle()
    {
        playerCamera.enabled = false;
        playerCamera.gameObject.GetComponent<AudioListener>().enabled = false;
        turretCamera.enabled = true;
        turretCamera.gameObject.GetComponent<AudioListener>().enabled = true;

        EventManager.TriggerPlayerCameraChanged(turretCamera);

        playerInVehicle = true;

        turretLight.enabled = true;

        audioSource.PlayOneShot(reloadSound, 0.8f * PlayerManager.instance.soundMultiplier);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInVehicle)
        {
            LookRotation(this.gameObject.transform, turretCamera.transform);

            nextShotTime = (nextShotTime > Time.time) ? nextShotTime : Time.time;

            if (Input.GetMouseButton(0) && lockCursor)
            {
                // We can fire another shot
                if (Time.time >= nextShotTime)
                {
                    // Start fire coroutine
                    StartCoroutine(Fire(turretCamera.transform));

                    EventManager.TriggerSoundGenerated(this.transform.position, audibleDistance);

                    nextShotTime = Time.time + fireRate;
                }
            }
        }
    }

    #region Shooting

    public float range = 150f;
    public float damage = 80f;
    public float impactForce = 50f;
    public float fireRate = 0.15f;
    public GameObject bloodSplatter;
    public GameObject impactSpark;
    public Transform shotOrigin;
    public AudioSource audioSource;
    public AudioClip[] shotSound;
    public AudioClip reloadSound;
    public float audibleDistance;
    public LineRenderer lineRenderer;
    public Renderer muzzleFlashRenderer;
    public Light muzzleFlashLight;

    private float nextShotTime = 0;

    IEnumerator Fire(Transform directionTransform)
    {
        RaycastHit hitInfo;

        float distance = range;

        // Did we hit anything?
        if (Physics.Raycast(directionTransform.position, directionTransform.forward, out hitInfo))
        {
            if (hitInfo.distance < range)
            {
                // If it was a zombie
                if (hitInfo.transform.CompareTag("zombie"))
                {
                    hitInfo.transform.gameObject.GetComponentInParent<IDamageAble>().Damage(damage);
                    hitInfo.rigidbody.AddForce(-hitInfo.normal * impactForce, ForceMode.Impulse);

                    GameObject effect = Instantiate(bloodSplatter, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    Destroy(effect, 1f);
                }
                // If it was anything else
                else
                {
                    GameObject effect = Instantiate(impactSpark, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    Destroy(effect, 1f);
                }

                distance = hitInfo.distance;
            }
        }

        Vector3 newDirection = (hitInfo.point - shotOrigin.position).normalized;

        // Calculate the random position of the bullet trail
        float trailStartOffsetDistance = UnityEngine.Random.Range(0, distance - distance / 4f);
        Vector3 trailStart = shotOrigin.position + newDirection * trailStartOffsetDistance;

        float trailEndOffsetDistance = UnityEngine.Random.Range(distance / 4f, Mathf.Min(distance / 2f, distance - trailStartOffsetDistance));
        Vector3 trailEnd = trailStart + newDirection * trailEndOffsetDistance;

        lineRenderer.SetPosition(0, trailStart);
        lineRenderer.SetPosition(1, trailEnd);

        // Enable bullet trail, muzzle flash mesh, and muzzle flash light
        lineRenderer.enabled = true;

        // Play shot sounds.
        int n = UnityEngine.Random.Range(1, shotSound.Length);
        audioSource.clip = shotSound[n];
        audioSource.PlayOneShot(audioSource.clip, 0.8f * PlayerManager.instance.soundMultiplier);

        // move picked sound to index 0 so it's not picked next time
        shotSound[n] = shotSound[0];
        shotSound[0] = audioSource.clip;

        muzzleFlashRenderer.enabled = true;
        muzzleFlashLight.enabled = true;

        // wait one frame
        yield return new WaitForEndOfFrame();

        // Disable bullet trail, muzzle flash mesh, and muzzle flash light
        lineRenderer.enabled = false;
        muzzleFlashRenderer.enabled = false;
        muzzleFlashLight.enabled = false;
    }

    #endregion

    #region MouseLook

    public void Init(Transform character, Transform camera)
    {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
        EventManager.MouseShouldHide += SetCursorLock;
    }


    public void LookRotation(Transform character, Transform camera)
    {
        if (lockCursor)
        {
            float yRot = Input.GetAxis("Mouse X") * XSensitivity;
            float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }
        }

        UpdateCursorLock();
    }

    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    #endregion

    private void OnDestroy()
    {
        EventManager.PlayerEnteredMissionVehicle -= PlayerEnteredMissionVehicle;
        EventManager.MouseShouldHide -= SetCursorLock;
    }
}
