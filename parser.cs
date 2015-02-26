using System;
using System.IO;
using System.Collections.Generic;

public class Parser {
    // Dunders because derp
    private List<Token> __tokens;
    private Token __lookahead;
    private List<Token>.Enumerator __e;

    public Parser(List<Token> tokens){
        this.__tokens = tokens;
        this.__e = __tokens.GetEnumerator();
        this.__e.MoveNext();
        this.__lookahead = __e.Current;
    }

    public void Parse(){
        systemGoal();
    }

    // Error
    private void error(string expected){
        throw new Exception(
            String.Format(
                Constants.ERROR_SYNTAX,
                __lookahead.Line,
                __lookahead.Column
            ) +
            String.Format(
                Constants.ERROR_PARSER,
                expected,
                __lookahead.Lexeme
            )
        );
    }

    // Match
    private void match(TOKENS token){
        // Simple output
        Console.WriteLine("Matching " + __lookahead.Lexeme);
        if(token == TOKENS.EOF){
            return;
        } else if(__e.Current.Type == token){
            if(__e.MoveNext()){
                __lookahead = __e.Current;
            } else {
                throw new Exception(
                    "Expected token, got null!"
                );
            }
        } else {
            throw new Exception("Match called on nonmatching token!");
        }
    }

    // Non-terminals
    private void systemGoal(){
        switch(__lookahead.Type){
            case TOKENS.PROGRAM:
                program();
                match(TOKENS.EOF);
                Console.WriteLine("The input program parses!");
                break;
            default:
                error("'program'");
                break;
        }
    }

    private void program(){
        switch(__lookahead.Type){
            case TOKENS.PROGRAM:
                programHeading();
                match(TOKENS.SCOLON);
                block();
                match(TOKENS.PERIOD);
                break;
            default:
                error("'program'");
                break;
        }
    }

    private void programHeading(){
        switch(__lookahead.Type){
            case TOKENS.PROGRAM:
                match(TOKENS.PROGRAM);
                programIdentifier();
                break;
            default:
                error("'program'");
                break;
        }
    }

    private void block(){
        switch(__lookahead.Type){
            case TOKENS.BEGIN:
            case TOKENS.FUNCTION:
            case TOKENS.PROCEDURE:
            case TOKENS.VAR:
                variableDeclarationPart();
                procedureAndFunctionDeclarationPart();
                statementPart();
                break;
            default:
                error("one of: 'begin', 'function', 'procedure', 'var'");
                break;
        }
    }

    private void variableDeclarationPart(){
        switch(__lookahead.Type){
            case TOKENS.VAR:
                match(TOKENS.VAR);
                variableDeclaration();
                match(TOKENS.SCOLON);
                variableDeclarationTail();
                break;
            case TOKENS.BEGIN:
            case TOKENS.FUNCTION:
            case TOKENS.PROCEDURE:
                break;
            default:
                error("one of: 'var', 'begin', 'function', 'procedure'");
                break;
        }
    }

    private void variableDeclarationTail(){
        switch(__lookahead.Type){
            case TOKENS.IDENTIFIER:
                variableDeclaration();
                match(TOKENS.SCOLON);
                variableDeclarationTail();
                break;
            case TOKENS.PROCEDURE:
                break;
            default:
                error("an identifier or 'procedure'");
                break;
        }
    }

    private void variableDeclaration(){
        switch(__lookahead.Type){
            case TOKENS.IDENTIFIER:
                identifierList();
                match(TOKENS.COLON);
                type();
                break;
            default:
                error("an identifier");
                break;
        }
    }

    private void type(){
        switch(__lookahead.Type){
            case TOKENS.INTEGER:
                match(TOKENS.INTEGER);
                break;
            case TOKENS.FLOAT:
                match(TOKENS.FLOAT);
                break;
            case TOKENS.STRING:
                match(TOKENS.STRING);
                break;
            case TOKENS.BOOLEAN:
                match(TOKENS.BOOLEAN);
                break;
            default:
                error("a type");
                break;
        }
    }

