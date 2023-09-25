using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int nbFloors = int.Parse(inputs[0]); // number of floors
        int width = int.Parse(inputs[1]); // width of the area
        int nbRounds = int.Parse(inputs[2]); // maximum number of rounds
        int exitFloor = int.Parse(inputs[3]); // floor on which the exit is found
        int exitPos = int.Parse(inputs[4]); // position of the exit on its floor
        int nbTotalClones = int.Parse(inputs[5]); // number of generated clones
        int nbAdditionalElevators = int.Parse(inputs[6]); // ignore (always zero)
        int nbElevators = int.Parse(inputs[7]); // number of elevators
        var elevators = new(int f,int p)[nbElevators];
        for (int i = 0; i < nbElevators; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            elevators[i] = (int.Parse(inputs[0]),int.Parse(inputs[1])); 
            
            Console.Error.WriteLine("Elevators: " + elevators[i]);
            // floor on which this elevator is found
            // position of the elevator on its floor
        }

        // game loop
        var lastFlour = 0;
        var target = 0;
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int cloneFloor = int.Parse(inputs[0]); // floor of the leading clone
            int clonePos = int.Parse(inputs[1]); // position of the leading clone on its floor
            int direction = inputs[2] == "RIGHT" ? 1 : -1; // direction of the leading clone: LEFT or RIGHT
            
            if(clonePos == -1){
                
                Console.WriteLine("WAIT");
                continue;
            }

            if(target == 0 || lastFlour !=cloneFloor){
                var targetPos = cloneFloor == exitFloor ? exitPos: elevators.First(x=>x.f == cloneFloor).p;
                target = targetPos > clonePos ? 1 : -1;
            }          
            
            Console.Error.WriteLine($"clonePos:{clonePos} flour = {cloneFloor}. Exit: {exitFloor},{{pos}}:{exitPos}  dir to exit = {target}");
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");


            if (direction == target || lastFlour !=cloneFloor)
            {
                Console.WriteLine("WAIT");
            }
            else
            {
                Console.WriteLine("BLOCK");
                target = 0;
            } 
            lastFlour = cloneFloor;
        }
    }
}