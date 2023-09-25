using System;

var r = P();

var (x,n) = (I(4),I(7));

var e = new int[n+1];
e[n] = x;
for (var i = 0; i < n; i++)
{
    P();
    e[I(0)] = I(1);
}

for(;;)
{
    P();
    var (f,p, d) = (I(0),I(1), r[2][0]=='R'?1:-1);
    Console.WriteLine((p-e[f==-1?n:f])*d<=0&f>=0? "WAIT" : "BLOCK");
}

string[]P() => r = Console.ReadLine().Split(' ');
int I(int i) => int.Parse(r[i]);