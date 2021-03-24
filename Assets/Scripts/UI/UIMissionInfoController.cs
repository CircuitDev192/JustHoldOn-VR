using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionInfoController : MonoBehaviour
{
    [SerializeField] private Text missionTitle;
    [SerializeField] private Text missionDescription;
    private bool visible = true;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.MissionChanged += MissionChanged;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            visible = !visible;
            missionTitle.gameObject.SetActive(visible);
            missionDescription.gameObject.SetActive(visible);
        }
    }

    private void MissionChanged(string missionTitle, string missionDescription)
    {
        this.missionTitle.text = missionTitle;
        this.missionDescription.text = missionDescription;
    }

    private void OnDestroy()
    {
        EventManager.MissionChanged -= MissionChanged;
    }
}