    private void procedureAndFunctionDeclarationPart(){
        switch(__lookahead.Type){
            case TOKENS.PROCEDURE:
                procedureDeclaration();
                procedureAndFunctionDeclarationPart();
                break;
            case TOKENS.FUNCTION:
                functionDeclaration();
                procedureAndFunctionDeclarationPart();
                break;
            case TOKENS.BEGIN:
                break;
            default:
                error("one of: 'procedure', 'function', 'begin'");
                break;
        }
    }

    private void procedureDeclaration(){
        switch(__lookahead.Type){
            case TOKENS.PROCEDURE:
                procedureHeading();
                match(TOKENS.SCOLON);
                block();
                match(TOKENS.SCOLON);
                break;
            default:
                error("'procedure'");
                break;
        }
    }

    private void functionDeclaration(){
        switch(__lookahead.Type){
            case TOKENS.FUNCTION:
                functionHeading();
                match(TOKENS.SCOLON);
                block();
                match(TOKENS.SCOLON);
                break;
            default:
                error("'function'");
                break;
        }
    }

    private void procedureHeading(){
        switch(__lookahead.Type){
            case TOKENS.PROCEDURE:
                match(TOKENS.PROCEDURE);
                procedureIdentifier();
                optionalFormalParameterList();
                break;
            default:
                error("'procedure'");
                break;
        }
    }

    private void functionHeading(){
        switch(__lookahead.Type){
            case TOKENS.FUNCTION:
                match(TOKENS.FUNCTION);
                functionIdentifier();
                optionalFormalParameterList();
                match(TOKENS.COLON);
                type();
                break;
            default:
                error("'function'");
                break;
        }
    }

    private void optionalFormalParameterList(){
        switch(__lookahead.Type){
            case TOKENS.LPAREN:
                match(TOKENS.LPAREN);
                formalParameterSection();
                formalParameterSectionTail();
                match(TOKENS.RPAREN);
                break;
            case TOKENS.COLON:
            case TOKENS.SCOLON:
                break;
            default:
                error("one of: '(', ':', ';'");
                break;
        }
    }

    private void formalParameterSectionTail(){
        switch(__lookahead.Type){
            case TOKENS.SCOLON:
                match(TOKENS.SCOLON);
                formalParameterSection();
                formalParameterSectionTail();
                break;
            case TOKENS.RPAREN:
                break;
            default:
                error("one of: ';', ')'");
                break;
        }
    }

    private void formalParameterSection(){
        switch(__lookahead.Type){
            case TOKENS.IDENTIFIER:
                valueParameterSection();
                break;
            case TOKENS.VAR:
                variableParameterSection();
                break;
            default:
                error("an identifier or 'var'");
                break;
        }
    }

    private void valueParameterSection(){
        switch(__lookahead.Type){
            case TOKENS.IDENTIFIER:
                identifierList();
                match(TOKENS.COLON);
                type();
                break;
            default:
                error("an identifier");
                break;
        }
    }

    private void variableParameterSection(){
        switch(__lookahead.Type){
            case TOKENS.VAR:
                match(TOKENS.VAR);
                identifierList();
                match(TOKENS.COLON);
                type();
                break;
            default:
                error("'var'");
                break;
        }
    }

    private void statementPart(){
        switch(__lookahead.Type){
            case TOKENS.BEGIN:
                compoundStatement();
                break;
            default:
                error("'begin'");
                break;
        }
    }

    private void compoundStatement(){
        switch(__lookahead.Type){
            case TOKENS.BEGIN:
                match(TOKENS.BEGIN);
                statementSequence();
                match(TOKENS.END);
                break;
            default:
                error("'begin'");
                break;
        }
    }

    private void statementSequence(){
        switch(__lookahead.Type){
            case TOKENS.BEGIN:
            case TOKENS.END:
            case TOKENS.FOR:
            case TOKENS.IF:
            case TOKENS.READ:
            case TOKENS.REPEAT:
            case TOKENS.WHILE:
            case TOKENS.WRITE:
            case TOKENS.WRITELN:
            case TOKENS.IDENTIFIER:
            case TOKENS.SCOLON:
                statement();
                statementTail();
                break;
            default:
                error("an identifier or one of: " +
                    "'begin', 'end', 'for', 'if', 'read', 'repeat', " +
                    "'while', 'write', 'writeln', ':'"
                );
                break;
        }
    }

