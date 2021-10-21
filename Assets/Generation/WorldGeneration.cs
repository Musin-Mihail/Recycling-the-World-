using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class WorldGeneration : MonoBehaviour
{
    float size = 10.0f;
    float DistancePlayer;
    public float ChanceYellow;
    public float ChanceRed;
    public float ChanceRes3;
    float chance;
    float DistantGen = 500;
    bool DisableColor = true;
    GameObject block;
    public List<Vector3> newChank = new List<Vector3>();
    public List<Vector3> vectorChank = new List<Vector3>();
    float PerlinNoiseCave;
    private float noiseScale = 0.02f;
    Vector3 Scale1 = new Vector3(10,10,1);
    Vector3 Scale2 = new Vector3(5,5,1);
    Vector3 Scale3 = new Vector3(2.5f,2.5f,1);
    Vector3 Scale4 = new Vector3(0.125f,0.125f,1);
    float fff;
    public GameObject Chank;
    Vector3 V1 = new Vector3(1,1,0);
    Vector3 V2 = new Vector3(-1,-1,0);
    Vector3 V3 = new Vector3(-1,1,0);
    Vector3 V4 = new Vector3(1,-1,0);
    void Start()
    {
        AddNewChank(new Vector3(0,10,0));
        AddNewVectorChank(new Vector3(0,10,0));
        StartCoroutine(CheckNewChank());
    }
    IEnumerator CheckNewChank()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.0001f);
            if(newChank.Count != 0)
            {
                float value = newChank[0].x + newChank[0].y;
                if(newChank[0].x % 10 == 0 && newChank[0].y % 10 == 0)
                {
                    ChankGeneration(newChank[0], Scale1, 0.50f); //0.55
                }
                else if((newChank[0].x * 2) % 5 == 0)
                {
                    ChankGeneration(newChank[0], Scale2, 0.55f); //0.55
                }
                else if((newChank[0].x * 2) % 2.5f == 0)
                {
                    ChankGeneration(newChank[0], Scale3, 0.6f); //0.6
                }
                // else if(value % 1.25f == 0)
                // {
                //     ChankGeneration(newChank[0], Scale4, 0.9f);
                // }
                newChank.RemoveAt(0);
            }
        }
    }
    void ChankGeneration(Vector3 vector, Vector3 scale, float noiseSize)
    {
        DistancePlayer = Vector3.Distance(vector, Vector3.zero);
        if (DistancePlayer < DistantGen)
        {
            var vectorUp = vector + Vector3.up * size;
            if (vectorUp.y <= 0)
            {
                BlockGeneration(vectorUp, scale, noiseSize);
                BlockGeneration(vector - Vector3.right * size, scale, noiseSize);
                BlockGeneration(vector + Vector3.right * size, scale, noiseSize);
                BlockGeneration(vector - Vector3.up * size, scale, noiseSize);
            }
            else
            {
                AddNewChank(vector - Vector3.up * size);
            }
        }
    }
    void BlockGeneration(Vector3 target, Vector3 scale, float noiseSize)
    {
        bool CheckChank = true;
        foreach (var vector in vectorChank)
        {
            if(vector == target)
            {
                CheckChank = false;
                break;
            }
        }
        if (CheckChank)
        {
            AddNewChank(target);
            AddNewVectorChank(target);
            PerlinNoiseCave = Mathf.PerlinNoise((target.x + Global.RandomCave) * noiseScale, (target.y + Global.RandomCave) * noiseScale);
            Vector3 V1 = new Vector3(1,1,0);
            Vector3 V2 = new Vector3(-1,-1,0);
            Vector3 V3 = new Vector3(-1,1,0);
            Vector3 V4 = new Vector3(1,-1,0);
            if (PerlinNoiseCave < noiseSize)
            {
                var Deep = -target.y;
                var DistSpawn1 = 50;
                var DistSpawn2 = 150;
                var DistSpawn3 = 250;
                if (Deep < DistSpawn3)
                {
                    ChanceRes3 = (Deep - (DistSpawn3 - 100)) / 20;
                }
                else
                {
                    ChanceRes3 -= (Deep - DistSpawn3 - 100) / 20;
                }
                if (Deep < DistSpawn2)
                {
                    ChanceYellow = (Deep - (DistSpawn2 - 100)) / 20;
                }
                else
                {
                    ChanceYellow -= (Deep - DistSpawn2 - 100) / 20;
                }

                if (Deep < DistSpawn1)
                {
                    ChanceRed = (Deep - (DistSpawn1 - 100)) / 20;
                }
                else
                {
                    ChanceRed -= (Deep - DistSpawn1 - 100) / 20;
                }
                var chank = Instantiate(Chank, target, Quaternion.identity, Global.Earth.transform);
                chank.transform.localScale = scale;
                
                var spriteRenderer = chank.GetComponent<SpriteRenderer>();
                if(scale.x == 5.0f)
                {
                    spriteRenderer.color = new Color32(20,0,0,255);
                }
                else if(scale.x == 2.5f)
                {
                    spriteRenderer.color = new Color32(40,20,10,255);
                }
                else if(scale.x == 1.25f)
                {
                    spriteRenderer.color = new Color32(60,40,20,255);
                }
                else if(scale.x == 0.625f)
                {
                    spriteRenderer.color = new Color32(80,60,30,255);
                }
                else if(scale.x == 0.3125f)
                {
                    spriteRenderer.color = new Color32(110,80,40,255);
                }

                chance = UnityEngine.Random.Range(0.0f, 10.0f);
                if (chance < ChanceRed)
                {
                    chank.tag = "Red";
                    chank.name = "ChankRed";
                }
                if (chance < ChanceYellow)
                {
                    chank.tag = "Yellow";
                    chank.name = "ChankYellow";
                }
                if (chance < ChanceRes3)
                {
                    chank.tag = "Res3";
                    chank.name = "ChankRes3";
                }
                else
                {
                    chank.name = "ChankGround";
                }
                if (DisableColor == false)
                {
                    PaintBlock(chank);
                }
                if(scale.x == 10)
                {
                    CreateVector2(target,scale);
                }
                else if(scale.x == 5f)
                {
                    CreateVector(target,scale);
                }
            }
            else
            {
                AddNewChank(target + V1 * scale.x/4);
                AddNewChank(target + V2 * scale.x/4);
                AddNewChank(target + V3 * scale.x/4);
                AddNewChank(target + V4 * scale.x/4);
            }
        }
    }
    void CreateVector(Vector3 vector, Vector3 scale)
    {
        AddNewVectorChank(vector + V1 * scale.x/4);
        AddNewVectorChank(vector + V2 * scale.x/4);
        AddNewVectorChank(vector + V3 * scale.x/4);
        AddNewVectorChank(vector + V4 * scale.x/4);
    }
    void CreateVector2(Vector3 vector, Vector3 scale)
    {
        Vector3 vector1 = vector + V1 * scale.x/4;
        Vector3 vector2 = vector + V2 * scale.x/4;
        Vector3 vector3 = vector + V3 * scale.x/4;
        Vector3 vector4 = vector + V4 * scale.x/4;
        float newScale = scale.x/2;
        AddNewVectorChank(vector1);
        AddNewVectorChank(vector2);
        AddNewVectorChank(vector3);
        AddNewVectorChank(vector4);
        CreateVector3(vector1,newScale);
        CreateVector3(vector2,newScale);
        CreateVector3(vector3,newScale);
        CreateVector3(vector4,newScale);
    }
    void CreateVector3(Vector3 vector, float scale)
    {
        AddNewVectorChank(vector + V1 * scale/4);
        AddNewVectorChank(vector + V2 * scale/4);
        AddNewVectorChank(vector + V3 * scale/4);
        AddNewVectorChank(vector + V4 * scale/4);
    }
    void AddNewChank(Vector3 vector1)
    {
        bool CheckChank = true;
        foreach (var vector2 in newChank)
        {
            if(vector1 == vector2)
            {
                CheckChank = false;
                break;
            }
        }
        if (CheckChank)
        {
            foreach (var vector2 in vectorChank)
            {
                if(vector1 == vector2)
                {
                    CheckChank = false;
                    break;
                }
            }
        }
        if (CheckChank)
        {
            newChank.Add(vector1);
        }
    }
    void AddNewVectorChank(Vector3 vector1)
    {
        bool CheckChank = true;
        foreach (var vector2 in vectorChank)
        {
            if(vector1 == vector2)
            {
                CheckChank = false;
                break;
            }
        }
        if (CheckChank)
        {
            vectorChank.Add(vector1);
        }
    }
    void PaintBlock(GameObject test)
    {
        var blockColor = block.GetComponent<SpriteRenderer>();
        if (block.tag == "Red")
        {
            blockColor.color = Color.red;
        }
        else if (block.tag == "Yellow")
        {
            blockColor.color = Color.yellow;
        }
        else if (block.tag == "Res3")
        {
            blockColor.color = Color.blue;
        }
    }
    
    // void GenCaveChank(Vector3 target)
    // {
    //     var block2 = Instantiate(Global.Chank2, target, transform.rotation);
        
    //     block2.transform.localScale = new Vector3(size/2,size/2,1);
        
    //     block2.name = "Block";
    //     block2.transform.parent = Parent.transform;
    //     block2.GetComponent<Chance>().Parent = Parent;
    //     float chance = Random.Range(0.0f,10.0f);
    //     if(block2.transform.localScale.x <= 1.0f)
    //     {
    //         if(chance<8.0f)
    //         {
    //             block2.tag = "Yellow";
    //             ColorChank(block2);
    //         }
    //         if(chance<1.0f)
    //         {
    //             block2.tag = "Red";
    //             ColorChank(block2);
    //         }
    //     }
    //     else
    //     {
    //         if(transform.tag == "Red")
    //         {
    //             if(chance<6.0f)
    //             {
    //                 block2.tag = "Red";
    //                 ColorChank(block2);
    //             }
    //             if(chance<0.1f)
    //             {
    //                 block2.tag = "Yellow";
    //                 ColorChank(block2);
    //             }
    //         }
    //         else if (transform.tag == "Yellow")
    //         {
    //             if(chance<4.0f)
    //             {
    //                 block2.tag = "Yellow";
    //                 ColorChank(block2);
    //             }
    //             if (chance<0.1f)
    //             {
    //                 block2.tag = "Red";
    //                 ColorChank(block2);
    //             }
    //         }
    //     }
    // }
    // void ColorChank(GameObject test)
    // {
    //     if(DisableColor == 0)
    //     {
    //         if(test.tag == "Red")
    //         {
    //             test.GetComponent<SpriteRenderer>().color = Color.red;
    //         }
    //         if(test.tag == "Yellow")
    //         {
    //             test.GetComponent<SpriteRenderer>().color = Color.yellow;
    //         }
    //     }
    // }
//     void GenChank(Vector3 target)
//     {
// //Генерация блоков
//         var block2 = Instantiate(Global.Chank2, target, transform.rotation);
//         block2.transform.localScale = new Vector3(size/2,size/2,1);
//         block2.name = "Block";
//         block2.transform.parent = Parent.transform;
//         block2.GetComponent<Chance>().Parent = Parent;
//         chance = Random.Range(0.0f,10.0f);
        
//         if(transform.tag == "Red")
//         {
//             if(chance<6.0f)
//             {
//                 block2.tag = "Red";
//                 ColorChank(block2);
//             }
//             if(chance<0.1f)
//             {
//                 block2.tag = "Yellow";
//                 ColorChank(block2);
//             }
//         }
//         else if (transform.tag == "Yellow")
//         {
//             if(chance<4.0f)
//             {
//                 block2.tag = "Yellow";
//                 ColorChank(block2);
//             }
//             if (chance<0.1f)
//             {
//                 block2.tag = "Red";
//                 ColorChank(block2);
//             }
//         }
//     }
}