BR L0
L0:
PUSH D0
ADD SP #6 SP
SUB SP #7 D0
PUSH #"This is test 7 of level C"
WRTS
PUSH #""
WRTLNS
PUSH #"This will further test reading and math"
WRTS
PUSH #""
WRTLNS
PUSH #""
WRTS
PUSH #""
WRTLNS
PUSH #"Please enter 2 integers"
POP 1(D0)
PUSH #""
WRTS
PUSH 1(D0)
WRTS
PUSH #""
WRTS
PUSH #""
WRTS
PUSH #""
WRTS
PUSH #""
WRTS
PUSH #""
WRTLNS
RD 2(D0)
RD 3(D0)
PUSH 2(D0)
WRTS
PUSH #" * "
WRTS
PUSH 3(D0)
WRTS
PUSH #" = "
WRTS
PUSH 2(D0)
PUSH 3(D0)
MULS
WRTS
PUSH #""
WRTLNS
PUSH #""
WRTS
PUSH #""
WRTLNS
PUSH #"Please enter 2 floats"
POP 1(D0)
PUSH #""
WRTS
PUSH #""
WRTS
PUSH #""
WRTS
PUSH #""
WRTS
PUSH #""
WRTS
PUSH #""
WRTS
PUSH 1(D0)
WRTS
PUSH #""
WRTLNS
RDF 5(D0)
RDF 6(D0)
PUSH 5(D0)
WRTS
PUSH #" * "
WRTS
PUSH 6(D0)
WRTS
PUSH #" = "
WRTS
PUSH 5(D0)
PUSH 6(D0)
MULSF
WRTS
PUSH #""
WRTLNS
PUSH #""
WRTS
PUSH #""
WRTLNS
PUSH #"Now to double your first flaot and integer"
WRTS
PUSH #""
WRTLNS
PUSH 5(D0)
PUSH 5(D0)
MULSF
POP 5(D0)
PUSH 2(D0)
PUSH 2(D0)
MULS
POP 2(D0)
PUSH 5(D0)
WRTS
PUSH #" and "
WRTS
PUSH 2(D0)
WRTS
PUSH #""
WRTLNS
PUSH #""
WRTS
PUSH #""
WRTLNS
PUSH #"Now to increase your second integer by your second float and vice versa"
WRTS
PUSH #" (should come out the same value)"
WRTS
PUSH #""
WRTLNS
PUSH 3(D0)
PUSH 6(D0)
SUB SP #1 SP
CASTSF
ADD SP #1 SP
MULSF
CASTSI
POP 2(D0)
PUSH 3(D0)
PUSH 6(D0)
SUB SP #1 SP
CASTSF
ADD SP #1 SP
MULSF
POP 6(D0)
PUSH 2(D0)
POP 3(D0)
PUSH 6(D0)
WRTS
PUSH #" and "
WRTS
PUSH 3(D0)
WRTS
PUSH #""
WRTS
PUSH #""
WRTLNS
SUB SP #6 SP
POP D0
HLT
