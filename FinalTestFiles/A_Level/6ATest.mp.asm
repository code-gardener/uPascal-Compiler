BR L0
L1:
ADD SP #0 SP
PUSH 1(D1)
PUSH #0
CMPEQS
BRFS L2
PUSH #1
POP -1(D1)
BR L3
L2:
PUSH 1(D1)
ADD SP #1 SP
PUSH D1
PUSH 1(D1)
PUSH #1
SUBS
SUB SP #2 D1
CALL L1
SUB SP #1 SP
POP D1
MULS
POP -1(D1)
L3:
SUB SP #0 SP
RET
L4:
ADD SP #0 SP
SUB SP #0 SP
RET
L0:
PUSH D0
ADD SP #1 SP
SUB SP #2 D0
PUSH #"This is test 7 of level A"
WRTS
PUSH #""
WRTLNS
PUSH #"This should not compile"
WRTS
PUSH #""
WRTLNS
PUSH #"Enter an integer to find the factorial of (hint: factorial of 6 is 720)"
WRTS
PUSH #""
WRTLNS
RD 1(D0)
PUSH #"The factorial of "
WRTS
PUSH 1(D0)
WRTS
PUSH #" is "
WRTS
ADD SP #1 SP
PUSH D1
