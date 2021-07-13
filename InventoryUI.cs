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


    public bool pb_close { get; private set; }

    public int targetIndex { get; private set; }

    private void Start() {
        closeButton.onClick.AddListener(() => pb_close = true);
    }

    private void LateUpdate() {
        pb_close = false;
    }

    public void UpdateUI(Inventory inventory) {
        descText.text = "";

        int currentButtonCount = contentHolder.transform.childCount;
        int currentItemCount = inventory.itemDataList.Count;

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

        //ボタンにアイテム名を表示する　→　ボタンにaddLestenerで押された時の処理を付け加える
        for (int i = 0; i < currentItemCount; i++) {
            ItemData itemData = inventory.itemDataList[i];
            GameObject buttonObject = contentHolder.transform.GetChild(i).gameObject;
            Text text = buttonObject.GetComponentInChildren<Text>();
            text.text = itemData.name;

            Button button = buttonObject.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnClickItemButton(buttonObject.transform.GetSiblingIndex(),inventory ) );
        }
    }

    void OnClickItemButton(int index, Inventory inventory) {
        targetIndex = index;
        descText.text = inventory.itemDataList[index].desc;
    }
}
