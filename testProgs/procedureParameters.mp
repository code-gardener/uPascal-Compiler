program procedureParameters;
	var someInt, someInt2 : integer;
	someFloat, someFloat2 : float;

	procedure printHello(x, y : integer);
	var hi, hey : integer;
	begin 
		writeln(y);
		writeln(x);
	end;

	procedure print;
	begin
		write('Hi');
	end;
begin
	someInt := 1;
	someInt2 := 2;
	printHello(someInt, someInt2);
	print;
end.
