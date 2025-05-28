using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataDeclaration;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShootingRoom : Room
{
    [SerializeField] private Transform[] targetPoints;
    [SerializeField] private Transform[] civilianTargetPoints;
    [SerializeField] private GameObject[] activeWalls;

    private List<BaseTarget> curActiveTargets = new();

    public StageSO Data { get; set; }

    private bool isNextRoomPlaced = false;

    protected override void Awake()
    {
        base.Awake();
        if (endPoint == null)
        {
            endPoint = transform.FindDeepChildByName("EndPoint");
        }

        exitGate.Door.OpenDoor += OpenDoor;
        exitGate.Door.DoorClosed += ResetRoom;
        exitGate.OnPassingGate += ExitRoom;
        enterGate.OnPassingGate += EnterRoom;
    }

    private void OnEnable()
    {
        if (Data != null)
        {
            InitWall();
            InitTarget();
            InitCivilianTarget();
        }
    }

    protected override void OpenDoor()
    {
        if (RoomManager.Instance.CurStageIndex == Constants.LAST_STAGE_INDEX &&
            RoomManager.Instance.CurRoomIndex == Constants.MAX_ROOM_INDEX)
        {
            StageManager.Instance.GameOver();
            AnalyticsManager.Instance.ClearGame();
            return;
        }
        AnalyticsManager.Instance.RoomInitData();
        base.OpenDoor();
        StageManager.Instance.RemainTime += Constants.ADDITIONAL_STAGE_TIME;
    }

    protected override void ExitRoom()
    {
        base.ExitRoom();
        AnalyticsManager.Instance.RoomDataFlush();
    }
    protected override void ResetRoom()
    {
        for (var i = 0; i < activeWalls.Length; i++)
        {
            activeWalls[i].SetActive(false);
        }

        UnregisterAllTargetEvents();

        ReturnTargetToPool();
        isNextRoomPlaced = false;
        StartCoroutine(DisableRoom(1f));
    }

    public override bool CanOpenDoor()
    {
        for (var i = 0; i < curActiveTargets.Count; i++)
        {
            if (curActiveTargets[i].gameObject.activeSelf)
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerator DisableRoom(float time)
    {
        yield return new WaitForSeconds(time);
        enterGate.Door.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void InitWall()
    {
        for (var i = 0; i < activeWalls.Length; i++)
        {
            activeWalls[i].SetActive(Data.WallPoints[i]);
        }
    }

    private void InitTarget()
    {
        var targetIDList = Data.Targets;
        var targetList = new List<TargetSO>();
        for (var i = 0; i < targetIDList.Length; i++)
        {
            var id = targetIDList[i];
            var targetSO = ResourceManager.Instance.Load<TargetSO>($"Data/SO/TargetSO/{id}");
            targetList.Add(targetSO);
        }

        targetList.OrderBy(x => Random.value).ToList();

        for (var i = 0; i < Data.RespawnPoints.Length; i++)
        {
            var respawnIndex = Data.RespawnPoints[i];
            BaseTarget prefab = null;
            switch (targetList[i].Type)
            {
                case (int)TargetType.LandTarget:
                    prefab = Resources.Load<BaseTarget>("Prefabs/Stage/Target/LandTarget");
                    break;
                case (int)TargetType.AerialTarget:
                    prefab = Resources.Load<BaseTarget>("Prefabs/Stage/Target/AerialTarget");
                    break;
            }

            var target = ObjectPoolManager.Instance.GetObject<BaseTarget>(prefab, targetPoints[respawnIndex].position,
                targetPoints[respawnIndex].rotation);
            target.InitData(targetList[i]);
            RegisterTargetEvents(target);
            curActiveTargets.Add(target);
        }
    }

    private void RegisterTargetEvents(BaseTarget target)
    {
        target.OnTargetDisabled += OnTargetDisabled;
    }

    private void UnregisterAllTargetEvents()
    {
        foreach (var target in curActiveTargets)
        {
            target.OnTargetDisabled -= OnTargetDisabled;
        }
    }
    private void OnTargetDisabled(BaseTarget target)
    {
        if (isNextRoomPlaced) return;


        int aliveCount = curActiveTargets
            .Where(t => t != null && t.gameObject != null && t.gameObject.activeSelf)
            .Count();

        if (aliveCount <= 1)
        {
            RoomManager.Instance.PlaceNextRoom();
            isNextRoomPlaced = true;
        }
    }

    private void InitCivilianTarget()
    {
        var civilianRespawnPoints =
            civilianTargetPoints.OrderBy(x => Random.value).Take(Data.CivilianRespawn).ToArray();
        for (var i = 0; i < civilianRespawnPoints.Length; i++)
        {
            civilianRespawnPoints[i].gameObject.SetActive(true);
        }
    }

    private void ReturnTargetToPool()
    {
        for (var i = 0; i < curActiveTargets.Count; i++)
        {
            ObjectPoolManager.Instance.ReturnToPool(curActiveTargets[i].gameObject);
        }

        curActiveTargets.Clear();
    }
}