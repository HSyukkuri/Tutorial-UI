using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemUI : MonoBehaviour
{

    [Header("システム")]
    public Button button_System;
    public GameObject panel_System;
    public Slider slider_Master;
    public Slider slider_Music;
    public Slider slider_SFX;
    public Slider slider_System;
    public Button button_Save;
    public Button button_Load;

    [Header("ゲームオーバー")]
    public GameObject panel_GameOver;
    public Button button_Retry;
    public Button button_Back;
    public Text text_GameOver;

    [Header("タイトル")]
    public GameObject panel_Title;
    public Button button_Start_Title;
    public Button button_Load_Title;

    bool pb_System = false;
    public bool pb_Retry { get; private set; } = false;
    public bool pb_Back { get; private set; } = false;
    public bool pb_Save { get; private set; } = false;
    public bool pb_Load { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        button_System.onClick.AddListener(() => pb_System = true);

        slider_Master.onValueChanged.AddListener(value => AudioManager.instance.SetVol_Master(value));
        slider_SFX.onValueChanged.AddListener(value => AudioManager.instance.SetVol_SFX(value));
        slider_Music.onValueChanged.AddListener(value => AudioManager.instance.SetVol_Music(value));
        slider_System.onValueChanged.AddListener(value => AudioManager.instance.SetVol_System(value));
        button_Save.onClick.AddListener(() => pb_Save = true);
        button_Load.onClick.AddListener(() => pb_Load = true);

        //ゲームオーバー
        button_Retry.onClick.AddListener(() => pb_Retry = true);
        button_Back.onClick.AddListener(() => pb_Back = true);
        text_GameOver.text = "GameOver";

        //タイトル
        button_Start_Title.onClick.AddListener(() => pb_Retry = true);
        button_Load_Title.onClick.AddListener(() => pb_Load = true);
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
        pb_Retry = false;
        pb_Back = false;
        pb_Save = false;
        pb_Load = false;
    }
}
