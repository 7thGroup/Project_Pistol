using System;
using System.Collections.Generic;
using System.Linq;
using DataDeclaration;
using Random = UnityEngine.Random;

public class RoomManager : SingletonBehaviour<RoomManager>
{
    private Dictionary<int, List<StageSO>> allStageDataDict; // 모든 스테이지 정보 저장(Key: 스테이지 index, Value: 3개의 방 정보)

    private StandbyRoom standbyRoom;
    private Dictionary<string, ShootingRoom> shootingRoomDict;

    public int CurStageIndex { get; private set; }
    public int CurRoomIndex { get; private set; }

    public Room PrevRoom { get; set; }
    public Room CurRoom { get; set; }
    public Room NextRoom { get; set; }

    public Action RoomChangedAction;

    public int TotlaStageCount => allStageDataDict.Keys.Max();

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        InitStageInfo();
        InitRoomPrefabs();

        RoomChangedAction += UpdateStageIndex;
    }

    public void InitRoom()
    {
        var startRoom = ResourceManager.Instance.Load<StartRoom>("Prefabs/Stage/Room/StartRoom");
        CurRoom = Instantiate(startRoom);
    }

    public void PlaceNextRoom()
    {
        if (CurRoomIndex == 3)
        {
            NextRoom = standbyRoom;
        }
        else
        {
            var key = allStageDataDict[CurStageIndex][CurRoomIndex].BaseRoom;
            NextRoom = shootingRoomDict[key];
            ((ShootingRoom)NextRoom).Data = allStageDataDict[CurStageIndex][CurRoomIndex];
        }

        NextRoom.transform.rotation = CurRoom.EndPoint.rotation;
        NextRoom.transform.position = CurRoom.EndPoint.position;
        NextRoom.gameObject.SetActive(true);
    }

    private void UpdateStageIndex()
    {
        if (CurRoomIndex == Constants.MAX_ROOM_INDEX)
        {
            CurRoomIndex = 0;
            CurStageIndex++;
        }
        else
        {
            CurRoomIndex++;
        }
    }

    private void InitStageInfo()
    {
        var stageDataList = ResourceManager.Instance.LoadAll<StageSO>("Data/SO/StageSO");
        allStageDataDict = new Dictionary<int, List<StageSO>>();

        foreach (var stageData in stageDataList)
        {
            if (!allStageDataDict.ContainsKey(stageData.StageIndex))
            {
                var stageList = new List<StageSO>();
                allStageDataDict.Add(stageData.StageIndex, stageList);
            }

            allStageDataDict[stageData.StageIndex].Add(stageData);
        }

        foreach (var key in allStageDataDict.Keys.ToList())
        {
            var fullList = allStageDataDict[key];
            var random3Rooms = fullList.OrderBy(x => Random.value).Take(3).ToList();
            allStageDataDict[key] = random3Rooms;
        }

        CurStageIndex = 1;
        CurRoomIndex = 0;
    }

    private void InitRoomPrefabs()
    {
        var standbyRoomPrefab = ResourceManager.Instance.Load<StandbyRoom>("Prefabs/Stage/Room/StandbyRoom");
        standbyRoom = Instantiate(standbyRoomPrefab);
        standbyRoom.gameObject.SetActive(false);

        var roomPrefabs = ResourceManager.Instance.LoadAll<ShootingRoom>("Prefabs/Stage/Room");
        shootingRoomDict = new Dictionary<string, ShootingRoom>();
        foreach (var room in roomPrefabs)
        {
            var shootingRoom = Instantiate(room);
            shootingRoom.gameObject.SetActive(false);
            shootingRoomDict.Add(room.name, shootingRoom);
        }
    }
}