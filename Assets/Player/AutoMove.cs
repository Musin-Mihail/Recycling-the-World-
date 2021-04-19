using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AutoMove : MonoBehaviour
{
    //Авто перемещение персонажем
    Rigidbody2D _Rigidbody;
    int layerMask = 1 << 8; //Block
    int layerMask2 = 1 << 17; //TopBlock
    int layerMask3;
    // int layerMask4 = 1 << 18; //Dron
    int busy = 1;
    int _storagefull = 0;
    public int _energyfull = 1;
    Collider2D[] hitColliders;
    public List<Collider2D> _ListBlock;
    public GameObject _Hand;
    public GameObject OldBuildings;
    public Vector3 _StartVector3;
    GameObject _target;
    int radius = 20;
    public int _energy;
    public int _red;
    public int _yellow;
    public int _blue;
    public int _storageCount;
    public GameObject _targetDigger;
    public int _drill;

    void Start()
	{
        _drill = 0;
        _energy = 5000;
        layerMask3 = layerMask | layerMask2;
        _StartVector3 = new Vector3 (0,0,0);
        _Rigidbody = GetComponent<Rigidbody2D>();
        Invoke("StartMove", 5.0f);
        StartCoroutine(ReSort());
    }
    void Update()
    {
        if (_ListBlock.Count == 0 && busy == 0)
        {
            busy = 1;
            SearchBlock();
        }
        if (_storagefull == 1 || _energyfull == 0 )
        {
            if(_target == null)
            {
                SearchNearestBase();
            }
            // else if (_target.GetComponent<Busy>()._busy == 1)
            // {
            //     SearchNearestBase();
            // }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position,_target.transform.position, 0.1f);
                _Hand.transform.rotation = Quaternion.LookRotation(Vector3.forward, _target.transform.position - _Hand.transform.position);
                if(Vector3.Distance(transform.position, _target.transform.position) < 0.2f)
                {
                    if(_energy < UpdatePlayer.EnergyCountMax)
                    {
                        _energy +=50;
                    }                
                    else if(_energyfull == 0)
                    {
                        _energyfull = 1;
                        // GetComponent<CircleCollider2D>().isTrigger = false;
                    }
                    if(_target.name == "MainBase")
                    {
                        for(int i = 0; i < Global.storage; i++)
                        {
                            if(_red > 0)
                            {
                                _red --;
                                Global.RedBase ++;
                            }
                            else if(_yellow > 0)
                            {
                                _yellow --;
                                Global.YellowBase ++;
                            }
                            else if(_blue > 0)
                            {
                                _blue --;
                                Global.BlueBase ++;
                            }
                        }
                    }
                    else if(_target.tag == "Base")
                    {
                        for(int i = 0; i < Global.storage; i++)
                        {
                            if(_red > 0)
                            {
                                _target.GetComponent<Base>().Red ++;
                                _red --;
                            }
                            else if(_yellow > 0)
                            {
                                _target.GetComponent<Base>().Yellow ++;
                                _yellow --;
                            }
                            else if(_blue > 0)
                            {
                                _target.GetComponent<Base>().Blue ++;
                                _blue --;
                            }
                        }
                    }
                    _storageCount = _red + _yellow + _blue;
                }
                if (_storageCount == 0 && _storagefull == 1)
                {
                    _storagefull = 0;
                }
                return;
            }
        }
        else if (_ListBlock.Count > 0)
        {
            if (_ListBlock[0] == null)
            {
                _ListBlock.RemoveAt(0);
            }
            else if (_drill == 0)
            {
                transform.position = Vector2.MoveTowards(transform.position,_ListBlock[0].transform.position, 0.1f);
                _Hand.transform.rotation = Quaternion.LookRotation(Vector3.forward, _ListBlock[0].transform.position - _Hand.transform.position);
                _energy -=5;
            }
        }
        if (_energy <= 0 && _energyfull == 1)
        {
            _energyfull = 0;
            _storagefull = 1;
        }
        if (_storageCount >= UpdatePlayer.storageCountMax && _storagefull == 0)
        {
            _storagefull = 1;
            _energyfull = 0;
        }
        if(_energyfull == 1 && _storageCount == 0 && _target != null)
        {
            _target.GetComponent<Busy>()._busy = 0;
            _target = null;
        }
        RaycastHit2D hit = Physics2D.Raycast(_Hand.transform.position, _Hand.transform.up,30,layerMask3);
        if(hit && hit.collider.name == "Top")
        {
            Destroy(hit.collider.gameObject);
        }
        // RaycastHit2D hit2 = Physics2D.Raycast(_Hand.transform.position, _Hand.transform.up,30,layerMask4);
        // if(hit2)
        // {
        //     Debug.Log(transform.position + " - " + hit2.transform.position);
        //     transform.position = transform.position + (transform.position - hit2.transform.position);
        // }
    }
    void SearchBlock()
    {
        if(_targetDigger == null)
        {
            if(Global.BuildingsDiger.Count >0)
            {
                _targetDigger = Global.BuildingsDiger[0];
                Global.BuildingsDiger.RemoveAt(0);
            }
            else
            {
                foreach (var item in Global.BuildingsCharge)
                {
                    hitColliders = Physics2D.OverlapCircleAll(item.transform.position, radius, layerMask);
                    if(hitColliders.Length > 0)
                    {
                        _targetDigger = item;
                        break;
                    } 
                }
            }
        }
        else
        {
            hitColliders = Physics2D.OverlapCircleAll(_targetDigger.transform.position, radius, layerMask);
            if (hitColliders.Length > 0)
            {
                _ListBlock = hitColliders.ToList();
            }
            else
            {
                _targetDigger = null;
            }
        }
        busy = 0;
    }
    IEnumerator ReSort()
    {
        while(true)
        {
            SearchNearestObject();
            yield return new WaitForSeconds(0.5f);
        }
    }
    void SearchNearestObject()
    {
        _ListBlock = _ListBlock.Where(x => x != null).OrderBy(x => Vector2.Distance(transform.position,x.transform.position)).ToList();
    }
    void SearchNearestBase()
    {
        var test = Global.BuildingsCharge.Where(x => x != null).OrderBy(x => Vector2.Distance(transform.position,x.transform.position)).ToList();
        foreach (var item in test)
        {
            if(item.GetComponent<Busy>()._busy == 0)
            {
                _target = item;
                item.GetComponent<Busy>()._busy = 1;
                _StartVector3 = item.transform.position;
                _StartVector3.z = 0;
                break;
            }
        }
    }
    void StartMove()
    {
        busy = 0;
    }
    // void OnTriggerStay2D(Collider2D other)
    // {
    //     if(other.name == "Resource")
    //     {
    //         if(_storageCount < UpdatePlayer.storageCountMax)
    //         {
    //             if(other.tag == "Yellow")
    //             {
    //                 Destroy(other.gameObject);
    //                 _yellow ++;
    //                 GetComponent<AutoMove>()._energy -=5;
    //             }
    //             else if(other.tag == "Red")
    //             {
    //                 Destroy(other.gameObject);
    //                 _red ++;
    //                 GetComponent<AutoMove>()._energy -=5;
    //             }
    //             else if(other.tag == "Blue")
    //             {
    //                 Destroy(other.gameObject);
    //                 _blue ++;
    //                 GetComponent<AutoMove>()._energy -=5;
    //             }
    //         }
    //         _storageCount = _red + _yellow + _blue;
    //     }
    // }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "Block")
        {
            _drill = 1;
        }
    }
}