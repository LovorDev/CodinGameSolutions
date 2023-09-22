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
    private const int TimeHorizontalLimit = 35;
    private const int DistanceStopLimit = 1000;
    private const int MinVelocityToFall = -40;
    private const int RotationTweak = 500;

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

            UpdateRotationMultiplayer(flatSurface, position, marsLandPoints);

            SetPositionAboveLand(flatSurface, position, speed);
        }
    }

    private static void UpdateRotationMultiplayer(FlatSurface flatSurface, Vector2 position,
        List<Vector2> marsLandPoints)
    {
        var allPicks = marsLandPoints.Where(x => x.X > position.X && x.X < flatSurface.Center.X ||
                                                 x.X < position.X && x.X > flatSurface.Center.X)
            .OrderByDescending(x => x.Y).ToList();
        if (!allPicks.Any())
        {
            return;
        }

        _highestOnDirection = allPicks.First();
        _rotationMp = Math.Abs(_highestOnDirection.Y - position.Y) / RotationTweak;
        Console.Error.WriteLine($"RotationMP = {_rotationMp}");
    }

    private static void SetPositionAboveLand(FlatSurface flatSurface, Vector2 position, Vector2 speed)
    {
        var direction = Math.Sign((flatSurface.Center - position).X);

        var rotation = (int)(-10 * direction * _rotationMp);
        var closestFlat = flatSurface.Center;

        var absHorizontalSpeed = Math.Abs(speed.X);
        var horizontalDistanceToFlat = Math.Abs(closestFlat.X - position.X);
        var timeToFlatHorizontal = horizontalDistanceToFlat / absHorizontalSpeed;

        var trust = 4;
        if (absHorizontalSpeed < 30)
        {
            trust = 4;
        }
        else
        {
            trust -= (int)_rotationMp;
            rotation = 0;
        }

        if (timeToFlatHorizontal < TimeHorizontalLimit)
        {
            StopHorizontal(speed, ref rotation, ref trust);
            Console.Error.WriteLine($"Stop Horizontal by speed. Time = {timeToFlatHorizontal}");
        }

        if (horizontalDistanceToFlat < DistanceStopLimit)
        {
            if (absHorizontalSpeed > 20)
            {
                StopHorizontal(speed, ref rotation, ref trust);
                Console.Error.WriteLine(
                    $"Stop Distance={horizontalDistanceToFlat} % = {(position.X - flatSurface.Center.X) / flatSurface.Length} R = {(position.X - flatSurface.Center.X) / flatSurface.Length * -50}");
            }
            else
            {
                rotation = 0;
                trust = 3;
            }
        }

        if (position.X < flatSurface.Left.X || position.X > flatSurface.Right.X)
        {
            var distToHeight = Math.Abs(_highestOnDirection.X - position.X);

            var timeToHill = distToHeight / absHorizontalSpeed;

            Console.Error.WriteLine($"Highest Diff {_highestOnDirection.Y - position.Y}. Time To:  {timeToHill}");

            if (_highestOnDirection.Y - position.Y > -300 && absHorizontalSpeed > 20)
            {
                StopHorizontal(speed, ref rotation, ref trust);
            }
        }

        if (position.Y < 1800)
        {
            rotation /= 2;
        }

        if (speed.Y < MinVelocityToFall)
        {
            trust = 4;
        }

        if (speed.Y > 4)
        {
            trust = 2;
        }

        var aboveFlat = position.X > flatSurface.Left.X && position.X < flatSurface.Right.X;
        
        if (aboveFlat)
        {
            
            var distanceToSurface = position.Y - flatSurface.Center.Y;

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



            
            const float g = -3.711f;
            var landingVelo = CalculateFinalVelocity(speed.Y, g + trust, distanceToSurface);
            trust = landingVelo switch
            {
                > 40 => 4,
                < 40 and > 30 => 3,
                < 30 and > 20 => 2,
                < 20 and > 10 => 1,
                < 10 => 0,
                _ => trust
            };
            
            if (distanceToSurface < 120)
            {
                rotation = 0;
                trust = landingVelo switch
                {
                    > 40-4 => 4,
                    < 40-4 and > 30-3 => 3,
                    < 30-3 and > 20-2 => 2,
                    < 20-2 and > 10-1 => 1,
                    < 10-1 => 0,
                    _ => trust
                };
            }
            
            Console.Error.WriteLine(
                $"Above flat. dist=x:{position.X - flatSurface.Center.X} y:{distanceToSurface}. % = {(position.X - flatSurface.Center.X) / flatSurface.Length}");
        }


        rotation = Math.Clamp(rotation, -90, 90);
        trust = Math.Clamp(trust, 0, 4);

        Console.WriteLine($"{rotation} {trust}");
    }

    public static float CalculateFinalVelocity(float initialVelocity, double g, float s)
    {
        var poweredCalculation = Math.Pow(initialVelocity, 2) + 2 * g * s;
        var velocityOnLand = (float)Math.Sqrt(poweredCalculation);
        Console.Error.WriteLine($"velocityOnLand: {velocityOnLand}");
        return velocityOnLand;
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