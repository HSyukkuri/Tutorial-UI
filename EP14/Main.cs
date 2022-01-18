using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public Text text;
    public GameObject buttonPrefab;
    public GameObject buttonHolder;
    public RectTransform hpBar;
    public Button button_main;
    bool pb_main = false;

    public List<RoomData> roomDataList;//部屋データのリスト

    public RoomData startRoomData;//ゲーム開始時点での部屋
    public Room currentRoom;//現在いる部屋

    public Image backGroundImage;
    public Image enemyImage;

    World world;
    Inventory inventory;

    InventoryUI inventoryUI;
    SystemUI systemUI;

    float playerHP = 1;

    enum gmState {
        MainCommand,
        Search_SelectNextRoom,//どの部屋に移動するか選ぶ
        Search_EnterNewRoom,//新しい部屋に入る
        Search_FindItem,//アイテム見つけた
        Search_FindNothing,//何も見つからなかった
        Inventory_LookInventory,//インベントリを見ている
        Inventory_Combine,//アイテムを組み合わせようとしている
        Combat_EnemyEncount,//敵とエンカウント！
        Combat_Command,//戦闘コマンド
        Combat_EnemyAttack,//敵の攻撃
        Combat_Attack,//敵に攻撃
        Combat_Win,//敵に勝利

        System_GameOver,//ゲームオーバー
    }

    gmState priviousState = gmState.Search_EnterNewRoom;
    gmState currentState = gmState.Search_EnterNewRoom;
    bool stateEnter = true;
    void ChangeState(gmState newState) {
        priviousState = currentState;
        currentState = newState;
        stateEnter = true;
        DeleteButton();
        Debug.Log(priviousState.ToString() + "⇒" + currentState.ToString());
    }


    enum Command {
        Move,
        Search,
        OpenInventory,
        Attack,
        Wait,
    }

    List<Command> commandList = new List<Command>();
    int commandIndex = -1;


    void CreateButton(List<Command> commandList) {
        DeleteButton();
        foreach (Command command in commandList) {
            GameObject newButton = Instantiate(buttonPrefab);
            newButton.transform.SetParent(buttonHolder.transform, false);

            switch (command) {
                case Command.Move:
                    newButton.GetComponentInChildren<Text>().text = "移動";
                    break;

                case Command.Search:
                    newButton.GetComponentInChildren<Text>().text = "探索";
                    break;

                case Command.OpenInventory:
                    newButton.GetComponentInChildren<Text>().text = "インベントリ";
                    break;

                case Command.Attack:
                    newButton.GetComponentInChildren<Text>().text = "攻撃";
                    break;

                case Command.Wait:
                    newButton.GetComponentInChildren<Text>().text = "待機";
                    break;
            }

            newButton.GetComponent<Button>().onClick.AddListener(() => commandIndex = newButton.transform.GetSiblingIndex());
        }
    }

    void CreateButton(string[] textArray) {
        DeleteButton();
        foreach (string text in textArray) {
            GameObject newButton = Instantiate(buttonPrefab);
            newButton.transform.SetParent(buttonHolder.transform, false);
            newButton.GetComponentInChildren<Text>().text = text;
            newButton.GetComponent<Button>().onClick.AddListener(() => commandIndex = newButton.transform.GetSiblingIndex());
        }
    }

    void DeleteButton() {
        foreach(Transform transform in buttonHolder.transform) {
            Destroy(transform.gameObject);
        }
    }

    private void Start() {

        //worldクラス初期化
        world = new World(roomDataList);
        currentRoom = world.GetRoom(startRoomData);

        //inventory初期化
        inventory = new Inventory(this);

        //コンポーネント取得
        inventoryUI = GetComponent<InventoryUI>();
        systemUI = GetComponent<SystemUI>();

        //メインボタン初期化
        button_main.onClick.AddListener(() => pb_main = true);
    }

    private void Initialize() {
        //worldクラス初期化
        world = new World(roomDataList);
        currentRoom = world.GetRoom(startRoomData);

        //inventory初期化
        inventory = new Inventory(this);

        //player初期化
        playerHP = 1f;

        //パネルの初期化
        systemUI.panel_GameOver.SetActive(false);
        enemyImage.gameObject.SetActive(false);

        //ステート初期化
        ChangeState(gmState.Search_EnterNewRoom);
    }


    private void Update() {

        hpBar.localScale = new Vector3(playerHP, 1, 1);
        

        switch (currentState) {

            case gmState.MainCommand: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = "どうしようか？";
                        commandList = new List<Command>();
                        commandList.Add(Command.Move);
                        commandList.Add(Command.Search);
                        commandList.Add(Command.OpenInventory);

                        CreateButton(commandList);

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Click);
                        AudioManager.instance.Play_Music(AudioManager.instance.bgm_Normal);

                    }

                    if (commandIndex != -1) {

                        switch (commandList[commandIndex]) {

                            case Command.Move: {
                                    ChangeState(gmState.Search_SelectNextRoom);
                                    return;
                                }

                            case Command.Search: {
                                    if (currentRoom.itemList != null && currentRoom.itemList.Count != 0) {
                                        ChangeState(gmState.Search_FindItem);
                                        return;
                                    }
                                    else {
                                        ChangeState(gmState.Search_FindNothing);
                                        return;
                                    }
                                }

                            case Command.OpenInventory: {
                                    ChangeState(gmState.Inventory_LookInventory);
                                    return;
                                }
                        }

                    }

                    return;
                }

            case gmState.Combat_EnemyEncount: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = currentRoom.enemy.data.name + "が現れた！！！！";
                        enemyImage.sprite = currentRoom.enemy.data.sprite;
                        enemyImage.gameObject.SetActive(true);

            

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Click);
                        AudioManager.instance.Play_Music(null,0f);
                    }

                    if (pb_main) {

                        ChangeState_Attack_50_Chance();

                    }


                    return;
                }

            case gmState.Combat_Command: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = "どうする！？";
                        commandList = new List<Command>();
                        commandList.Add(Command.Attack);
                        commandList.Add(Command.Wait);
                        commandList.Add(Command.OpenInventory);

                        CreateButton(commandList);

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Click);
                        AudioManager.instance.Play_Music(AudioManager.instance.bgm_Battle, 0);
                    }

                    if (commandIndex != -1) {
                        switch (commandList[commandIndex]) {
                            case Command.Attack: {
                                    ChangeState(gmState.Combat_Attack);
                                    return;
                                }

                            case Command.Wait: {
                                    ChangeState(gmState.Combat_EnemyAttack);
                                    return;
                                }

                            case Command.OpenInventory: {
                                    ChangeState(gmState.Inventory_LookInventory);
                                    return;
                                }
                        }
                    }

                    return;
            }

            case gmState.Combat_Attack: {
                    if (stateEnter) {
                        stateEnter = false;

                        if (inventory.equip != null) {
                            text.text = $"敵に{inventory.equip.name}で{inventory.equip.atk:0.0}ダメージ与えた";
                            currentRoom.enemy.TakeDamage(inventory.equip.atk);
                        }
                        else {
                            text.text = "敵に1ダメージ与えた";
                            currentRoom.enemy.TakeDamage(1);
                        }

                        text.text += $"\n 敵HP:{currentRoom.enemy.hp:00%}";

             

                        AudioManager.instance.Play_SFX(AudioManager.instance.sfx_Attack);
                    }

                    if(pb_main) {

                        if(currentRoom.enemy.hp <= 0) {
                            ChangeState(gmState.Combat_Win);
                            return;
                        }
                        else {
                            ChangeState_Attack_50_Chance();
                            return;
                        }
            
                    }

                    

                    return;
                }

            case gmState.Combat_EnemyAttack: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = $"{currentRoom.enemy.data.name}が攻撃してきた！\n{currentRoom.enemy.data.atk:0.0}ダメージ食らった";
                        TakeDamage(currentRoom.enemy.data.atk);
                        text.text += $"\nHP:{playerHP:0.0}";


                        AudioManager.instance.Play_SFX(AudioManager.instance.sfx_Attack);
                    }

                    if(pb_main) {

                        if(playerHP <= 0) {
                            ChangeState(gmState.System_GameOver);
                            return;
                        }

                        ChangeState_Attack_50_Chance();
                    }

                    return;
                }

            case gmState.Combat_Win: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = currentRoom.enemy.data.name + "を倒したぞ";
                        currentRoom.enemy = null;
                        enemyImage.sprite = null;
                        enemyImage.gameObject.SetActive(false);

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Success);
                    }

                    if (pb_main) {
                        ChangeState(gmState.MainCommand);
                        return;
                    }

                    return;
                }

            case gmState.Inventory_LookInventory: {
                    if (stateEnter) {
                        stateEnter = false;

                        inventoryUI.inventoryPanel.SetActive(true);
                        inventoryUI.UpdateUI(inventory);

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Click);
                    }

                    if(inventoryUI.actionButtonIndex != -1) {

                        switch (inventoryUI.actionButtonIndex) {
                            case 0: {
                                    //「使う」ボタンが押された。
                                    inventory.UseItem(inventoryUI.targetIndex);
                                    inventoryUI.UpdateUI(inventory);

                                    AudioManager.instance.Play_System(AudioManager.instance.sys_Click);
                                    return;
                                }
                            case 1: {
                                    //「組合せ」ボタンが押された。
                                    ChangeState(gmState.Inventory_Combine);
                                    return;
                                }
                            case 2: {
                                    //「捨てる」ボタンが押された。
                                    return;
                                }
                        }



                    }


                    if (inventoryUI.pb_close) {
                        inventoryUI.inventoryPanel.SetActive(false);
                        if(currentRoom.enemy != null) {
                            ChangeState(gmState.Combat_Command);
                            return;
                        }
                        else {
                            ChangeState(gmState.MainCommand);
                            return;
                        }
                        
                    }

                    return;
                }

            case gmState.Inventory_Combine: {
                    if (stateEnter) {
                        stateEnter = false;
                        inventoryUI.UpdateUICombine(inventory);

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Click);
                    }

                    if (inventoryUI.combineTargetIndex != -1) {
                        Debug.Log("組み合わせるアイテムが選択された");
                        //組合せ相手のアイテムが指定された。
                        if (inventory.TryCombine(inventoryUI.targetIndex, inventoryUI.combineTargetIndex)) {
                            //組合せ成功
                            AudioManager.instance.Play_System(AudioManager.instance.sys_Success);
                            ChangeState(gmState.Inventory_LookInventory);
                            return;
                        }
                        else {
                            //組合せ失敗
                            AudioManager.instance.Play_System(AudioManager.instance.sys_Fail);
                            Debug.Log("だめです");
                        }
                    }

                    if (inventoryUI.pb_close) {
                        ChangeState(gmState.Inventory_LookInventory);
                        return;
                    }

                    return;
                }

            case gmState.Search_SelectNextRoom: {
                    if (stateEnter) {
                        stateEnter = false;

                        text.text = "どっちに移動しようか？";

                        string[] roomNameAarry = new string[currentRoom.data.connectedRoomList.Count];

                        for (int i = 0; i < currentRoom.data.connectedRoomList.Count; i++) {
                            roomNameAarry[i] = currentRoom.data.connectedRoomList[i].name;
                        }

                        CreateButton(roomNameAarry);

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Click);

                    }

                    if(commandIndex != -1) {

                        currentRoom = world.GetRoom(currentRoom.data.connectedRoomList[commandIndex]);
                        ChangeState(gmState.Search_EnterNewRoom);
                        return;
                    }

                    if (pb_main) {
                        ChangeState(gmState.MainCommand);
                        return;
                    }


                    return;
                }

            case gmState.Search_EnterNewRoom: {

                    if (stateEnter) {
                        stateEnter = false;

                        text.text = currentRoom.data.name +"へ移動した。\n"+ currentRoom.data.desc;
                        backGroundImage.sprite = currentRoom.data.sprite;
                     

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Click);

                    }

                    if(pb_main) {
                        if(currentRoom.enemy != null) {
                            ChangeState(gmState.Combat_EnemyEncount);
                            return;
                        }
                        else {
                            ChangeState(gmState.MainCommand);
                            return;
                        }
                    }


                    return;
                }

            case gmState.Search_FindNothing: {
                    if(stateEnter) {
                        stateEnter = false;
                        text.text = "何も見つからなかった";



                        AudioManager.instance.Play_System(AudioManager.instance.sys_Fail);
                    }

                    if(pb_main) {
                        ChangeState(gmState.MainCommand);
                        return;
                    }

                    return;
                }

            case gmState.Search_FindItem: {
                    if (stateEnter) {
                        stateEnter = false;

                        ItemData newItem = currentRoom.itemList[0];
                        text.text = newItem.name + "を見つけた";
                        inventory.itemDataList.Add(newItem);
                        currentRoom.itemList.Remove(newItem);
                 

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Success);
                    }
                    

                    if(pb_main) {
                        ChangeState(gmState.MainCommand);
                        return;

                    }

                    return;
                }

            case gmState.System_GameOver: {
                    if (stateEnter) {
                        stateEnter = false;
                        systemUI.panel_GameOver.SetActive(true);
                        AudioManager.instance.Play_Music(null, 0f);
                        AudioManager.instance.Play_System(AudioManager.instance.sys_GameOver);
                    }

                    if (systemUI.pb_Retry) {
                        Initialize();
                        return;
                    }

                    return;
                }

        }




    }

    private void LateUpdate() {
        commandIndex = -1;
        pb_main = false;
    }

    private void TakeDamage(float damage) {
        playerHP -= damage * 0.1f;

        playerHP = Mathf.Clamp01(playerHP);
    }

    public void Heal(float value) {
        playerHP += value;

        playerHP = Mathf.Clamp01(playerHP);
    }

    private void ChangeState_Attack_50_Chance() {
        float randomValue = Random.value;

        if(randomValue <= 0.5f) {
            ChangeState(gmState.Combat_Command);
            return;
        }
        else {
            ChangeState(gmState.Combat_EnemyAttack);
            return;
        }
    }

}
