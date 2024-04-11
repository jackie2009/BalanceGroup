using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BalanceGroupDemo : MonoBehaviour
{
  public static  BalanceGroup balanceGroup;
    // Start is called before the first frame update
    void Start()
    {
        balanceGroup = new BalanceGroup(4);


    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
        var box=new GameObject().AddComponent<DemoBox>();
        balanceGroup.Add(box);
        UpdateAllCubesPos();
        }
    }

    public static void UpdateAllCubesPos()
    {
        foreach (var items in balanceGroup.groups)
        {


            for (int i = 0; i < items.Count; i++)
            {
                UpdateCubePos(items[i].balanceGroupID, items[i] as DemoBox);

            }
        }
    }

    public  static void UpdateCubePos(int groupID, DemoBox box)
    {
        var items = balanceGroup.groups[groupID];
        float y=box.height/2;
        for (int i = 0; i < items.Count; i++) {
            if (items[i] == box) break;
            y += (items[i] as DemoBox).height;
        }
        box.targetPos=new Vector3 (groupID*2-3,y,0);
    }
}


public class DemoBox : MonoBehaviour, IBalanceGroupItem
{
    GameObject box;
    public Vector3 targetPos;
    public float height;
    public int _balanceGroupID;


    public int balanceGroupID { get => _balanceGroupID; set => _balanceGroupID=value; }

    public void Awake( ) {
        height = Random.Range(0.2f, 2f);
        box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.transform.localScale=new Vector3 (1, height, 1);
        box.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
        box.transform.position= new Vector3(0,10,0);
        box.AddComponent<MouseDownCallback>().init(this);
    }
    public void OnMouseDown()
    {
        BalanceGroupDemo.balanceGroup.Remove(this);
        BalanceGroupDemo.UpdateAllCubesPos();
    
        Destroy(box);
        Destroy(this);


    }
    public void Update()
    {
        box.transform.position = Vector3.Lerp(box.transform.position,  targetPos, 0.1f);
    }
    public int getScore()
    {
        return (int)(box.transform.localScale.y * 100);
    }
}

public class MouseDownCallback : MonoBehaviour
{
    MonoBehaviour target;
    internal void init(MonoBehaviour target)
    {
        this.target = target;
    }
    public void OnMouseDown()
    {

        target.SendMessage("OnMouseDown");

    }
}