using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;
using System;

public class MapCotroller : MonoSingleton<MapCotroller>
{
    public GameObject mapPrant;
    public List<MapSlot> allSlot; //lowerleft sort, 유니티와 비슷한 좌표구성
    List<MapSlot> activeSlots;
    int callIndex = 0; //부른 위치 임시저장

    public const int mapSizeX = 6; //0,1,2,3,4,5 = 6
    public const int mapSizeY = 6; //0,1,2,3,4,5 = 6

    public void Start()
    {
        //리스트 초기화
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
    /// 맵 슬롯에서 클릭됨을 알림
    /// </summary>
    /// <param name="call_slot"></param>
    public void CallingMapSlot(MapSlot call_slot)
    {
        //널이면 턴이 꼬일 수 있음으로 추가적인 제어처리 필요
        if (call_slot == null){
            return;
        }

        callIndex = (int)(call_slot.mapPoint.x * mapSizeX + call_slot.mapPoint.y);
        OffAllSlot();

        //게임 매니저에게 송출하기
        GameManager.Instance.Action(callIndex);
    }

    #region slot func

    public enum SlotShape { Target, Cross, Around1, Around2, Around3, All, NotTarget, Random }
    //                      1칸     십자   주변1칸  주변2칸  주변3칸  전체 나빼고전체 아무위치 1개

    /// <summary>
    /// 누를 수 없는 상태로 전환하기
    /// </summary>
    public void OffAllSlot()
    {
        //활성화가 별도로 되어있지 않다면 전부 오프
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
    /// 누를 수 있는 상태로 전환하기
    /// </summary>
    /// <param name="target_point"></param>
    /// <param name="shape"></param>
    public void OnSlots(int index, SlotShape shape = SlotShape.Target) 
    {
        //초기화 및 비워주기
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
    /// 해당 위치에 슬롯이 있는지 확인
    /// </summary>
    /// <param name="target_point"></param>
    /// <returns></returns>
    public MapSlot ExistTargetSlot(Vector2 point) 
    {
        if (point.x >= mapSizeX || point.x < 0)
            return null;
        if (point.y >= mapSizeY || point.y < 0)
            return null;

        //인덱스에 따른 슬롯 찾기
        int targetindex = (int)((point.x * mapSizeX) + point.y);
        if (allSlot.Count < targetindex)
            return null;
        if (allSlot[targetindex] == null)
            return null;
        if (allSlot[targetindex].mapPoint.x != point.x || allSlot[targetindex].mapPoint.y != point.y)
            DebugLogger.SendDebug("위치와 순서가 맞지 않는 슬롯이 호출되었습니다. slotpoint:"+ allSlot[targetindex].mapPoint.ToString() + " / callpoint:"+ point.ToString());
        
        return allSlot[targetindex];
    }

    /// <summary>
    /// 맵 인덱스 기반으로 위치 반환
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
    /// 맵 포인트에 맞게 수정
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
    /// 전환 int > vec2
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    static public Vector2 TransMapVector2(int index)
    {
        return GetMapPoint(0, index);
    }

    /// <summary>
    /// 전환 vec2 > int
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    static public int TransMapIndex(Vector2 point)
    {
        return (int)(point.x * mapSizeX + point.y);
    }

    #endregion
}
