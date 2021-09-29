using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public Text text;
    public GameObject buttonPrefab;
    public GameObject buttonHolder;

    public List<RoomData> roomDataList;//部屋データのリスト

    public RoomData startRoomData;//ゲーム開始時点での部屋
    public Room currentRoom;//現在いる部屋

    public Image backGroundImage;
    public Image enemyImage;

    World world;
    Inventory inventory;

    InventoryUI inventoryUI;

    float playerHP = 1;

    enum gmState {
        MainCommand,
        SelectNextRoom,
        EnterNewRoom,
        FindItem,//アイテム見つけた
        FindNothing,//何も見つからなかった
        LookInventory,//インベントリを見ている
        EnemyEncount,//敵とエンカウント！
        CombatCommand,
        Attack,//敵に攻撃
        Win,//敵に勝利
    }

    gmState priviousState = gmState.EnterNewRoom;
    gmState currentState = gmState.EnterNewRoom;
    bool stateEnter = true;
    void ChangeState(gmState newState) {
        priviousState = currentState;
        currentState = newState;
        stateEnter = true;
    }


    enum Command {
        Move,
        Search,
        OpenInventory,
        Attack,
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
        inventory = new Inventory();

        //コンポーネント取得
        inventoryUI = GetComponent<InventoryUI>();
    }

    private void Update() {

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

                    }

                    if(commandIndex != -1) {

                        switch (commandList[commandIndex]) {

                            case Command.Move: {
                                    ChangeState(gmState.SelectNextRoom);
                                    return;
                                }

                            case Command.Search: {
                                    if (currentRoom.itemList != null && currentRoom.itemList.Count != 0) {
                                        ChangeState(gmState.FindItem);
                                        return;
                                    }
                                    else {
                                        ChangeState(gmState.FindNothing);
                                        return;
                                    }
                                }

                            case Command.OpenInventory: {
                                    ChangeState(gmState.LookInventory);
                                    return;
                                }
                        }

                    }

                    return;
                }

            case gmState.CombatCommand: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = "どうする！？";
                        commandList = new List<Command>();
                        commandList.Add(Command.Attack);
                        commandList.Add(Command.OpenInventory);

                        CreateButton(commandList);
                    }

                    if (commandIndex != -1) {
                        switch (commandList[commandIndex]) {
                            case Command.Attack: {
                                    ChangeState(gmState.Attack);
                                    return;
                                }

                            case Command.OpenInventory: {
                                    ChangeState(gmState.LookInventory);
                                    return;
                                }
                        }
                    }

                    return;
            }

            case gmState.Attack: {
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

                        CreateButton(new string[] { "OK" });
                    }

                    if(commandIndex != -1) {

                        if(currentRoom.enemy.hp <= 0) {
                            ChangeState(gmState.Win);
                            return;
                        }
                        else {
                            ChangeState(gmState.CombatCommand);
                        }
                        return;
                    }

                    

                    return;
                }

            case gmState.Win: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = currentRoom.enemy.data.name + "を倒したぞ";
                        currentRoom.enemy = null;
                        enemyImage.sprite = null;
                        enemyImage.gameObject.SetActive(false);
                        CreateButton(new string[] { "OK" });
                    }

                    if (commandIndex != -1) {
                        ChangeState(gmState.MainCommand);
                        return;
                    }

                    return;
                }

            case gmState.LookInventory: {
                    if (stateEnter) {
                        stateEnter = false;

                        inventoryUI.inventoryPanel.SetActive(true);
                        inventoryUI.UpdateUI(inventory);
                    }

                    if(inventoryUI.actionButtonIndex != -1) {

                        switch (inventoryUI.actionButtonIndex) {
                            case 0: {
                                    //「使う」ボタンが押された。
                                    inventory.EquipItem(inventoryUI.targetIndex);
                                    inventoryUI.UpdateUI(inventory);
                                    return;
                                }
                            case 1: {
                                    //「組合せ」ボタンが押された。
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
                        ChangeState(priviousState);
                        return;
                    }

                    return;
                }

            case gmState.SelectNextRoom: {
                    if (stateEnter) {
                        stateEnter = false;

                        text.text = "どっちに移動しようか？";

                        string[] roomNameAarry = new string[currentRoom.data.connectedRoomList.Count];

                        for (int i = 0; i < currentRoom.data.connectedRoomList.Count; i++) {
                            roomNameAarry[i] = currentRoom.data.connectedRoomList[i].name;
                        }

                        CreateButton(roomNameAarry);

                    }

                    if(commandIndex != -1) {

                        currentRoom = world.GetRoom(currentRoom.data.connectedRoomList[commandIndex]);
                        ChangeState(gmState.EnterNewRoom);
                        return;
                    }


                    return;
                }

            case gmState.EnterNewRoom: {

                    if (stateEnter) {
                        stateEnter = false;

                        text.text = currentRoom.data.name +"へ移動した。\n"+ currentRoom.data.desc;
                        backGroundImage.sprite = currentRoom.data.sprite;
                        CreateButton(new string[] { "OK" });

                    }

                    if(commandIndex != -1) {
                        if(currentRoom.enemy != null) {
                            ChangeState(gmState.EnemyEncount);
                            return;
                        }
                        else {
                            ChangeState(gmState.MainCommand);
                            return;
                        }
                    }


                    return;
                }

            case gmState.EnemyEncount: {
                    if (stateEnter) {
                        stateEnter = false;
                        text.text = currentRoom.enemy.data.name + "が現れた！！！！";
                        enemyImage.sprite = currentRoom.enemy.data.sprite;
                        enemyImage.gameObject.SetActive(true);

                        CreateButton(new string[] { "OK" });
                    }

                    if(commandIndex != -1) {
                        ChangeState(gmState.CombatCommand);
                        return;
                    }


                    return;
                }

            case gmState.FindNothing: {
                    if(stateEnter) {
                        stateEnter = false;
                        text.text = "何も見つからなかった";

                        CreateButton(new string[] { "OK" });
                    }

                    if(commandIndex != -1) {
                        ChangeState(gmState.MainCommand);
                        return;
                    }

                    return;
                }

            case gmState.FindItem: {
                    if (stateEnter) {
                        stateEnter = false;

                        ItemData newItem = currentRoom.itemList[0];
                        text.text = newItem.name + "を見つけた";
                        inventory.itemDataList.Add(newItem);
                        currentRoom.itemList.Remove(newItem);
                        CreateButton(new string[] { "OK" });
                    }

                    if(commandIndex != -1) {
                        ChangeState(gmState.MainCommand);
                        return;

                    }

                    return;
                }

        }




    }

    private void LateUpdate() {
        commandIndex = -1;
    }

}
