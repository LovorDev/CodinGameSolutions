using System;

/**
 * Don't let the machines win. You are humanity's last hope...
 */
class Player
{
    private static void Main(string[] args)
    {
        var width = int.Parse(Console.ReadLine());  // the number of cells on the X axis
        var height = int.Parse(Console.ReadLine()); // the number of cells on the Y axis

        var matrix = new bool[height][];
        for (var i = 0; i < height; i++)
        {
            var line = Console.ReadLine(); // width characters, each either 0 or .
            matrix[i] = new bool[width];
            for (var i1 = 0; i1 < width; i1++)
            {
                var c = line[i1];
                matrix[i][i1] = c == '0';
            }

            Console.Error.WriteLine($"{line}");
        }


        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var target = matrix[y][x];

                if (!target)
                {
                    continue;
                }

                var right = "-1 -1";
                for (var xCheck = x + 1; xCheck < width; xCheck++)
                {
                    if (matrix[y][xCheck])
                    {
                        right = $"{xCheck} {y}";
                        break;
                    }
                }

                var bottom = "-1 -1";
                for (var yCheck = y + 1; yCheck < height; yCheck++)
                {
                    if (matrix[yCheck][x])
                    {
                        bottom = $"{x} {yCheck}";
                        break;
                    }
                }

                Console.WriteLine($"{x} {y} {right} {bottom}");
            }
        }
    }
}