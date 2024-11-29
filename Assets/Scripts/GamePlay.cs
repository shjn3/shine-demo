using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class GamePlay : MonoBehaviour
{
    public GameObject tubePrefab;
    public List<List<Tube>> tubeGrid = null;
    public LevelData levelData;


    void Awake()
    {
        TextAsset myAsset = Resources.Load<TextAsset>("levels/level_00008");
        LevelData levelData = JsonConvert.DeserializeObject<LevelData>(myAsset.text);
        Generate(levelData);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Generate(LevelData levelData)
    {

        //Destroy old objects
        if (tubeGrid != null)
        {
            foreach (List<Tube> tubes in tubeGrid)
            {
                foreach (Tube tube in tubes)
                {
                    Destroy(tube.gameObject);
                }
            }
            tubeGrid = null;
        }

        //Generate new tubes
        int maxRow = 3;
        int bottleCount = levelData.bottleList.Count + 1;
        bool stagger = bottleCount % 2 == 0;
        int maxTubeInRow = Math.Max(1, Mathf.RoundToInt(bottleCount / maxRow));
        tubeGrid = new();
        float gapVertical = 50f;
        float x = 0;
        float y = (TubeConfig.height + gapVertical) / 2;
        float remainingHorizontalSpace = Math.Max(0, GameSceneConfig.width - maxTubeInRow * TubeConfig.width);
        float gapHorizontal = remainingHorizontalSpace / (maxTubeInRow + 1);

        for (int row = 0; row < maxRow; row++)
        {
            var tubeList = new List<Tube>();
            int tubeInRow = stagger && row % 2 != 0 ? maxTubeInRow - 1 : maxTubeInRow;

            float marginLeft = stagger && row % 2 != 0 ? Math.Max(0, GameSceneConfig.width - (tubeInRow * TubeConfig.width + gapHorizontal * (tubeInRow - 1))) / 2f : gapHorizontal;

            x = -GameSceneConfig.width / 2 + marginLeft + TubeConfig.width / 2;
            for (int column = 0; column < tubeInRow; column++)
            {
                GameObject tubeObject = Instantiate(tubePrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
                tubeList.Add(tubeObject.GetComponent<Tube>());
                tubeObject.transform.localPosition = new Vector3(x, y, 0);
                x += TubeConfig.width + gapHorizontal;
            }
            tubeGrid.Add(tubeList);
            y -= TubeConfig.height + gapVertical;
        }
    }
}
