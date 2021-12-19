using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemUI : MonoBehaviour
{
    public Button button_System;
    public GameObject panel_System;
    public Slider slider_Master;
    public Slider slider_Music;
    public Slider slider_SFX;
    public Slider slider_System;

    bool pb_System = false;

    // Start is called before the first frame update
    void Start()
    {
        button_System.onClick.AddListener(() => pb_System = true);

        slider_Master.onValueChanged.AddListener(delegate { AudioManager.instance.SetVol_Master(slider_Master.value); });
        slider_SFX.onValueChanged.AddListener(delegate { AudioManager.instance.SetVol_SFX(slider_SFX.value); });
        slider_Music.onValueChanged.AddListener(delegate { AudioManager.instance.SetVol_Music(slider_Music.value); });
        slider_System.onValueChanged.AddListener(delegate { AudioManager.instance.SetVol_System(slider_System.value); });
    }

    // Update is called once per frame
    void Update()
    {
        if (pb_System) {

            if (panel_System.activeSelf) {
                panel_System.SetActive(false);
            }
            else {
                panel_System.SetActive(true);
            }


        }
    }

    private void LateUpdate() {
        pb_System = false;
    }
}
