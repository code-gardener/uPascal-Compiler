{
20
20
7
5
7
}

program test;

var x:integer; y:integer;

procedure one(var a:integer);
 begin
 a := a+10;
 write(a);
{writeln;}
 end;

procedure two(b:integer);
 begin
 b := b+5;
 one(b);
 write(b);
{writeln;}
 end;

begin

  x := 5;
  y := -3;

  two(x);
  one(y);

  write(x);
{writeln;}
  write(y);
{writeln;}

end.
