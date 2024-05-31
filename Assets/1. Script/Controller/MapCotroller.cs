using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;
using System;

public class MapCotroller : MonoSingleton<MapCotroller>
{
    public GameObject mapPrant;
    public List<MapSlot> allSlot; //lowerleft sort, ����Ƽ�� ����� ��ǥ����
    List<MapSlot> activeSlots;
    int callIndex = 0; //�θ� ��ġ �ӽ�����

    public const int mapSizeX = 6; //0,1,2,3,4,5 = 6
    public const int mapSizeY = 6; //0,1,2,3,4,5 = 6

    public void Start()
    {
        //����Ʈ �ʱ�ȭ
        allSlot ??= new List<MapSlot>();
        activeSlots ??= new List<MapSlot>();

        for (int i = 0; i < allSlot.Count; ++i) 
        {
            if (allSlot == null) continue;

            allSlot[i].slotindex = i;
            allSlot[i].mapPoint = GetMapPoint(0, i);
        }
    }

    /// <summary>
    /// �� ���Կ��� Ŭ������ �˸�
    /// </summary>
    /// <param name="call_slot"></param>
    public void CallingMapSlot(MapSlot call_slot)
    {
        //���̸� ���� ���� �� �������� �߰����� ����ó�� �ʿ�
        if (call_slot == null){
            return;
        }

        callIndex = (int)(call_slot.mapPoint.x * mapSizeX + call_slot.mapPoint.y);
        OffAllSlot();

        //���� �Ŵ������� �����ϱ�
        GameManager.Instance.Action(callIndex);
    }

    #region slot func

    public enum SlotShape { Target, Cross, Around1, Around2, Around3, All, NotTarget, Random }
    //                      1ĭ     ����   �ֺ�1ĭ  �ֺ�2ĭ  �ֺ�3ĭ  ��ü ��������ü �ƹ���ġ 1��

    /// <summary>
    /// ���� �� ���� ���·� ��ȯ�ϱ�
    /// </summary>
    public void OffAllSlot()
    {
        //Ȱ��ȭ�� ������ �Ǿ����� �ʴٸ� ���� ����
        if (activeSlots == null || activeSlots.Count <= 0)
        {
            for (int i = 0; i < allSlot.Count; ++i)
                allSlot[i].OnOffButton(false);
        }
        else 
        {
            for (int i = 0; i < activeSlots.Count; ++i)
                activeSlots[i].OnOffButton(false);

            activeSlots.Clear();
        }
    }

