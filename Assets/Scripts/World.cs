using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public GameObject spawnMaterial;
    public Transform spawnArea;
    public float spawnTime;
    public static int coinsCount = 0;

    private Slider timer;
    private Text coinsCounter;
    private static int combineCount = 0;

    private void Start()
    {
        float max_x = this.spawnArea.position.x + this.spawnArea.localScale.x / 2.0f;
        float min_x = this.spawnArea.position.x - this.spawnArea.localScale.x / 2.0f;
        float max_y = this.spawnArea.position.y + this.spawnArea.localScale.y / 2.0f;
        float min_y = this.spawnArea.position.y - this.spawnArea.localScale.y / 2.0f;

        SetupUI();
        StartCoroutine(SpawnNewMaterial(min_x, max_x, min_y, max_y));
    }

    void Update()
    {
        // If 2 materials are being combined
        DeleteAndSpawn();
        // Move timer slider
        MoveTimerSlider();

        this.coinsCounter.text = coinsCount.ToString();
    }

    private void DeleteAndSpawn()
    {
        if (combineCount == 2)
        {
            GameObject[] materials_list = GameObject.FindGameObjectsWithTag("Material");
            List<GameObject> deleted_materials_list = new List<GameObject>();
            int material_tier = 0;

            // Get the highest tier of the combining materials and add them to a list, then destroy them in the world
            foreach (GameObject material in materials_list)
            {
                if (material.GetComponent<Material>().isCombined == true)
                {
                    material_tier = material.GetComponent<Material>().tier;

                    deleted_materials_list.Add(material);

                    Destroy(material);
                }
            }

            // Instantiate a new material with a higher tier at a location between the "parent" materials
            if (deleted_materials_list.Count == 2)
            {
                GameObject first_material = deleted_materials_list[0];
                GameObject second_material = deleted_materials_list[1];
                GameObject new_material = Instantiate(this.spawnMaterial, (first_material.transform.position + second_material.transform.position) / 2, Quaternion.identity);

                new_material.GetComponent<Material>().tier = material_tier + 1;
            }

            // Reset the count of the materials that are being combined
            combineCount = 0;
        }
    }

    private IEnumerator SpawnNewMaterial(float min_x, float max_x, float min_y, float max_y)
    {
        yield return new WaitForSeconds(this.spawnTime);

        // Spawn a new material in the world at a random location on the screen
        GameObject new_item = Instantiate(this.spawnMaterial, new Vector3(Random.Range(min_x, max_x), Random.Range(min_y, max_y), 0), Quaternion.identity);

        // Recursively call this method
        StartCoroutine(SpawnNewMaterial(min_x, max_x, min_y, max_y));
    }

    private void SetupUI()
    {
        // Set slider
        this.timer = GameObject.Find("SpawnTimer").GetComponent<Slider>();
        this.timer.minValue = this.timer.value = 0;
        this.timer.maxValue = this.spawnTime;

        // Set coin counter
        this.coinsCounter = GameObject.Find("Count").GetComponent<Text>();
    }

    private void MoveTimerSlider()
    {
        this.timer.value += Time.deltaTime;
        if (this.timer.value >= spawnTime)
            this.timer.value = 0;
    }

    public void AddCount()
    {
        combineCount++;
    }
}