    private void statementTail(){
        switch(__lookahead.Type) {
            case TOKENS.END:
                break;
            case TOKENS.SCOLON:
                match(TOKENS.SCOLON);
                statement();
                statementTail();
                break;
            default:
                error("end of line or ';'");
                break;
        }
    }

    private void statement(){
        switch(__lookahead.Type) {
            case TOKENS.BEGIN:
                compoundStatement();
                break;
            case TOKENS.FOR:
                forStatement();
                break;
            case TOKENS.IF:
                ifStatement();
                break;
            case TOKENS.READ:
                readStatement();
                break;
            case TOKENS.REPEAT:
                repeatStatement();
                break;
            case TOKENS.WHILE:
                whileStatement();
                break;
            case TOKENS.WRITE:
            case TOKENS.WRITELN:
                writeStatement();
                break;
            case TOKENS.IDENTIFIER:
                assignmentStatement();
                // OR procedureStatement();
                break;
            case TOKENS.END:
            case TOKENS.SCOLON:
                emptyStatement();
                break;
            default:
                error("an identifier or one of: " +
                    "'begin', 'end', 'for', 'if', 'read', 'repeat', " +
                    "'while', 'write', 'writeln', ':'"
                );
                break;
        }
    }

    private void emptyStatement(){
        switch(__lookahead.Type) {
            case TOKENS.SCOLON:
                break;
            default:
                error("';'");
                break;
        }
    }

    private void readStatement(){
        switch(__lookahead.Type) {
            case TOKENS.READ:
                match(TOKENS.READ);
                match(TOKENS.LPAREN);
                readParameter();
                readParameterTail();
                match(TOKENS.RPAREN);
                break;
            default:
                error("'read'");
                break;
        }
    }

    private void readParameterTail(){
        switch(__lookahead.Type) {
            case TOKENS.COMMA:
                match(TOKENS.COMMA);
                readParameter();
                readParameterTail();
                break;
            case TOKENS.RPAREN:
                break;
            default:
                error("',',')'");
                break;
        }
    }

    private void readParameter(){
        switch(__lookahead.Type) {
            case TOKENS.IDENTIFIER:
                variableIdentifier();
                break;
            default:
                error("An identifier");
                break;
        }
    }

    private void writeStatement(){
        switch(__lookahead.Type) {
            case TOKENS.WRITE:
                match(TOKENS.WRITE);
                match(TOKENS.LPAREN);
                writeParameter();
                writeParameterTail();
                match(TOKENS.RPAREN);
                break;
            case TOKENS.WRITELN:
                match(TOKENS.WRITELN);
                match(TOKENS.LPAREN);
                writeParameter();
                writeParameterTail();
                match(TOKENS.RPAREN);
                break;
            default:
                error("'write','writeln'");
                break;
        }
    }

    private void writeParameterTail(){
        switch(__lookahead.Type) {
            case TOKENS.COMMA:
                match(TOKENS.COMMA);
                writeParameter();
                writeParameterTail();
                break;
            case TOKENS.RPAREN:
                break;
            default:
                error("',',')'");
                break;
        }
    }

    private void writeParameter(){
        switch(__lookahead.Type) {
            case TOKENS.FALSE:
            case TOKENS.NOT:
            case TOKENS.TRUE:
            case TOKENS.IDENTIFIER:
            case TOKENS.INTEGER_LIT:
            case TOKENS.FIXED_LIT:
            case TOKENS.FLOAT_LIT:
            case TOKENS.STRING_LIT:
            case TOKENS.LPAREN:
            case TOKENS.MINUS:
            case TOKENS.PLUS:
                ordinalExpression();
                break;
            default:
                error(
                    "An identifier, integer, float, fixed, string, or any of the following: " +
                    "'false','not','true','(','-','+'"
                );
                break;
        }
    }

