using System;

var inputs = Console.ReadLine().Split(' ');
var exit = int.Parse(inputs[4]); 
var ne = int.Parse(inputs[7]); // number of elevators
var e = new int[ne];
for (var i = 0; i < ne; i++)
{
    var l = Console.ReadLine().Split(' ');
    e[int.Parse(l[0])] = int.Parse(l[1]);
}

var lastPos = 0;
while (true)
{
    inputs = Console.ReadLine().Split(' ');
    var(cf,cp) = (int.Parse(inputs[0]),int.Parse(inputs[1]));
    if (cf == -1)
    {
        Console.WriteLine("WAIT");
        continue;
    }
    var d = cf >= ne ? exit : e[cf];
    
    Console.WriteLine(Math.Abs(cp - d) > Math.Abs(lastPos - d) ? "BLOCK" : "WAIT");
    lastPos = cp;
}