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
    //����item �ҵ���С��group��� ������������group������ �ǳ��Դ��Լ���������һ��item ����Сgroup
    public void Add(IBalanceGroupItem item) {
      
        getMinAndMaxGroup(out int minGroupIndex, out int maxGroupIndex, out int diffScore);
        groups[minGroupIndex].Add(item);
         item.balanceGroupID = minGroupIndex;
        tryMoveMaxGroupItemToMinGroup(item.balanceGroupID,false,true);

    }
    //ɾ��item  ���ɾ�������group�����С �ǳ��Դ����group����һ��item ����
    public void Remove(IBalanceGroupItem item)
    {

    
        groups[item.balanceGroupID].Remove(item);

        tryMoveMaxGroupItemToMinGroup(item.balanceGroupID,true,false);
 
    }
    //���Դ���С��group���ҳ�һ������ʵ�item ʹ���ƶ������group��ʱ 2��group�����С(��ƽ��)
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

    //�ҳ� ������ ���group-��Сgroup ��ֵһ��� item.��Ϊ�ƶ�����ʹ 2��group ������ӽ�
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

 // ͬʱ����������Сֵ ��sortҪ��Ч,������Ҫͬʱ�õ�2�� ��дһ���� ���ֻ�õ����ֿ����ܸ�һ���
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