    private void assignmentStatement(){
        switch(__lookahead.Type) {
            case TOKENS.IDENTIFIER:
                variableIdentifier();
                // OR functionIdentifier();
                match(TOKENS.ASSIGN);
                expression();
                break;
            default:
                error("An identifier");
                break;
        }
    }

    private void ifStatement(){
        switch(__lookahead.Type) {
            case TOKENS.IF:
                match(TOKENS.IF);
                booleanExpression();
                match(TOKENS.THEN);
                statement();
                optionalElsePart();
                break;
            default:
                error("'if'");
                break;
        }
    }

    private void optionalElsePart(){
        switch(__lookahead.Type) {
            case TOKENS.ELSE:
                match(TOKENS.ELSE);
                statement();
                break;
            case TOKENS.END:
            case TOKENS.SCOLON:
                break;
            default:
                error("end of line or 'else',';'");
                break;
        }
    }

    private void repeatStatement(){
        switch(__lookahead.Type) {
            case TOKENS.REPEAT:
                match(TOKENS.REPEAT);
                statementSequence();
                match(TOKENS.UNTIL);
                booleanExpression();
                break;
            default:
                error("'repeat'");
                break;
        }
    }

    private void whileStatement(){
        switch(__lookahead.Type) {
            case TOKENS.WHILE:
                match(TOKENS.WHILE);
                booleanExpression();
                match(TOKENS.DO);
                statement();
                break;
            default:
                error("'while'");
                break;
        }
    }

    private void forStatement(){
        switch(__lookahead.Type) {
            case TOKENS.FOR:
                match(TOKENS.FOR);
                controlVariable();
                match(TOKENS.ASSIGN);
                initialValue();
                stepValue();
                finalValue();
                match(TOKENS.DO);
                statement();
                break;
            default:
                error("'for'");
                break;
        }
    }

    private void controlVariable(){
        switch(__lookahead.Type) {
            case TOKENS.IDENTIFIER:
                variableIdentifier();
                break;
            default:
                error("An identifier");
                break;
        }
    }

    private void initialValue(){
        switch(__lookahead.Type) {
            case TOKENS.FALSE:
            case TOKENS.NOT:
            case TOKENS.TRUE:
            case TOKENS.IDENTIFIER:
            case TOKENS.INTEGER_LIT:
            case TOKENS.FIXED_LIT:
            case TOKENS.FLOAT_LIT:
            case TOKENS.STRING_LIT:
            case TOKENS.LPAREN:
            case TOKENS.MINUS:
            case TOKENS.PLUS:
                ordinalExpression();
                break;
            default:
                error(
                    "An identifier, integer, fixed, float, string or any of the following: " +
                    "'false','not','true','(','-','+'"
                );
                break;
        }
    }

    private void stepValue(){
        switch(__lookahead.Type) {
            case TOKENS.DOWNTO:
                match(TOKENS.DOWNTO);
                break;
            case TOKENS.TO:
                match(TOKENS.TO);
                break;
            default:
                error("'downto','to'");
                break;
        }
    }

    private void finalValue(){
        switch(__lookahead.Type) {
            case TOKENS.FALSE:
            case TOKENS.NOT:
            case TOKENS.TRUE:
            case TOKENS.IDENTIFIER:
            case TOKENS.INTEGER_LIT:
            case TOKENS.FIXED_LIT:
            case TOKENS.FLOAT_LIT:
            case TOKENS.STRING_LIT:
            case TOKENS.LPAREN:
            case TOKENS.MINUS:
            case TOKENS.PLUS:
                ordinalExpression();
                break;
            default:
                error(
                    "An identifier, integer, fixed, float, string or any of the following: " +
                    "'false','not','true','(','-','+'"
                );
                break;
        }
    }

    private void procedureStatement(){
        switch(__lookahead.Type) {
            case TOKENS.IDENTIFIER:
                procedureIdentifier();
                optionalActualParameterList();
                break;
            default:
                error("An identifier");
                break;
        }
    }

    private void optionalActualParameterList(){
        switch(__lookahead.Type) {
            case TOKENS.END:
            case TOKENS.SCOLON:
                break;
            case TOKENS.LPAREN:
                match(TOKENS.LPAREN);
                actualParameter();
                actualParameterTail();
                match(TOKENS.RPAREN);
                break;
            default:
                error("End of line or ';','('");
                break;
        }
    }

