BR L1
L2:
PRTS
PRTR
ADD SP #0 SP
PUSH 1(D1)
WRTS
PUSH #""
WRTLNS
SUB SP #0 SP
RET
L1:
PUSH SP
SUB SP #1 D0
ADD SP #1 SP
RD 1(D0)
PUSH SP
SUB SP #1 D1
PUSH 1(D0)
CALL L2
SUB SP #1 SP
POP D1
SUB SP #1 SP
SUB SP #1 SP
HLT