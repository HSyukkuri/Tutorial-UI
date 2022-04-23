using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




[System.Serializable]
public class SaveData {
    public int currenRoomID;
    public SaveDataOfRoom[] world;
    public SaveDataOfItem[] inventoryItemArray;
    public int inventoryEquipItemIndex;
    public float playerHP;
    public Main.AbnormalState abState;
    public float armorTime;

    public SaveData(Main main, DataBase database) {
        
        currenRoomID = database.GetRoomID(main.currentRoom.data);
        world = new SaveDataOfRoom[main.world.roomList.Count];
        for (int i = 0; i < world.Length; i++) {
            world[i] = new SaveDataOfRoom(main.world.roomList[i],database);
        }
        
        inventoryItemArray = new SaveDataOfItem[main.inventory.itemList.Count];
        for (int i = 0; i < inventoryItemArray.Length; i++) {
            inventoryItemArray[i] = new SaveDataOfItem(main.inventory.itemList[i], database);
        }

        inventoryEquipItemIndex = -1;
        for (int i = 0; i < main.inventory.itemList.Count; i++) {
            if(main.inventory.itemList[i] == main.inventory.equip) {
                inventoryEquipItemIndex = i;
                break;
            }
        }

        playerHP = main.playerHP;
        abState = main.abState;
        armorTime = main.armorTime;
    }

}


public class Main : MonoBehaviour
{
    public Text text;
    public GameObject buttonPrefab;
    public GameObject buttonHolder;
    public RectTransform hpBar;
    public Button button_main;
    bool pb_main = false;

    public DataBase database;

    public Room currentRoom;//現在いる部屋

    public Image backGroundImage;
    public Image enemyImage;
    public Image eventImage;

    public World world { get; private set; }
    public Inventory inventory { get; private set; }

    InventoryUI inventoryUI;
    SystemUI systemUI;

    public float playerHP { get; private set; } = 1;
    int distanceFromEnemy;
    float playerActionBuff = 1f;

    EventData currentEvent;
    int currentEventIndex = 0;

    public enum gmState {
        MainCommand,

        Search_SelectNextRoom,//どの部屋に移動するか選ぶ
        Search_EnterNewRoom,//新しい部屋に入る
        Search_FindItem,//アイテム見つけた
        Search_FindNothing,//何も見つからなかった
        Search_PoisonProgress,//毒によるダメージを受けている

        Inventory_LookInventory,//インベントリを見ている
        Inventory_Combine,//アイテムを組み合わせようとしている

        Combat_EnemyEncount,//敵とエンカウント！
        Combat_Command,//戦闘コマンド
        Combat_EnemyAttack,//敵の攻撃
        Combat_Attack,//敵に攻撃
        Combat_Win,//敵に勝利
        Combat_EnemyAproach,//敵が近づく
        Combat_Approach,//こちらが敵に近づく
        Combat_StepBack,//こちらが敵から後退する
        Combat_GetPoison,//毒を食らった！
        Combat_PoisonProgress,//毒がまわっている

        System_Event, //イベント
        System_GameOver,//ゲームオーバー
    }

    public enum AbnormalState {
        Normal,
        Poison,
        Armor
    }

    //状態異常ステータス
    public AbnormalState abState { get; private set; } = AbnormalState.Normal;

    //毒関連
    const float POISON_PROGRESS = 0.05f;
    const float POISON_LIMIT = 0.05f;

    //バフ関連
    public float armorTime { get; private set; } = 1;
    const float ARMOR_BUFF = 1.5f;
    const float ARMOR_DECREASE = 0.33f;

    gmState priviousState = gmState.Search_EnterNewRoom;
    gmState currentState = gmState.Search_EnterNewRoom;
    bool stateEnter = true;
    void ChangeState(gmState newState) {
        priviousState = currentState;
        currentState = newState;
        stateEnter = true;
        DeleteButton();
        Debug.Log(priviousState.ToString() + "⇒" + currentState.ToString());

        if (currentState == gmState.MainCommand) {
            systemUI.ShowSaveButton();
        }
        else {
            systemUI.HideSaveButton();
        }
    }


    enum Command {
        Move,
        Search,
        OpenInventory,
        Attack,
        Wait,
        Approach,
        StepBack,
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

                case Command.Approach:
                    newButton.GetComponentInChildren<Text>().text = "近づく";
                    break;