    private void actualParameterTail(){
        switch(__lookahead.Type){
            case TOKENS.COMMA:
                // Rule 70
                match(TOKENS.COMMA);
                actualParameter();
                actualParameterTail();
                break;
            case TOKENS.RPAREN:
                // Rule 71
                break;
            default:
                error("An error with actualParameterTail");
                break;
        }
    }

    private void actualParameter(){
        switch(__lookahead.Type){
            case TOKENS.FALSE:
            case TOKENS.NOT:
            case TOKENS.TRUE:
            case TOKENS.IDENTIFIER:
            case TOKENS.INTEGER_LIT:
            case TOKENS.FIXED_LIT:
            case TOKENS.FLOAT_LIT:
            case TOKENS.STRING_LIT:
            case TOKENS.LPAREN:
            case TOKENS.MINUS:
            case TOKENS.PLUS:
                // Rule 72
                ordinalExpression();
                break;
            default:
                error("An error with actualParameter");
                break;
        }
    }

    private void expression(){
        switch(__lookahead.Type){
            case TOKENS.FALSE:
            case TOKENS.NOT:
            case TOKENS.TRUE:
            case TOKENS.IDENTIFIER:
            case TOKENS.INTEGER_LIT:
            case TOKENS.FIXED_LIT:
            case TOKENS.FLOAT_LIT:
            case TOKENS.STRING_LIT:
            case TOKENS.LPAREN:
            case TOKENS.MINUS:
            case TOKENS.PLUS:
                // Rule 73
                simpleExpression();
                optionalRelationalPart();
                break;
            default:
                error("An error with expression");
                break;
        }
    }

    private void optionalRelationalPart(){
        switch(__lookahead.Type){
            case TOKENS.DO:
            case TOKENS.DOWNTO:
            case TOKENS.END:
            case TOKENS.THEN:
            case TOKENS.TO:
            case TOKENS.COMMA:
            case TOKENS.RPAREN:
            case TOKENS.SCOLON:
                // Rule 75
                break;
            case TOKENS.EQUAL:
            case TOKENS.GEQUAL:
            case TOKENS.GTHAN:
            case TOKENS.LEQUAL:
            case TOKENS.LTHAN:
            case TOKENS.NEQUAL:
                // Rule 74
                relationalOperator();
                simpleExpression();
                break;
            default:
                error("An error with optionalRelationalPart");
                break;
        }
    }

    private void relationalOperator(){
        switch(__lookahead.Type){
            case TOKENS.EQUAL:
                // Rule 76
                match(TOKENS.EQUAL);
                break;
            case TOKENS.LTHAN:
                // Rule 77
                match(TOKENS.LTHAN);
                break;
            case TOKENS.GTHAN:
                // Rule 78
                match(TOKENS.GTHAN);
                break;
            case TOKENS.LEQUAL:
                // Rule 79
                match(TOKENS.LEQUAL);
                break;
            case TOKENS.GEQUAL:
                // Rule 80
                match(TOKENS.GEQUAL);
                break;
            case TOKENS.NEQUAL:
                // Rule 81
                match(TOKENS.NEQUAL);
                break;
            default:
                error("An error with relationalOperator");
                break;
        }
    }

    private void simpleExpression(){
        switch(__lookahead.Type){
            case TOKENS.FALSE:
            case TOKENS.NOT:
            case TOKENS.TRUE:
            case TOKENS.IDENTIFIER:
            case TOKENS.INTEGER_LIT:
            case TOKENS.FIXED_LIT:
            case TOKENS.FLOAT_LIT:
            case TOKENS.STRING_LIT:
            case TOKENS.LPAREN:
            case TOKENS.MINUS:
            case TOKENS.PLUS:
                // Rule 82
                optionalSign();
                term();
                termTail();
                break;
            default:
                error("An error with simpleExpression");
                break;
        }
    }

