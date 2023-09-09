using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

class Player
{
    public enum Owner
    {
        NoStructure = -1,
        Friendly = 0,
        Enemy = 1,
    }

    public enum SpawnSide
    {
        Left = 0,
        Right = 1,
    }

    public enum StructureType
    {
        NoStructure = -1,
        GoldMine = 0,
        Tower = 1,
        Barracks = 2,
    }

    // ReSharper disable once InconsistentNaming
    public enum UnitType
    {
        QUEEN = -1,
        KNIGHT = 0,
        ARCHER = 1,
        GIANT = 2,
    }

    public static int[] GoldCostProgression = { 80, 180, 420, 420 };

    private static void Main(string[] args)
    {
        string[] inputs;
        var numSites = int.Parse(Console.ReadLine());
        var sites = new Site[numSites];

        var actionsQueue = new Queue<IQueenAction>();
        IQueenAction currentAction = null;

        for (var i = 0; i < numSites; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            var siteId = int.Parse(inputs[0]);
            var x = int.Parse(inputs[1]);
            var y = int.Parse(inputs[2]);
            var radius = int.Parse(inputs[3]);
            sites[i] = new Site(siteId, new Vector2(x, y), radius);
        }

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            var gold = int.Parse(inputs[0]);
            var touchedSite = int.Parse(inputs[1]); // -1 if none
            for (var i = 0; i < numSites; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var siteId = int.Parse(inputs[0]);
                var mineGold = int.Parse(inputs[1]);      // used in future leagues
                var mineAmount = int.Parse(inputs[2]);    // used in future leagues
                var structureType = int.Parse(inputs[3]); // -1 = No structure, 2 = Barracks
                var owner = int.Parse(inputs[4]);         // -1 = No structure, 0 = Friendly, 1 = Enemy
                var param1 = int.Parse(inputs[5]);
                var param2 = int.Parse(inputs[6]);
                sites[i].SetOther(siteId, mineGold, mineAmount, structureType, owner, param1, param2);
            }

            var numUnits = int.Parse(Console.ReadLine());
            var units = new Unit[numUnits];

            for (var i = 0; i < numUnits; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var x = int.Parse(inputs[0]);
                var y = int.Parse(inputs[1]);
                var owner = int.Parse(inputs[2]);
                var unitType = int.Parse(inputs[3]); // -1 = QUEEN, 0 = KNIGHT, 1 = ARCHER
                var health = int.Parse(inputs[4]);
                units[i] = new Unit(new Vector2(x, y), owner, unitType, health);
            }


            var defaultInput = new DefaultInput
                { Gold = gold, TouchSite = touchedSite, Sites = sites, Units = units };

            if (!actionsQueue.Any())
            {
                actionsQueue = InitQueue(defaultInput);
                currentAction = actionsQueue.Dequeue();
            }

            currentAction.Proceed(defaultInput);

            var queenAction = currentAction.GetAction();

            while (queenAction is null)
            {
                currentAction = actionsQueue.Dequeue();
                currentAction.Proceed(defaultInput);
                queenAction = currentAction.GetAction();
            }

            var ourBarracksIds = sites
                .Where(x => x.StructureType == StructureType.Barracks && x.Owner == Owner.Friendly).Select(x => x.Id)
                .ToList();

            var trainAction = ourBarracksIds.Count switch
            {
                0 => "TRAIN",
                > 0 => $"TRAIN {string.Join(' ', ourBarracksIds)}",
                _ => null,
            };

            Console.WriteLine(queenAction);
            Console.WriteLine(trainAction);
        }
    }

    private static Queue<IQueenAction> InitQueue(DefaultInput defaultInput)
    {
        var queen = defaultInput.Units.First(x => x is { UnitType: UnitType.QUEEN, Owner: Owner.Friendly });

        var spawnSide = queen.Position.X > 1000 ? SpawnSide.Right : SpawnSide.Left;

        var initPoint = spawnSide switch
        {
            SpawnSide.Right => new Vector2(1920, 1000), SpawnSide.Left => new Vector2(0, 0),
        };

        var closest = defaultInput.Sites.OrderBy(x => Vector2.Distance(x.Position, initPoint)).ToList();

        Console.Error.WriteLine($"Closest: {string.Join(",", closest.Select(x=>x.Id))}");
        var goldMines = closest.Take(5)
            .OrderByDescending(x => x.GoldAmount)
            .Take(3)
            .OrderBy(x => Math.Abs(x.Position.Y - initPoint.Y))
            .ToList();

        Console.Error.WriteLine($"goldMines: {string.Join(",", goldMines.Select(x=>x.Id))}");
        var forStructures = closest.Except(goldMines).Take(2).ToList();
        Console.Error.WriteLine($"forStructures: {string.Join(",", forStructures.Select(x=>x.Id))}");
        var closestPlaces = closest.Except(forStructures).Except(goldMines).ToList();
        Console.Error.WriteLine($"closestPlaces: {string.Join(",", closestPlaces.Select(x=>x.Id))}");

        var queue = new Queue<IQueenAction>();

        BuildMine(defaultInput, queue, goldMines);
        BuildBarack(defaultInput, queue, forStructures, UnitType.KNIGHT);
        BuildMine(defaultInput, queue, goldMines);
        BuildTower(queue, closestPlaces);
        BuildTower(queue, closestPlaces);
        BuildTower(queue, closestPlaces);
        BuildBarack(defaultInput, queue, forStructures, UnitType.KNIGHT);
        BuildMine(defaultInput, queue, goldMines);
        // BuildBarack(defaultInput, queue, forStructures, UnitType.GIANT);
        BuildTower(queue, closestPlaces);
        BuildTower(queue, closestPlaces);

        queue.Enqueue(new Wait(2));
        return queue;
    }

    private static void BuildTower(Queue<IQueenAction> queue, List<Site> closestPlaces)
    {
        // queue.Enqueue(new Move(-1, closestPlaces.First()));
        queue.Enqueue(new Build(closestPlaces.First(), UnitType.QUEEN, StructureType.Tower));
        closestPlaces.Remove(closestPlaces.First());
    }

    private static void BuildMine(DefaultInput defaultInput, Queue<IQueenAction> queue, List<Site> goldMines)
    {
        var firstPlace = goldMines.First();

        // queue.Enqueue(new Move(defaultInput.TouchSite, firstPlace));
        Console.Error.WriteLine($"GoldAmount: ({firstPlace.Id}) {firstPlace.GoldAmount}");
        for (var i = 0; i < firstPlace.GoldAmount; i++)
        {
            queue.Enqueue(new Build(firstPlace, UnitType.QUEEN, StructureType.GoldMine));
        }

        goldMines.Remove(firstPlace);
    }

    private static void BuildBarack(DefaultInput defaultInput, Queue<IQueenAction> queue, List<Site> forStructures,
        UnitType type)
    {
        // queue.Enqueue(new Move(defaultInput.TouchSite, forStructures.First()));
        queue.Enqueue(new Build(forStructures.First(), type, StructureType.Barracks));
        forStructures.Remove(forStructures.First());
    }

    public class Site
    {
        public int Id { get; private set; }
        public int Gold { get; private set; }
        public int GoldAmount { get; private set; }
        public Vector2 Position { get; private set; }
        public int Radius { get; private set; }
        public StructureType StructureType { get; private set; }
        public Owner Owner { get; private set; }
        public int BuildParameter { get; private set; }
        public UnitType BarackType { get; private set; }

        public Site(int siteId, Vector2 position, int radius)
        {
            Id = siteId;
            Position = position;
            Radius = radius;
        }

        public void SetOther(int siteId, int gold, int goldAmount, int structureType, int owner, int timeToUnit,
            int barackType)
        {
            Id = siteId;
            Gold = gold;
            GoldAmount = goldAmount;
            StructureType = (StructureType)structureType;
            Owner = (Owner)owner;
            BuildParameter = timeToUnit;
            BarackType = (UnitType)barackType;
        }

        public override string ToString()
        {
            return
                $"{nameof(Id)}: {Id}, {nameof(Position)}: {Position}, {nameof(Radius)}: {Radius}, {nameof(StructureType)}: {StructureType}, {nameof(Owner)}: {Owner}, {nameof(BuildParameter)}: {BuildParameter}, {nameof(BarackType)}: {BarackType}";
        }
    }

    public struct Unit
    {
        public Vector2 Position { get; }
        public Owner Owner { get; }
        public UnitType UnitType { get; }
        public int Health { get; private set; }

        public Unit(Vector2 position, int owner, int unitType, int health)
        {
            Position = position;
            Owner = (Owner)owner;
            UnitType = (UnitType)unitType;
            Health = health;
        }
    }

    public struct DefaultInput
    {
        public int Gold;
        public int TouchSite;
        public Unit[] Units;
        public Site[] Sites;
    }

    public interface IQueenAction
    {
        public string GetAction();
        public void Proceed(DefaultInput defaultInput);
    }

    public class Move : IQueenAction
    {
        private readonly Site _closestPlace;
        private int _currentTouch;
        private Vector2 _selfPosition;

        public Move(int currentTouch, Site closestPlace)
        {
            _currentTouch = currentTouch;
            _closestPlace = closestPlace;
        }

        public Move(Vector2 targetPosition)
        {
            _closestPlace = new Site(-100, targetPosition, -1);
        }

        public string GetAction()
        {
            return _currentTouch == _closestPlace.Id || Vector2.Distance(_closestPlace.Position, _selfPosition) < 50
                ? null
                : $"MOVE {_closestPlace.Position.X} {_closestPlace.Position.Y}";
        }

        public void Proceed(DefaultInput defaultInput)
        {
            _currentTouch = defaultInput.TouchSite;
            _selfPosition = defaultInput.Units.First(x => x.UnitType == UnitType.QUEEN && x.Owner == Owner.Friendly)
                .Position;
        }
    }

    public class Wait : IQueenAction
    {
        private int _time;

        public Wait(int time)
        {
            _time = time;
        }

        public string GetAction()
        {
            Console.Error.WriteLine($"Time: {_time} == {_time > 0}");
            return _time > 0 ? "WAIT" : null;
        }

        public void Proceed(DefaultInput defaultInput)
        {
            _time--;
        }
    }

    public class Build : IQueenAction
    {
        private readonly UnitType _buildType;
        private Site _targetSite;
        private StructureType _structureType;
        private bool _builded;

        public Build(Site targetSite, UnitType buildType, StructureType structureType)
        {
            _targetSite = targetSite;
            _buildType = buildType;
            _structureType = structureType;
            _builded = false;
        }

        public string GetAction()
        {
            var structureWord = _structureType switch
            {
                StructureType.NoStructure => "expression",
                StructureType.Tower => "TOWER",
                StructureType.GoldMine => "MINE",
                StructureType.Barracks => $"BARRACKS-{_buildType.ToString()}",
                _ => throw new ArgumentOutOfRangeException(),
            };

            if (_targetSite.StructureType == StructureType.GoldMine)
            {
                if (_targetSite.BuildParameter >= _targetSite.GoldAmount)
                {
                    Console.Error.WriteLine($"Mine Complete");
                    return null;
                }
            }
            else if (_targetSite.StructureType == _structureType)
            {
                Console.Error.WriteLine($"Already built {_targetSite.Id} {_structureType}");
                return null;
            }

            _builded = true;

            return $"BUILD {_targetSite.Id} {structureWord}";
        }

        public void Proceed(DefaultInput defaultInput)
        {
            _targetSite = defaultInput.Sites.First(x => x.Id == _targetSite.Id);
            var selfPosition = defaultInput.Units.First(x => x is { UnitType: UnitType.QUEEN, Owner: Owner.Friendly })
                .Position;
            var closestEnemy = defaultInput.Units.Count(x => Vector2.Distance(selfPosition, x.Position) < 100) > 2;

            var selfStruct = defaultInput.Sites.Where(x => x.Owner == Owner.Friendly).ToList();
            var selfRrakcs = selfStruct.Where(x => x.StructureType == StructureType.Barracks).ToList();
            var selfMines = selfStruct.Where(x => x.StructureType == StructureType.GoldMine).ToList();
            var selfTowers = selfStruct.Where(x => x.StructureType == StructureType.Tower).ToList();
            
            if (_structureType == StructureType.Barracks && selfRrakcs.Count >= 2)
            {
                if (selfTowers.Count < 2)
                {
                    _structureType = StructureType.Tower;
                    return;
                }

                if (selfMines.Count < 3)
                {
                    _structureType = StructureType.GoldMine;
                    return;   
                }
                _structureType = StructureType.Tower;
            }
            
            if (_structureType == StructureType.GoldMine && (_targetSite.Gold == 0 || closestEnemy))
            {
                Console.Error.WriteLine($"Swap To Tower {_targetSite.Id}");
                _structureType = StructureType.Tower;
            }
        }
    }
}