using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

/**
 * Save humans, destroy zombies!
 */
class Player
{
    private static void Main(string[] args)
    {
        string[] inputs;
        var lastId = -1;

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            var x = int.Parse(inputs[0]);
            var y = int.Parse(inputs[1]);
            var humanCount = int.Parse(Console.ReadLine());
            var humans = new Entity[humanCount];
            for (var i = 0; i < humanCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var humanId = int.Parse(inputs[0]);
                var humanX = int.Parse(inputs[1]);
                var humanY = int.Parse(inputs[2]);
                humans[i] = new Entity(new Vector2(humanX, humanY), new Vector2(0, 0), humanId);
            }

            var zombieCount = int.Parse(Console.ReadLine());
            var zombies = new Entity[zombieCount];
            for (var i = 0; i < zombieCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var zombieId = int.Parse(inputs[0]);
                var zombieX = int.Parse(inputs[1]);
                var zombieY = int.Parse(inputs[2]);
                var zombieXNext = int.Parse(inputs[3]);
                var zombieYNext = int.Parse(inputs[4]);

                var position = new Vector2(zombieX, zombieY);
                var direction = new Vector2(zombieXNext, zombieYNext) - position;
                zombies[i] = new Entity(position, direction, zombieId);
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            var closest = FindNew(humans, zombies, new Vector2(x, y), lastId);
            lastId = closest.Id;
            var targetPoint = TargetZombiePosition(closest);

            Console.WriteLine($"{(int) targetPoint.X} {(int) targetPoint.Y}");
        }
    }

    private static Entity FindNew(Entity[] humans, Entity[] zombies, Vector2 heroPos, int lastId)
    {
        if (lastId != -1)
        {
            var exist = zombies.FirstOrDefault(x => x.Id == lastId);
            if (exist.Id is not 0)
            {
                return exist;
            }
        }

        const int characterSpeed = 1000;
        const int zombieSpeed = 400;

        var closestZombies = new List<ZombieToHuman>();

        foreach (var h in humans)
        {
            var closestToHuman = zombies.OrderBy(x => (h.Position - x.Position).LengthSquared()).First();

            var closestMagnitude = (h.Position - closestToHuman.Position).Length();

            var zombieToHumanTime = closestMagnitude / zombieSpeed;
            
            Console.Error.WriteLine($"HumanId: {h.Id} ZombieId: {closestToHuman.Id} Distance: {closestMagnitude} Time: {zombieToHumanTime}");

            var targetPosition = TargetZombiePosition(closestToHuman);

            var heroToZombieTime = (heroPos - targetPosition).Length() / characterSpeed;

            if (heroToZombieTime - zombieToHumanTime > 1.6f)
            {
                continue;
            }

            closestZombies.Add(
                new ZombieToHuman { zombie = closestToHuman, human = h, timeToHuman = zombieToHumanTime });
        }

        Entity target;
        if (closestZombies.Count == 0)
        {
            target = humans.First();
        }
        else 
        // if (closestZombies.Count(x => x.timeToHuman < 1) < 2)
        {
            target = closestZombies.OrderBy(x => x.timeToHuman).First().zombie;
        }
        // else
        // {
            // target = closestZombies.OrderBy(x => (heroPos - x.zombie.Position).LengthSquared()).First().zombie;
        // }

        return target;
    }

    private static Vector2 TargetZombiePosition(Entity closestToHuman)
    {
        return closestToHuman.Position + closestToHuman.Direction * 1.2f;
    }

    public struct Entity
    {
        public Vector2 Position;
        public Vector2 Direction;
        public int Id;

        public Entity(Vector2 pos, Vector2 dir, int id)
        {
            Position = pos;
            Direction = dir;
            Id = id;
        }
    }

    private record ZombieToHuman
    {
        public Entity zombie;
        public Entity human;
        public float timeToHuman;
    }
}