using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    public class Stone
    {
        public int Position { get; set; }
        public int AlphabetPosition { get; init; }
        public char Letter { get; init; }

        public override string ToString()
        {
            return $"{nameof(Letter)}: {Letter}, {nameof(Position)}: {Position}, {nameof(AlphabetPosition)}: {AlphabetPosition}";
        }
    }


    public static void Main(string[] args)
    {
        var magicPhrase = args.FirstOrDefault() ?? Console.ReadLine();

        var currentWorldPosition = 0;
        var message = "";
        var stoneList = new List<Stone>();
        var debugMessage = "";
        foreach (var c in magicPhrase)
        {
            var rightStone = stoneList.OrderByDescending(x => x.Position).FirstOrDefault();
            var leftStone = stoneList.OrderBy(x => x.Position).FirstOrDefault();
                
            var closestEmptyStonePosition = rightStone is null ? 0 : 
                Math.Abs(rightStone.Position + 1 - currentWorldPosition) <
                Math.Abs(leftStone.Position - 1 - currentWorldPosition)
                    ? rightStone.Position + 1
                    : leftStone.Position - 1;

            closestEmptyStonePosition = ClampPosition(closestEmptyStonePosition);
            
            // Console.Error.WriteLine($"EmptyPos: {closestEmptyStonePosition}. LeftPosition: {leftStone?.Position}. RightPosition: {rightStone?.Position}");

            var letter = new Stone { Letter = c,Position = closestEmptyStonePosition, AlphabetPosition = c % 32 };
            
            var (usePlus,direction) = ComputeLetter(letter, currentWorldPosition, stoneList);

            // Console.Error.WriteLine($"{letter.Letter}:= Pos: {letter.Position} Cur: {currentWorldPosition}. Dir: {direction}");

            currentWorldPosition = letter.Position;
            switch (direction)
            {
                case 0: break;
                case < 0:
                    message += new string('<', -direction);
                    break;
                case > 0:
                    message += new string('>', direction);
                    break;
            }

            switch (usePlus)
            {
                case 0: break;
                case < 0:
                    message += new string('-', -usePlus);
                    break;
                case > 0:
                    message += new string('+', usePlus);
                    break;
            }


            message += '.';
            // Console.Error.WriteLine(new string('-', 20) + (debugMessage+=letter.Letter));
        }

        Console.Error.WriteLine(magicPhrase);
        Console.WriteLine(message);
    }

    private static (int usePlus, int direction) ComputeLetter(Stone letter, int currentWorldPosition, List<Stone> stoneList)
    {
        var direction = ClampPosition(letter.Position - currentWorldPosition);

        var usePlus = letter.AlphabetPosition > 13 ? letter.AlphabetPosition - 27 : letter.AlphabetPosition;

        if (stoneList.Any())
        {
            var closestExistLetter = ClosestExistLetter(stoneList, letter, currentWorldPosition);

            var index = stoneList.IndexOf(closestExistLetter);

            var tasksSum = MinDistancePosition(closestExistLetter.Position , currentWorldPosition) +
                           MinDistanceAlphabet(closestExistLetter.AlphabetPosition, letter.AlphabetPosition);

            var simpleTasksSum = MathF.Abs(direction) + MathF.Abs(usePlus);

            // Console.Error.WriteLine($"TasksSum: {tasksSum}. Closest: ({closestExistLetter.Letter}), pos: {closestExistLetter.Position}");

            if (tasksSum < simpleTasksSum)
            {
                stoneList[index] = letter;
                stoneList[index].Position = closestExistLetter.Position;
                usePlus = letter.AlphabetPosition - closestExistLetter.AlphabetPosition;
                direction = ClampPosition(closestExistLetter.Position - currentWorldPosition);
            }
            else
            {
                stoneList.Add(letter);
            }
        }
        else
        {
            stoneList.Add(letter);
        }

        return (usePlus,direction);
    }

    private static int ClampPosition(int position)
    {
        return position switch
        {
            > 15 => position - 30,
            < -15 => position + 30,
            var _ => position,
        };
    }
    private static Stone ClosestExistLetter(List<Stone> stoneList, Stone letter, int pos)
    {
        return stoneList.OrderBy(x => MinDistancePosition(x.Position , pos) + MinDistanceAlphabet(x.AlphabetPosition, letter.AlphabetPosition)).First();
    }

    private static int MinDistanceAlphabet(int one, int two)
    {
        return CalculateMinDistance(one, two, 0, 27);
    }
    private static int MinDistancePosition(int one, int two)
    {
        return CalculateMinDistance(one, two, -15,15);
    }
    
    public static int CalculateMinDistance(int num1, int num2, int minValue, int maxValue)
    {
        // Нормализация чисел в заданный диапазон
        num1 = Math.Max(minValue, Math.Min(num1, maxValue));
        num2 = Math.Max(minValue, Math.Min(num2, maxValue));

        // Найти абсолютную разницу между числами
        int absDifference = Math.Abs(num1 - num2);

        // Разница между maxValue и minValue, учитывая циклическое переполнение
        int cyclicDifference = Math.Min(num1, num2) + (maxValue - minValue + 1 - Math.Max(num1, num2));

        // Минимальная дистанция будет минимумом из абсолютной разницы и циклической разницы
        int minDistance = Math.Min(absDifference, cyclicDifference);

        return minDistance;
    }
}