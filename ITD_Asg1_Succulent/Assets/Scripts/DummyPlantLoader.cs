using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlantListUI : MonoBehaviour
{
    [Header("References")]
    public GameObject plantCardPrefab;      // Your PlantCard prefab
    public Transform plantListContent;      // ScrollView/Viewport/Content

    [Header("Dummy Settings")]
    public int numberOfDummyPlants = 3;     // How many cards to show

    void Start()
    {
        SpawnDummyPlants();
    }

    void SpawnDummyPlants()
    {
        if (plantCardPrefab == null || plantListContent == null)
        {
            Debug.LogError("[PlantListUI] Missing prefab or content reference!");
            return;
        }

        for (int i = 0; i < numberOfDummyPlants; i++)
        {
            GameObject card = Instantiate(plantCardPrefab, plantListContent);

            // These names must match your child objects in PlantCard
            var nameText = card.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            var stageText = card.transform.Find("StageText")?.GetComponent<TextMeshProUGUI>();
            var progress = card.transform.Find("ProgressSlider")?.GetComponent<Slider>();

            if (nameText != null)
                nameText.text = "Succulent " + (i + 1);

            if (stageText != null)
                stageText.text = "Stage: " + Random.Range(0, 3);

            if (progress != null)
                progress.value = Random.Range(0.2f, 1f);
        }
    }
}
