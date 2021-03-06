using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject buttonPrefab;
    public Button closeButton;
    public GameObject contentHolder;
    public Text descText;

    public Button button_Use;
    public Button button_Combine;
    public Button button_Despose;


    public bool pb_close { get; private set; }

    public int targetIndex { get; private set; }

    public int actionButtonIndex { get; private set; } = -1;

    public int combineTargetIndex { get; private set; } = -1;

private void Start() {
        closeButton.onClick.AddListener(() => pb_close = true);
        button_Use.onClick.AddListener(() => actionButtonIndex = 0);
        button_Combine.onClick.AddListener(() => actionButtonIndex = 1);
        button_Despose.onClick.AddListener(() => actionButtonIndex = 2);
    }

    private void LateUpdate() {
        pb_close = false;
        actionButtonIndex = -1;
        combineTargetIndex = -1;
    }

    public void UpdateUI(Inventory inventory) {
        button_Use.gameObject.SetActive(true);
        button_Combine.gameObject.SetActive(true);
        button_Despose.gameObject.SetActive(true);


        descText.text = "";

        int currentButtonCount = contentHolder.transform.childCount;
        int currentItemCount = inventory.itemList.Count;

        if(currentButtonCount < currentItemCount) {
            //表示されているボタンの数より　持っているアイテムの数の方が多い
            int num = currentItemCount - currentButtonCount;

            for (int i = 0; i < num; i++) {
                GameObject newButtonObject = Instantiate(buttonPrefab);
                newButtonObject.transform.SetParent(contentHolder.transform, false);
            }


        }else
        if(currentButtonCount > currentItemCount) {
            //持っているアイテムの数より　表示されているボタンの数が多い
            for (int i = currentButtonCount - 1; i > currentItemCount -1; i--) {
                Destroy(contentHolder.transform.GetChild(i).gameObject);
            }
        }

        
        for (int i = 0; i < currentItemCount; i++) {

            //ボタンにアイテム名を表示する
            ItemData itemData = inventory.itemList[i].data;
            GameObject buttonObject = contentHolder.transform.GetChild(i).gameObject;
            Text text = buttonObject.GetComponentInChildren<Text>();
            text.text = itemData.name;

            //装備中のアイテムのボタンなら「E」を表示させる
            GameObject equipMark = buttonObject.transform.Find("Equip").gameObject;
            if(inventory.equip == inventory.itemList[i]) {
                equipMark.SetActive(true);
            }
            else {
                equipMark.SetActive(false);
            }

            //遠距離武器、または弾丸アイテムなら数字を表示する
            GameObject countText = buttonObject.transform.Find("Count").gameObject;
            if(itemData is EquipRangeItemData || itemData is StackItemData) {
                countText.SetActive(true);
                countText.GetComponent<Text>().text = inventory.itemList[i].value.ToString();
            }
            else {
                countText.SetActive(false);
            }

            //ボタンにaddLestenerで押された時の処理を付け加える
            Button button = buttonObject.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnClickItemButton(buttonObject.transform.GetSiblingIndex(),inventory ) );
        }
    }

    public void UpdateUICombine(Inventory inventory) {
        button_Use.gameObject.SetActive(false);
        button_Combine.gameObject.SetActive(false);
        button_Despose.gameObject.SetActive(false);

        descText.text = "どのアイテムと組み合わせる？";

        foreach(Transform buttonTransform in contentHolder.transform) {
            Button button = buttonTransform.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => combineTargetIndex = buttonTransform.GetSiblingIndex());
        }


    }

    void OnClickItemButton(int index, Inventory inventory) {
        targetIndex = index;
        descText.text = inventory.itemList[index].data.desc;

        //「使う」「組合せ」「捨てる」ボタンの表示の処理。
        if(targetIndex > -1 || targetIndex < inventory.itemList.Count) {

            if(inventory.itemList[targetIndex].data is EquipItemData) {

                if(inventory.equip != inventory.itemList[targetIndex]) {
                    button_Use.GetComponentInChildren<Text>().text = "装備";
                }
                else {
                    button_Use.GetComponentInChildren<Text>().text = "装備外す";
                }

            }
            else {
                button_Use.GetComponentInChildren<Text>().text = "使う";
            }
        }
    }
}
