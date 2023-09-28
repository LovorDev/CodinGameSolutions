using System;

var r=P();
var n= I(7) + 1;
var e= new int[n + 1];
for (e[n] = I(4); n-- > 1; e[I(0) + 1] = I(1))P();

for (;;Console.WriteLine((I(1) - e[I(0) + 1]) * (r[2][0] - 80) <= 0 ? "WAIT" : "BLOCK")) P();

string[] P() => r = Console.ReadLine().Split(' ');
int I(int i) => int.Parse(r[i]);