                case Command.StepBack:
                    newButton.GetComponentInChildren<Text>().text = "離れる";
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

        //コンポーネント取得
        inventoryUI = GetComponent<InventoryUI>();
        systemUI = GetComponent<SystemUI>();

        //メインボタン初期化
        button_main.onClick.AddListener(() => pb_main = true);

        Initialize();
    }

    private void Initialize() {
        //worldクラス初期化
        world = new World(database.roomDatabase);
        currentRoom = world.GetRoom(database.roomDatabase[0]);

        //inventory初期化
        inventory = new Inventory(this);

        //player初期化
        playerHP = 1f;

        //パネルの初期化
        systemUI.panel_GameOver.SetActive(false);
        enemyImage.gameObject.SetActive(false);
        systemUI.panel_System.SetActive(false);

        //ステート初期化
        StartEvent(database.roomDatabase[0].eventData);
        ChangeState(gmState.System_Event);
        backGroundImage.sprite = currentRoom.data.sprite;

        //状態異常初期化
        abState = AbnormalState.Normal;
        armorTime = 1;
    }
    
    public void Save() {
        SaveData newSaveData = new SaveData(this, database);

        SaveSystem.SaveGame(newSaveData);

    }

    private void Load(SaveData saveData) {
        //worldクラス初期化
        world = new World(saveData.world, database);
        currentRoom = world.GetRoom(database.roomDatabase[saveData.currenRoomID]);

        Debug.Log("worldクラス初期化成功");

        //Inventory初期化
        inventory = new Inventory(this, saveData.inventoryItemArray, saveData.inventoryEquipItemIndex, database);

        Debug.Log("Inventory初期化成功");

        //player初期化
        playerHP = saveData.playerHP;

        //パネルの初期化
        systemUI.panel_GameOver.SetActive(false);
        enemyImage.gameObject.SetActive(false);
        systemUI.panel_System.SetActive(false);

        //ステートの初期化
        ChangeState(gmState.MainCommand);
        backGroundImage.sprite = currentRoom.data.sprite;

        //状態異常初期化
        abState = saveData.abState;
        armorTime = saveData.armorTime;

        Debug.Log("全ての初期化成功");
    }


