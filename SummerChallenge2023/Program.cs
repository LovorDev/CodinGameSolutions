using System;
using System.Collections.Generic;

namespace SummerChallenge2023
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<List<int>>()
            {
                new() { 9, 7 },
                new() { 6, 0 },
                new() { 5, 6 },
                new() { 9, 0 },
                new() { 2, 8 },
                new() { 4, 5 },
                new() { 6, 8 },
                new() { 3, 6 },
                new() { 9, 1 },
            };
            var invalid = new List<List<int>>
            {
                new() { 2, 1 },
                new() { 0, 2 },
                new() { 0, 1 },
            };
            Console.WriteLine(string.Join(',',ExploitingSuperpowers_5.GearBalance(10, list)));
        }
        
        
    
    }
}