    private void termTail(){
        switch(__lookahead.Type){
            case TOKENS.DO:
            case TOKENS.DOWNTO:
            case TOKENS.END:
            case TOKENS.THEN:
            case TOKENS.TO:
            case TOKENS.COMMA:
            case TOKENS.EQUAL:
            case TOKENS.GEQUAL:
            case TOKENS.GTHAN:
            case TOKENS.LEQUAL:
            case TOKENS.LTHAN:
            case TOKENS.NEQUAL:
            case TOKENS.RPAREN:
            case TOKENS.SCOLON:
                // Rule 84
                break;
            case TOKENS.OR:
            case TOKENS.MINUS:
            case TOKENS.PLUS:
                // Rule 83
                addingOperator();
                term();
                termTail();
                break;
            default:
                error("An error with termTail");
                break;
        }
    }

    private void optionalSign(){
        switch(__lookahead.Type){
            case TOKENS.FALSE:
            case TOKENS.NOT:
            case TOKENS.TRUE:
            case TOKENS.IDENTIFIER:
            case TOKENS.INTEGER_LIT:
            case TOKENS.FIXED_LIT:
            case TOKENS.FLOAT_LIT:
            case TOKENS.STRING_LIT:
            case TOKENS.LPAREN:
                // Rule 87
                break;
            case TOKENS.MINUS:
                // Rule 86
                match(TOKENS.MINUS);
                break;
            case TOKENS.PLUS:
                // Rule 85
                match(TOKENS.PLUS);
                break;
            default:
                error("An error with optionalSign");
                break;
        }
    }

    private void addingOperator(){
        switch(__lookahead.Type){
            case TOKENS.OR:
                // Rule 90
                match(TOKENS.OR);
                break;
            case TOKENS.MINUS:
                // Rule 89
                match(TOKENS.MINUS);
                break;
            case TOKENS.PLUS:
                // Rule 88
                match(TOKENS.PLUS);
                break;
            default:
                error("An error with addingOperator");
                break;
        }
    }

    private void term(){
        switch(__lookahead.Type){
            case TOKENS.FALSE:
            case TOKENS.NOT:
            case TOKENS.TRUE:
            case TOKENS.IDENTIFIER:
            case TOKENS.INTEGER_LIT:
            case TOKENS.FIXED_LIT:
            case TOKENS.FLOAT_LIT:
            case TOKENS.STRING_LIT:
            case TOKENS.LPAREN:
                // Rule 91
                factor();
                factorTail();
                break;
            default:
                error("An error with term");
                break;
        }
    }

    private void factorTail(){
        switch(__lookahead.Type){
            case TOKENS.AND:
            case TOKENS.DIV:
            case TOKENS.MOD:
            case TOKENS.FLOAT_DIVIDE:
            case TOKENS.TIMES:
                // Rule 92
                multiplyingOperator();
                factor();
                factorTail();
                break;
            case TOKENS.DO:
            case TOKENS.DOWNTO:
            case TOKENS.END:
            case TOKENS.OR:
            case TOKENS.THEN:
            case TOKENS.TO:
            case TOKENS.COMMA:
            case TOKENS.EQUAL:
            case TOKENS.GEQUAL:
            case TOKENS.GTHAN:
            case TOKENS.LEQUAL:
            case TOKENS.LTHAN:
            case TOKENS.MINUS:
            case TOKENS.NEQUAL:
            case TOKENS.PLUS:
            case TOKENS.RPAREN:
            case TOKENS.SCOLON:
                // Rule 93
                break;
            default:
                error("An error with factorTail");
                break;
        }
    }

    private void multiplyingOperator(){
        switch(__lookahead.Type){
            case TOKENS.AND:
                // Rule 98
                match(TOKENS.AND);
                break;
            case TOKENS.DIV:
                // Rule 96
                match(TOKENS.DIV);
                break;
            case TOKENS.MOD:
                //Rule 97
                match(TOKENS.MOD);
                break;
            case TOKENS.FLOAT_DIVIDE:
                // Rule 95
                match(TOKENS.FLOAT_DIVIDE);
                break;
            case TOKENS.TIMES:
                // Rule 94
                match(TOKENS.TIMES);
                break;
            default:
                error("An error with multiplyingOperator");
                break;
        }
    }

