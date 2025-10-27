grammar CubeCell;

formula
    : expression EOF
    ;

expression
    : '(' expression ')'                                               # ParenExpr
    | CELL_REF                                                         # CellRefExpr
    | NUMBER                                                           # NumberExpr
    | 'max' '(' expression ',' expression ')'                          # MaxExpr
    | 'min' '(' expression ',' expression ')'                          # MinExpr
    | 'mmax' '(' expressionList ')'                                    # MmaxExpr
    | 'mmin' '(' expressionList ')'                                    # MminExpr
    | ('+' | '-') expression                                           # UnaryPlusMinusExpr
    | 'not' expression                                                 # NotExpr
    | 'inc' expression                                                 # IncExpr
    | 'dec' expression                                                 # DecExpr
    | <assoc=right> expression '^' expression                          # PowerExpr
    | expression ('*' | '/' | 'div' | 'mod') expression                # MulDivModExpr
    | expression ('+' | '-') expression                                # AddSubExpr
    | expression ('=' | '<>' | '<' | '>' | '<=' | '>=') expression     # ComparisonExpr
    | expression 'and' expression                                      # AndExpr
    | expression 'or' expression                                       # OrExpr
    | expression 'eqv' expression                                      # EqvExpr
    ;

expressionList
    : expression (',' expression)*
    ;

CELL_REF
    : [A-Za-z]+ [0-9]+
    ;

NUMBER
    : [0-9]+
    ;

WS
    : [ \t\r\n]+ -> skip
    ;
