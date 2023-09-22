using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

/**
 * Save the Planet.
 * Use less Fossil Fuel.
 */
class Player
{
    private static float _rotationMp = -1;
    private static Vector2 _highestOnDirection;

    private static void Main(string[] args)
    {
        string[] inputs;
        var N = int.Parse(Console.ReadLine()); // the number of points used to draw the surface of Mars.

        var marsLandPoints = new List<Vector2>();
        for (var i = 0; i < N; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            var landX = int.Parse(inputs[0]);
            var landY = int.Parse(inputs[1]);
            marsLandPoints.Add(new Vector2(landX, landY));
        }

        var flatSurface = marsLandPoints.Skip(1)
            .Select((x, i) =>
                new FlatSurface(marsLandPoints[i], x, (marsLandPoints[i] + x) / 2, x.X - marsLandPoints[i].X))
            .FirstOrDefault(x => x.Left.Y == x.Right.Y);


        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            var position = new Vector2(int.Parse(inputs[0]), int.Parse(inputs[1]));
            var speed = new Vector2(int.Parse(inputs[2]), int.Parse(inputs[3]));

            var F = int.Parse(inputs[4]); // the quantity of remaining fuel in liters.
            var R = int.Parse(inputs[5]); // the rotation angle in degrees (-90 to 90).
            var P = int.Parse(inputs[6]); // the thrust power (0 to 4).


            if (_rotationMp == -1)
            {
                var direction = Math.Sign((flatSurface.Center - position).X);
                _highestOnDirection = marsLandPoints.Where(x => position.X - x.X > direction)
                    .OrderByDescending(x => x.Y).First();

                _rotationMp = Math.Abs(_highestOnDirection.Y - position.Y) / 1000;
                Console.Error.WriteLine($"RotationMP = {_rotationMp}");
            }

            SetPositionAboveLand(flatSurface, position, speed, marsLandPoints);
        }
    }

    private static void SetPositionAboveLand(FlatSurface flatSurface, Vector2 position, Vector2 speed,
        List<Vector2> marsLandPoints)
    {
        var direction = Math.Sign((flatSurface.Center - position).X);

        var rotation = (int)(-10 * direction * _rotationMp);

        var trust = 4;

        if (Math.Abs(speed.X) < 30)
        {
            trust = 4;
        }
        else
        {
            trust -= (int)_rotationMp;
            rotation = 0;
        }

        var closestFlat = flatSurface.Center;

        if (Math.Abs(closestFlat.X - position.X) < 4000)
        {
            if (Math.Abs(speed.X) > 50)
            {
                StopHorizontal(speed, ref rotation, ref trust);
                Console.Error.WriteLine("Stop Horizontal by speed");
            }
        }

        if (Math.Abs(closestFlat.X - position.X) < 1000)
        {
            if (Math.Abs(speed.X) > 20)
            {
                StopHorizontal(speed, ref rotation, ref trust);
                Console.Error.WriteLine(
                    $"Stop Distance={Math.Abs(closestFlat.X - position.X)} % = {(position.X - flatSurface.Center.X) / flatSurface.Length} R = {(position.X - flatSurface.Center.X) / flatSurface.Length * -50}");
            }
            else
            {
                rotation = 0;
                trust = 2;
            }
        }

        _highestOnDirection =
            marsLandPoints.Where(x => position.X - x.X > direction).OrderByDescending(x => x.Y).First();

        if (position.X < flatSurface.Left.X || position.X > flatSurface.Right.X)
        {
            var distToHeight = Math.Abs(_highestOnDirection.X - position.X);

            var timeToHill = distToHeight / Math.Abs(speed.X);

            Console.Error.WriteLine($"Highest Diff {_highestOnDirection.Y - position.Y}. Time To:  {timeToHill}");

            if (_highestOnDirection.Y - position.Y > -300 && Math.Abs(speed.X) > 20)
            {
                StopHorizontal(speed, ref rotation, ref trust);
            }
        }

        if (position.Y < 1800)
        {
            rotation /= 2;
        }

        if (speed.Y < -30)
        {
            trust = 4;
        }

        if (speed.Y > 4)
        {
            trust = 2;
        }

        if (position.X > flatSurface.Left.X && position.X < flatSurface.Right.X)
        {
            Console.Error.WriteLine(
                $"Above flat. dist={position.X - flatSurface.Center.X}. % = {(position.X - flatSurface.Center.X) / flatSurface.Length}");

            if (direction == 1)
            {
                if (speed.X < 0)
                {
                    rotation -= 20;
                }
            }

            if (direction == -1)
            {
                if (speed.X > 0)
                {
                    rotation += 20;
                }
            }

            if (position.Y - flatSurface.Center.Y < 100)
            {
                rotation = 0;
            }
        }


        Console.WriteLine($"{rotation} {trust}");
    }

    private static void StopHorizontal(Vector2 speed, ref int rotation, ref int trust)
    {
        var clamp = 20 * _rotationMp;
        rotation = (int)Math.Clamp(speed.X, -clamp, clamp);
        trust = 4;
    }

    public record FlatSurface(Vector2 Left, Vector2 Right, Vector2 Center, float Length)
    {
        public override string ToString()
        {
            return $"{{ Left = {Left}, Right = {Right}, Center = {Center}, Length = {Length} }}";
        }
    }
}