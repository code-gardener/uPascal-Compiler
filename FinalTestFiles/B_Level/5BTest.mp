program test;
var 	a, startA		:inteGer;
	b1	 		:boolean;

begin

	writeln('This is test 6 of B level');
	writeln('Please enter a positive integer');
	read(a);

	startA := a;

	for a := a downto  0 do
	begin
		write(a, ', ');
	end;

	writeln('\nShould print from ', startA ,' to 0 inclusive');

	writeln('Please enter a negative integer');
	read(a);

	startA := a;

	for a := a to 0 do
	begin
		write(a, ', ');
	end;

	writeln('\nShould print from ', startA , ' to 0 inclusive');


end.
