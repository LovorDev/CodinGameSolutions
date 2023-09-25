
using System;

var inp = P();

var (ex,ne) = (Ip(4),Ip(7));

var e = new int[ne];
for (var i = 0; i < ne; i++)
{
    P();
    e[Ip(0)] = Ip(1);
}

var lp = 0;
while (true)
{
    P();
    var(cf,cp)=(Ip(0),Ip(1));
    var sk=cf==-1;
    var d=cf>=ne|sk?ex:e[cf];
    Console.WriteLine(sk|Math.Abs(cp - d) <= Math.Abs(lp - d) ? "WAIT" : "BLOCK");
    lp = sk?lp:cp;
}

string[] P() => inp = Console.ReadLine().Split(' ');
int Ip(int i) => int.Parse(inp[i]);