    private void factor(){
        switch(__lookahead.Type){
            case TOKENS.FALSE:
                // Rule 103
                match(TOKENS.FALSE);
                break;
            case TOKENS.NOT:
                // Rule 104
                match(TOKENS.NOT);
                factor();
                break;
            case TOKENS.TRUE:
                // Rule 102
                match(TOKENS.TRUE);
                break;
            case TOKENS.IDENTIFIER:
                // Rule 106
                functionIdentifier();
                optionalActualParameterList();
                break;
            case TOKENS.INTEGER_LIT:
                // Rule 99
                match(TOKENS.INTEGER_LIT);
                break;
            case TOKENS.FIXED_LIT:
                // Rule 100?
                match(TOKENS.FIXED_LIT);
                break;
            case TOKENS.FLOAT_LIT:
                // Rule 100
                match(TOKENS.FLOAT_LIT);
                break;
            case TOKENS.STRING_LIT:
                // Rule 101
                match(TOKENS.STRING_LIT);
                break;
            case TOKENS.LPAREN:
                // Rule 105
                match(TOKENS.LPAREN);
                expression();
                match(TOKENS.RPAREN);
                break;
            default:
                error("An error with factor");
                break;
        }
    }

    private void programIdentifier(){
        switch(__lookahead.Type){
            case TOKENS.IDENTIFIER:
                // Rule 107
                match(TOKENS.IDENTIFIER);
                break;
            default:
                error("An error with programIdentifier");
                break;
        }
    }

    private void variableIdentifier(){
        switch(__lookahead.Type){
            case TOKENS.IDENTIFIER:
                // Rule 108
                match(TOKENS.IDENTIFIER);
                break;
            default:
                error("An error with variableIdentifier");
                break;
        }
    }

    private void procedureIdentifier(){
        switch(__lookahead.Type){
            case TOKENS.IDENTIFIER:
                // Rule 109
                match(TOKENS.IDENTIFIER);
                break;
            default:
                error("An error with procedureIdentifier");
                break;
        }
    }

    private void functionIdentifier(){
        switch(__lookahead.Type){
            case TOKENS.IDENTIFIER:
                // Rule 110
                match(TOKENS.IDENTIFIER);
                break;
            default:
                error("An error with functionIdentifier");
                break;
        }
    }

    private void booleanExpression(){
        switch(__lookahead.Type){
            case TOKENS.FALSE:
            case TOKENS.NOT:
            case TOKENS.TRUE:
            case TOKENS.IDENTIFIER:
            case TOKENS.FIXED_LIT:
            case TOKENS.FLOAT_LIT:
            case TOKENS.STRING_LIT:
            case TOKENS.LPAREN:
                // Rule 111
                expression();
                break;
            default:
                error("An error with booleanExpression");
                break;
        }
    }

    private void ordinalExpression(){
        switch(__lookahead.Type){
            case TOKENS.FALSE:
            case TOKENS.NOT:
            case TOKENS.TRUE:
            case TOKENS.IDENTIFIER:
            case TOKENS.INTEGER_LIT:
            case TOKENS.FIXED_LIT:
            case TOKENS.FLOAT_LIT:
            case TOKENS.STRING_LIT:
            case TOKENS.LPAREN:
                // Rule 112
                expression();
                break;
            default:
                error("An error with ordinalExpression");
                break;
        }
    }

    private void identifierList(){
        switch(__lookahead.Type){
            case TOKENS.IDENTIFIER:
                // Rule 113
                match(TOKENS.IDENTIFIER);
                identifierTail();
                break;
            default:
                error("An error with identifierList");
                break;
        }
    }

    private void identifierTail(){
        switch(__lookahead.Type){
            case TOKENS.COLON:
                // Rule 115
                break;
            case TOKENS.COMMA:
                // Rule 114
                match(TOKENS.COMMA);
                match(TOKENS.IDENTIFIER);
                identifierTail();
                break;
            default:
                error("An error with identifierTail");
                break;
        }
    }
}
