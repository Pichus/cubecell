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
    | 'NOT' expression                                    # NotExpr
    | expression ('OR' | 'AND') expression               # LogicalExpr
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
    : CELL_REF                                            # SimpleCell
    | CELL_REF ':' CELL_REF                              # RangeCell
    ;

number
    : NUMBER
    ;

string
    : STRING
    ;

// Lexer Rules (ORDER MATTERS!)
CELL_REF
    : '$'? [A-Z]+ '$'? [0-9]+
    ;

NUMBER
    : '-'? [0-9]+ ('.' [0-9]+)?
    ;

STRING
    : '"' (~["\r\n])* '"'
    ;

IDENTIFIER
    : [A-Z][A-Z0-9_]*
    ;

WS
    : [ \t\r\n]+ -> skip
    ;