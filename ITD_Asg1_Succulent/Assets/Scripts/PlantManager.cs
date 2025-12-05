// PlantManager.cs
using UnityEngine;
using UnityEngine.UI;

public class PlantManager : MonoBehaviour
{
    public ImageTracker imageTracker;
    public GameObject infoPanel; // optional global fallback panel

    void Awake()
    {
        if (imageTracker != null)
        {
            imageTracker.OnPlantActivated += SetupButtons;
            imageTracker.OnPlantDeactivated += OnPlantDeactivated;
        }
    }

    void OnDestroy()
    {
        if (imageTracker != null)
        {
            imageTracker.OnPlantActivated -= SetupButtons;
            imageTracker.OnPlantDeactivated -= OnPlantDeactivated;
        }
    }

    private void SetupButtons(GameObject plant)
    {
        if (plant == null) return;

        Transform canvas = plant.transform.Find("Canvas");
        if (canvas == null) return;

        // Water
        Button water = canvas.Find("Water")?.GetComponent<Button>();
        if (water != null)
        {
            water.onClick.RemoveAllListeners();
            water.onClick.AddListener(() => WaterPlant(plant));
        }

        // Fertilize
        Button fertilize = canvas.Find("Fertilize")?.GetComponent<Button>();
        if (fertilize != null)
        {
            fertilize.onClick.RemoveAllListeners();
            fertilize.onClick.AddListener(() => FertilizePlant(plant));
        }

        // Info
        Button info = canvas.Find("Info")?.GetComponent<Button>();
        if (info != null)
        {
            info.onClick.RemoveAllListeners();
            info.onClick.AddListener(() => ShowInfo(plant));
        }

        // Ensure prefab info panel starts hidden
        Transform infoPanelLocal = canvas.Find("InfoPanel");
        if (infoPanelLocal != null)
            infoPanelLocal.gameObject.SetActive(false);
    }

    private void OnPlantDeactivated(GameObject plant)
    {
        if (plant == null) return;

        Transform panel = plant.transform.Find("Canvas/InfoPanel");
        if (panel != null)
            panel.gameObject.SetActive(false);

        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    private void WaterPlant(GameObject plant)
    {
        if (plant == null) return;

        PlantGrowth growth = plant.GetComponent<PlantGrowth>();
        if (growth != null)
        {
            growth.Grow();
            Debug.Log($"Watered {plant.name} -> level {growth.growth}");
        }
    }

    private void FertilizePlant(GameObject plant)
    {
        if (plant == null) return;

        PlantGrowth growth = plant.GetComponent<PlantGrowth>();
        if (growth != null)
        {
            growth.Grow();
            growth.Grow();
            Debug.Log($"Fertilized {plant.name} -> level {growth.growth}");
        }
    }

    private void ShowInfo(GameObject plant)
    {
        if (plant == null) return;

        PlantGrowth growth = plant.GetComponent<PlantGrowth>();
        if (growth != null && growth.IsMature())
        {
            Transform panel = plant.transform.Find("Canvas/InfoPanel");
            if (panel != null)
            {
                panel.gameObject.SetActive(true);
                return;
            }

            if (infoPanel != null)
                infoPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Plant not mature yet or missing PlantGrowth.");
        }
    }

    // Public helper if you need to hide from other systems
    public void HideInfo(GameObject plant)
    {
        if (plant == null) return;

        Transform panel = plant.transform.Find("Canvas/InfoPanel");
        if (panel != null)
            panel.gameObject.SetActive(false);

        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}



