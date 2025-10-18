grammar CubeCell;

// Parser Rules
formula
    : '=' expression EOF
    ;

expression
    : expression '^' expression                           # PowerExpr
    | expression ('*' | '/') expression                   # MulDivExpr
    | expression ('+' | '-') expression                   # AddSubExpr
    | expression ('=' | '<' | '>') expression            # ComparisonExpr
    | expression ('<=' | '>=' | '<>') expression         # ComparisonExpr2
    | 'not' expression                                    # NotExpr
    | expression ('or' | 'and') expression               # LogicalExpr
    | functionCall                                        # FunctionExpr
    | cellReference                                       # CellRefExpr
    | number                                              # NumberExpr
    | string                                              # StringExpr
    | '(' expression ')'                                  # ParenExpr
    ;

functionCall
    : IDENTIFIER '(' (expression (',' expression)*)? ')'
    ;

cellReference
    : COLUMN ROW (':' COLUMN ROW)?                        # AbsoluteOrRelativeCell
    | '$' COLUMN '$' ROW (':' '$' COLUMN '$' ROW)?       # AbsoluteCell
    ;

number
    : NUMBER
    ;

string
    : STRING
    ;

// Lexer Rules
NUMBER
    : '-'? [0-9]+ ('.' [0-9]+)?
    ;

STRING
    : '"' (~["\r\n])* '"'
    ;

IDENTIFIER
    : [a-zA-Z_][a-zA-Z0-9_]*
    ;

COLUMN
    : [A-Z]+
    ;

ROW
    : [0-9]+
    ;

WS
    : [ \t\r\n]+ -> skip
    ;