    /// <summary>
    /// ���� �� �ִ� ���·� ��ȯ�ϱ�
    /// </summary>
    /// <param name="target_point"></param>
    /// <param name="shape"></param>
    public void OnSlots(int index, SlotShape shape = SlotShape.Target) 
    {
        //�ʱ�ȭ �� ����ֱ�
        activeSlots ??= new List<MapSlot>();
        activeSlots.Clear();

        Vector2 target_point = TransMapVector2(index);

        switch (shape)
        {
            case SlotShape.Target:
                MapSlot getslot = ExistTargetSlot(GetMapPoint(target_point.x, target_point.y));

                if (getslot != null)
                {
                    activeSlots.Add(getslot);
                    getslot.OnOffButton(true);
                }
                break;

            case SlotShape.Cross:
                // [-1, 1] [0, 1] [1, 1]
                // [-1, 0] [0, 0] [1, 0]
                // [-1,-1] [0, -1] [1, -1]

                for (int x = -1; x < 2; ++x){
                    for (int y = -1; y < 2; ++y){
                        if (x + y == 0) continue;

                        MapSlot getslot2 = ExistTargetSlot(GetMapPoint(target_point.x + x, target_point.y + y));

                        if (getslot2 == null) continue;

                        activeSlots.Add(getslot2);
                        getslot2.OnOffButton(true);
                    }
                }
                break;

            case SlotShape.Around1:
                // [-1, 1] [0, 1] [1, 1]
                // [-1, 0] [0, 0] [1, 0]
                // [-1,-1] [0, -1] [1, -1]

                for (int x = -1; x < 2; ++x) {
                    for (int y = -1; y < 2; ++y){
                        if (x == 0 && y == 0) continue;
                        MapSlot getslot3 = ExistTargetSlot(new Vector2(target_point.x + x, target_point.y + y));

                        if (getslot3 == null) continue;

                        activeSlots.Add(getslot3);
                        getslot3.OnOffButton(true);
                    }
                }
                break;

            case SlotShape.Around2:
                for (int x = -2; x < 3; ++x){
                    for (int y = -2; y < 3; ++y){
                        if (x == 0 && y == 0) continue;
                        MapSlot getslot4 = ExistTargetSlot(new Vector2(target_point.x + x, target_point.y + y));

                        if (getslot4 == null) continue;

                        activeSlots.Add(getslot4);
                        getslot4.OnOffButton(true);
                    }
                }
                break;
            case SlotShape.Around3:
                for (int x = -3; x < 4; ++x){
                    for (int y = -3; y < 4; ++y){
                        if (x == 0 && y == 0) continue;
                        MapSlot getslot5 = ExistTargetSlot(new Vector2(target_point.x + x, target_point.y + y));

                        if (getslot5 == null) continue;

                        activeSlots.Add(getslot5);
                        getslot5.OnOffButton(true);
                    }
                }
                break;

            case SlotShape.All:
                for (int i = 0; i < allSlot.Count; ++i) 
                {
                    activeSlots.Add(allSlot[i]);
                    allSlot[i].OnOffButton(true);
                }
                break;

            case SlotShape.NotTarget:
                for (int i = 0; i < allSlot.Count; ++i)
                {
                    if (allSlot[i].mapPoint.Equals(target_point)) continue;

                    activeSlots.Add(allSlot[i]);
                    allSlot[i].OnOffButton(true);
                }
                break;

            case SlotShape.Random:
                MapSlot getslot6 = ExistTargetSlot(GetMapPoint(UnityEngine.Random.Range(0, mapSizeX), UnityEngine.Random.Range(0, mapSizeY)));

                if (getslot6 != null)
                {
                    activeSlots.Add(getslot6);
                    getslot6.OnOffButton(true);
                }
                
                break;
        }
    }

    /// <summary>
    /// �ش� ��ġ�� ������ �ִ��� Ȯ��
    /// </summary>
    /// <param name="target_point"></param>
    /// <returns></returns>
    public MapSlot ExistTargetSlot(Vector2 point) 
    {
        if (point.x >= mapSizeX || point.x < 0)
            return null;
        if (point.y >= mapSizeY || point.y < 0)
            return null;

        //�ε����� ���� ���� ã��
        int targetindex = (int)((point.x * mapSizeX) + point.y);
        if (allSlot.Count < targetindex)
            return null;
        if (allSlot[targetindex] == null)
            return null;
        if (allSlot[targetindex].mapPoint.x != point.x || allSlot[targetindex].mapPoint.y != point.y)
            DebugLogger.SendDebug("��ġ�� ������ ���� �ʴ� ������ ȣ��Ǿ����ϴ�. slotpoint:"+ allSlot[targetindex].mapPoint.ToString() + " / callpoint:"+ point.ToString());
        
        return allSlot[targetindex];
    }

    /// <summary>
    /// �� �ε��� ������� ��ġ ��ȯ
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 GetSlotPosition(int index) 
    {
        if (allSlot.Count >= index)
            return Vector3.zero;

        return allSlot[index].transform.position;
    }

    #endregion

    #region static get set trans

    /// <summary>
    /// �� ����Ʈ�� �°� ����
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    static public Vector2 GetMapPoint(float pointx, float pointy)
    {
        while (pointx > mapSizeX)
        {
            pointx -= 1;
            pointy += mapSizeX;
        }

        while (pointx < 0)
        {
            pointx += 1;
            pointy -= mapSizeX;
        }

        while (pointy > mapSizeY)
        {
            pointy -= mapSizeY;
            pointx += 1;
        }

        while (pointy < 0)
        {
            pointy += mapSizeY;
            pointx -= 1;
        }

        return new Vector2(pointx, pointy);
    }

    /// <summary>
    /// ��ȯ int > vec2
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    static public Vector2 TransMapVector2(int index)
    {
        return GetMapPoint(0, index);
    }

    /// <summary>
    /// ��ȯ vec2 > int
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    static public int TransMapIndex(Vector2 point)
    {
        return (int)(point.x * mapSizeX + point.y);
    }

    #endregion
}
