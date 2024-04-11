using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public interface IBalanceGroupItem {
    public int getScore();
    public int balanceGroupID { set; get; }
}
public class BalanceGroup 
{
   public List<IBalanceGroupItem> []groups;
 

    public BalanceGroup(int groupCount) {
        groups = new List<IBalanceGroupItem>[groupCount];
        for (int i = 0; i < groupCount; i++) { groups[i] = new List<IBalanceGroupItem>(); }
        
    }
    //新增item 找到最小的group存放 如果新增后这个group变成最大 那尝试从自己这里里找一个item 给最小group
    public void Add(IBalanceGroupItem item) {
      
        getMinAndMaxGroup(out int minGroupIndex, out int maxGroupIndex, out int diffScore);
        groups[minGroupIndex].Add(item);
         item.balanceGroupID = minGroupIndex;
        tryMoveMaxGroupItemToMinGroup(item.balanceGroupID,false,true);

    }
    //删除item  如果删除后这个group变成最小 那尝试从最大group里找一个item 给他
    public void Remove(IBalanceGroupItem item)
    {

    
        groups[item.balanceGroupID].Remove(item);

        tryMoveMaxGroupItemToMinGroup(item.balanceGroupID,true,false);
 
    }
    //尝试从最小的group里找出一个最合适的item 使他移动到最大group里时 2个group差距最小(最平衡)
    private void tryMoveMaxGroupItemToMinGroup(int groupID,bool doOnlyIsMinGroup, bool doOnlyIsMaxGroup)
    {
         getMinAndMaxGroup(out int minGroupIndex, out int maxGroupIndex,out int diffScore);
        if (doOnlyIsMinGroup)
        {
            if (groupID != minGroupIndex) return;
        }
        if (doOnlyIsMaxGroup)
        {
            if (groupID != maxGroupIndex) return;
 
        }
      
        var minDiffItemIndex = getMinDiffItemIndex(maxGroupIndex,diffScore);
   
        var tryMoveItem = groups[maxGroupIndex][minDiffItemIndex];
        if (tryMoveItem.getScore() < diffScore) {
          
            groups[maxGroupIndex].RemoveAt(minDiffItemIndex);
            groups[minGroupIndex].Add(tryMoveItem);
            tryMoveItem.balanceGroupID = minGroupIndex;

        }
    }

    //找出 分数与 最大group-最小group 差值一半的 item.因为移动他会使 2个group 分数最接近
    private int getMinDiffItemIndex(int groupIndex, int diffScore)
    {
        var minDiffItemIndex = -1;


        int minDiffScore = int.MaxValue;
        var items = groups[groupIndex];
        for (int i = 0; i < items.Count; i++)
        {
            var d = Mathf.Abs(items[i].getScore() - diffScore/2);
            if (d< minDiffScore)
            {
                minDiffScore = d;
                minDiffItemIndex = i;
            }

        }
        return minDiffItemIndex;
    }

 // 同时计算出最大最小值 比sort要高效,常常需要同时用到2个 就写一起了 如果只用单个分开性能高一点点
    private void getMinAndMaxGroup(out int minGroupIndex, out int maxGroupIndex,out int diffScore)
    {
        minGroupIndex = -1;
        maxGroupIndex = -1;
        int maxScore= int.MinValue;
        int minScore = int.MaxValue;  
        for (int i = 0;i < groups.Length;i++)
        {
            int score = 0;
            foreach (var item in groups[i])
            {
                score += item.getScore();
            }
            if (score > maxScore) {
                maxScore = score;
                maxGroupIndex = i ;
            }
            if (score < minScore)
            {
                minScore = score;
                minGroupIndex=i;
            }
        }
        diffScore=maxScore-minScore;
    }
}