    private void Update() {

        hpBar.localScale = new Vector3(playerHP, 1, 1);
        

        switch (currentState) {

            case gmState.MainCommand: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = $"{currentRoom.desc}\nどうしようか？";
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

                    if (systemUI.pb_Save) {
                        Save();
                        AudioManager.instance.Play_System(AudioManager.instance.sys_Success);
                    }

                    if (systemUI.pb_Load) {
                        Load(SaveSystem.LoadGame());
                    }

                    return;
                }
            #region Combat
            case gmState.Combat_EnemyEncount: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = currentRoom.enemy.data.name + "が現れた！！！！";
                        enemyImage.sprite = currentRoom.enemy.data.sprite;
                        enemyImage.gameObject.SetActive(true);
                        distanceFromEnemy = Random.Range(2, currentRoom.data.area);
            

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Click);
                        AudioManager.instance.Play_Music(null,0f);
                    }

                    if (pb_main) {

                        if(currentRoom.enemy.data.encountEvent != null) {
                            StartEvent(currentRoom.enemy.data.encountEvent);
                            ChangeState(gmState.System_Event);
                            return;
                        }


                        if (PoisonProgress()) {
                            return;
                        }

                        ChangeState_CombatAction();

                    }


                    return;
                }

            case gmState.Combat_Command: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = $"どうする！？\n敵との距離：{distanceFromEnemy}m";
                        if(inventory.equip != null) {
                            if(inventory.equip.data is EquipRangeItemData) {
                                EquipRangeItemData equipRangeItemData = (EquipRangeItemData)inventory.equip.data;
                                float percent = equipRangeItemData.HitPercent(distanceFromEnemy);
                                text.text += $"命中率：{percent:0.0%}";
                            }
                        }

                        commandList = new List<Command>();
                        commandList.Add(Command.Attack);
                        commandList.Add(Command.Wait);
                        commandList.Add(Command.OpenInventory);
                        if(distanceFromEnemy > 1) {
                            Debug.Log("敵との距離が遠い");
                            commandList.Add(Command.Approach);
                        }

                        if(distanceFromEnemy < currentRoom.data.area) {
                            commandList.Add(Command.StepBack);
                        }

                        CreateButton(commandList);

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Click);
                        AudioManager.instance.Play_Music(AudioManager.instance.bgm_Battle, 0);
                    }

                    if (commandIndex != -1) {
                        switch (commandList[commandIndex]) {
                            case Command.Attack: {
                                    playerActionBuff -= 1f;
                                    ChangeState(gmState.Combat_Attack);
                                    return;
                                }

                            case Command.Wait: {
                                    playerActionBuff = 1f;
                                    EnemyAction();
                                    return;
                                }

                            case Command.OpenInventory: {
                                    playerActionBuff -= 1f;
                                    ChangeState(gmState.Inventory_LookInventory);
                                    return;
                                }

                            case Command.Approach: {
                                    playerActionBuff -= 1f;
                                    ChangeState(gmState.Combat_Approach);
                                    return;
                                }

                            case Command.StepBack: {
                                    playerActionBuff -= 0.5f;
                                    ChangeState(gmState.Combat_StepBack);
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
                            //武器を装備している
                            EquipItemData equipItemData = (EquipItemData)inventory.equip.data;
                            if (equipItemData is EquipRangeItemData) {
                                //武器が遠距離武器の場合

                                if(inventory.equip.value > 0) {
                                    //弾が残っている
                                    inventory.equip.value -= 1;

                                    EquipRangeItemData equipRangeItemData = (EquipRangeItemData)equipItemData;

                                    float percent = equipRangeItemData.HitPercent(distanceFromEnemy);

                                    if(percent >= Random.value) {
                                        //命中！
                                        text.text = $"敵に{equipItemData.name}で{equipItemData.atk:0.0}ダメージ与えた";
                                        currentRoom.enemy.TakeDamage(equipItemData.atk);
                                    }
                                    else {
                                        //外した・・・
                                        text.text = "外した・・・";
                                    }


                                    AudioManager.instance.Play_SFX(AudioManager.instance.sfx_Attack);
                                }
                                else {
                                    //弾が残っていない！
                                    text.text = "弾切れだ！";
                                    AudioManager.instance.Play_SFX(AudioManager.instance.sys_Fail);
                                }

                            }
                            else {
                                //武器が近距離武器の場合
                                if(distanceFromEnemy > 1) {
                                    //遠すぎる
                                    text.text = "攻撃が届かない！";
                                    AudioManager.instance.Play_SFX(AudioManager.instance.sys_Fail);
                                }
                                else {
                                    //攻撃が届く！
                                    text.text = $"敵に{equipItemData.name}で{equipItemData.atk:0.0}ダメージ与えた";
                                    currentRoom.enemy.TakeDamage(equipItemData.atk);
                                    AudioManager.instance.Play_SFX(AudioManager.instance.sfx_Attack);
                                }
                            }

                        }
                        else {
                            //武器を装備していない
                            if (distanceFromEnemy > 1) {
                                //攻撃が届かない！
                                text.text = "攻撃が届かない！";
                                AudioManager.instance.Play_System(AudioManager.instance.sys_Fail);
                            }
                            else {
                                //攻撃が届く
                                text.text = "敵に1ダメージ与えた";
                                currentRoom.enemy.TakeDamage(1);
                                AudioManager.instance.Play_SFX(AudioManager.instance.sfx_Attack);
                            }

                        }

                        text.text += $"\n 敵HP:{currentRoom.enemy.hp:00%}";

             


                    }

                    if(pb_main) {

                        if(currentRoom.enemy.hp <= 0) {
                            ChangeState(gmState.Combat_Win);
                            return;
                        }
                        else {

                            if (PoisonProgress()) {
                                return;
                            }

                            ChangeState_CombatAction();
                            return;
                        }
            
                    }

                    

                    return;
                }

            case gmState.Combat_GetPoison: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = $"毒におかされた！";
                        abState = AbnormalState.Poison;
                    }

                    if (pb_main) {
                        ChangeState_CombatAction();
                        return;
                    }

                    return;
                }

            case gmState.Combat_PoisonProgress: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = "毒のダメージを受けている！";
                        if(playerHP >= POISON_LIMIT + POISON_PROGRESS) {
                            playerHP -= POISON_PROGRESS;
                        }
                        else {
                            playerHP = POISON_LIMIT;
                        }
                    }

                    if (pb_main) {
                        ChangeState_CombatAction();
                        return;
                    }

                    return;
                }

            case gmState.Combat_EnemyAttack: {
                    if (stateEnter) {
                        stateEnter = false;

                        float damage = (abState == AbnormalState.Armor) ? currentRoom.enemy.data.atk / ARMOR_BUFF : currentRoom.enemy.data.atk;

                        text.text = $"{currentRoom.enemy.data.name}が攻撃してきた！\n{damage:0.0}ダメージ食らった";
                        TakeDamage(damage);
                        


                        AudioManager.instance.Play_SFX(AudioManager.instance.sfx_Attack);
                    }

                    if(pb_main) {

                        if(playerHP <= 0) {
                            ChangeState(gmState.System_GameOver);
                            return;
                        }

                        if (currentRoom.enemy.data.isToxic && abState == AbnormalState.Normal) {
                            //敵が毒持ちだ！
                            ChangeState(gmState.Combat_GetPoison);
                            return;
                        }

                        if (PoisonProgress()) {
                            return;
                        }

                        ChangeState_CombatAction();
                    }

                    return;
                }

            case gmState.Combat_Win: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = currentRoom.enemy.data.name + "を倒したぞ";

                        enemyImage.sprite = null;
                        enemyImage.gameObject.SetActive(false);

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Success);
                    }

                    if (pb_main) {

                        if (currentRoom.enemy.data.winEvent != null) {
                            StartEvent(currentRoom.enemy.data.winEvent);
                            ChangeState(gmState.System_Event);
                            currentRoom.enemy = null;
                            return;
                        }
                        else {
                            ChangeState(gmState.MainCommand);
                            currentRoom.enemy = null;
                            return;
                        }
                    }

                    return;
                }

            case gmState.Combat_EnemyAproach: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = $"{currentRoom.enemy.data.name}が近づいてきた！";
                        distanceFromEnemy -= 1;

                        AudioManager.instance.Play_SFX(AudioManager.instance.sfx_Move);
                    }

                    if (pb_main) {

                        if (PoisonProgress()) {
                            return;
                        }

                        ChangeState_CombatAction();
                        return;
                    }

                    return;
                }

            case gmState.Combat_Approach: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = "敵に近づいた";
                        distanceFromEnemy -= 1;

                        AudioManager.instance.Play_SFX(AudioManager.instance.sfx_Move);
                    }

                    if (pb_main) {

                        if (PoisonProgress()) {
                            return;
                        }

                        ChangeState_CombatAction();
                        return;
                    }

                    return;
                }

            case gmState.Combat_StepBack: {
                    if (stateEnter){
                        stateEnter = false;
                        text.text = "敵から離れた";
                        distanceFromEnemy += 1;
                        AudioManager.instance.Play_SFX(AudioManager.instance.sfx_Move);
                    }

                    if (pb_main) {

                        if (PoisonProgress()) {
                            return;
                        }


                        ChangeState_CombatAction();
                        return;
                    }

                    return;
                }
            #endregion

            #region Inventory
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
            #endregion

            #region Search
            case gmState.Search_SelectNextRoom: {
                    if (stateEnter) {
                        stateEnter = false;

                        text.text = "どっちに移動しようか？";

                        string[] roomNameAarry = new string[currentRoom.connectedRoomList.Count];

                        for (int i = 0; i < currentRoom.connectedRoomList.Count; i++) {
                            roomNameAarry[i] = currentRoom.connectedRoomList[i].name;
                        }

                        CreateButton(roomNameAarry);

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Click);

                    }

                    if(commandIndex != -1) {

                        currentRoom = world.GetRoom(currentRoom.connectedRoomList[commandIndex]);
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

                        text.text = currentRoom.data.name +"へ移動した。\n"+ currentRoom.desc;
                        backGroundImage.sprite = currentRoom.data.sprite;
                     

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Click);

                        //アーマー減少
                        armorTime -= ARMOR_DECREASE;
                        if(armorTime <= 0 && abState == AbnormalState.Armor) {
                            //アーマーが切れた
                            abState = AbnormalState.Normal;
                            text.text += $"アーマーが切れた！";
                        }

                    }

                    if(pb_main) {

                        if(currentRoom.eventData != null) {
                            StartEvent(currentRoom.eventData);
                            currentRoom.eventData = null;

                            ChangeState(gmState.System_Event);
                            return;
                        }

                        if(currentRoom.enemy != null) {
                            ChangeState(gmState.Combat_EnemyEncount);
                            return;
                        }
                        else {

                            if (PoisonProgress(false)) {
                                return;
                            }

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

                        if (PoisonProgress(false)) {
                            return;
                        }

                        ChangeState(gmState.MainCommand);
                        return;
                    }

                    return;
                }

            case gmState.Search_FindItem: {
                    if (stateEnter) {
                        stateEnter = false;

                        Item newItem = currentRoom.itemList[0];
                        text.text = newItem.data.name + "を見つけた";
                        inventory.itemList.Add(newItem);
                        currentRoom.itemList.Remove(newItem);
                 

                        AudioManager.instance.Play_System(AudioManager.instance.sys_Success);
                    }
                    

                    if(pb_main) {

                        if (PoisonProgress(false)) {
                            return;
                        }

                        ChangeState(gmState.MainCommand);
                        return;

                    }

                    return;
                }

            case gmState.Search_PoisonProgress: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = $"毒のダメージを受けている";
                        if (playerHP >= POISON_LIMIT + POISON_PROGRESS) {
                            playerHP -= POISON_PROGRESS;
                        }
                        else {
                            playerHP = POISON_LIMIT;
                        }

                    }

                    if (pb_main) {
                        ChangeState(gmState.MainCommand);
                        return;
                    }


                    return;
                }
            #endregion

            #region System

            case gmState.System_Event: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = currentEvent.textList[currentEventIndex];
                        currentEventIndex++;
                    }

                    if (pb_main) {
                        if(currentEventIndex < currentEvent.textList.Count) {
                            ChangeState(gmState.System_Event);
                            return;
                        }
                        else {
                            eventImage.gameObject.SetActive(false);

                            if(currentEvent.nextEvent != null) {
                                StartEvent(currentEvent.nextEvent);
                                ChangeState(gmState.System_Event);
                                return;
                            }
                            else {
                                if(currentRoom.enemy != null) {
                                    ChangeState_CombatAction();
                                    return;
                                }
                                else {
                                    ChangeState(gmState.MainCommand);
                                    return;
                                }
                            }
                        }
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
                #endregion
        }




    }

    private void LateUpdate() {
        commandIndex = -1;
        pb_main = false;
    }

    private void StartEvent(EventData data) {
        currentEvent = data;
        currentEventIndex = 0;
        if (currentEvent.image != null) {
            eventImage.sprite = currentEvent.image;
            eventImage.gameObject.SetActive(true);
        }

        if(data.sound != null) {
            AudioManager.instance.Play_SFX(data.sound);
        }

        if(data.roomOverrideList.Count != 0) {
            foreach(RoomOverride roomOverride in data.roomOverrideList) {
                roomOverride.OverrideRoom(this);
            }
        }
    }

    private void TakeDamage(float damage) {
        playerHP -= damage * 0.1f;

        playerHP = Mathf.Clamp01(playerHP);
    }

    public void Heal(HealItemData healItemData) {
        playerHP += healItemData.healValue;

        playerHP = Mathf.Clamp01(playerHP);

        if(abState == AbnormalState.Poison && healItemData.isAntidote) {
            abState = AbnormalState.Normal;
            text.text = $"毒が治った！";
        }

        if (healItemData.isArmor) {
            abState = AbnormalState.Armor;
            armorTime = 1;
            text.text += $"\n防御力が一時的に上がった！";
        }
    }

    private void ChangeState_CombatAction() {
        float randomValue = Random.value;

        float enemyDex = currentRoom.enemy.data.dex;

        float percent = 0.3f + 0.4f * ((playerHP + enemyDex - 1) / 2) * playerActionBuff;

        if(randomValue <= percent) {
            ChangeState(gmState.Combat_Command);
            return;
        }
        else {
            EnemyAction();
            return;
        }
    }

    private bool PoisonProgress(bool isCombat = true) {
        if (playerHP > POISON_LIMIT && abState == AbnormalState.Poison) {
            if (isCombat) {
                ChangeState(gmState.Combat_PoisonProgress);
            }
            else {
                ChangeState(gmState.Search_PoisonProgress);
            }

            return true;
        }

        return false;
    }

    void EnemyAction() {
        playerActionBuff += 1f;
        if(distanceFromEnemy >= 2) {
            ChangeState(gmState.Combat_EnemyAproach);
            return;
        }
        else {
            ChangeState(gmState.Combat_EnemyAttack);
            return;
        }
    }

}
