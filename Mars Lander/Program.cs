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
    public record FlatSurface(Vector2 Left, Vector2 Right, Vector2 Center, float Length)
    {
        public override string ToString()
        {
            return $"{{ Left = {Left}, Right = {Right}, Center = {Center}, Length = {Length} }}";
        }
    }

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

        var flatSurface2 = marsLandPoints.Skip(1)
            .Select((x, i) => new FlatSurface(marsLandPoints[i], x, (marsLandPoints[i] + x) / 2, x.X - marsLandPoints[i].X));
        var flatSurface = flatSurface2
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

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
            SetPositionAboveLand(flatSurface, position, speed);
        }
    }

    private static void SetPositionAboveLand(FlatSurface flatSurface, Vector2 position, Vector2 speed)
    {
        var directionToSurface = -1;


        var direction = (flatSurface.Center - position).X;
        var rotation = -20 * (direction / 4000) - (speed.X / 50);
        Console.Error.WriteLine($"Direction%: {direction / 4000}. speed%{speed.X / 50}");
        
        // if (direction < -flatSurface.Length / 2)
        // {
        //     rotation *= 1;
        // }
        // else if (direction > flatSurface.Length / 2)
        // {
        //     rotation *= -1;
        // }

        int trust = 0;
        if (position.Y < 2500)
        {
            trust = 4;
        }
        else
        {
            trust = 3;
        }

        // R P. R is the desired rotation angle. P is the desired thrust power.
        Console.WriteLine($"{(int)rotation} {trust}");
    }
}