L0:
PUSH D0
MOV SP D0
ADD SP #3 SP
PUSH #10
POP 0(D0)
L1:
PUSH #0
PUSH 0(D0)
CMPLES
BRFS L2
PUSH #10
POP 1(D0)
L3:
PUSH #0
PUSH 1(D0)
CMPLES
BRFS L4
PUSH 0(D0)
WRTS
PUSH #""
WRTLNS
PUSH #-1
PUSH 1(D0)
ADDS
POP 1(D0)
BR L3
L4:
PUSH 0(D0)
PUSH #1
ADDS
POP 0(D0)
PUSH #-1
PUSH 0(D0)
ADDS
POP 0(D0)
BR L1
L2:
SUB SP #3 SP
POP D0
HLT
