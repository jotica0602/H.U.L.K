// After we made our list of Tokens, we need to find the sense of the expression, if it exists.
public class Parser{

    private List<Token> tokens;
    private int currentTokenIndex;
    private Token currentToken;
    private List<TokenKind> invalidtokens = new List<TokenKind>(){
        TokenKind.Colon,
        TokenKind.Comma,
        TokenKind.Semicolon,
        TokenKind.FullStop,
        TokenKind.RightCurlyBracket,
        TokenKind.Quote,
        TokenKind.RightBracket

    };

    public Parser(List<Token>tokens){
        this.tokens = tokens;
        currentTokenIndex=0;
        currentToken=tokens[currentTokenIndex];
    }

    // Now we can Move through our Tokens List
    private void Next(){
        currentTokenIndex++;
        if(currentTokenIndex<tokens.Count())
            currentToken=tokens[currentTokenIndex];
    
        else
            currentToken= new Token(TokenKind.EndOfFile, "");
    }

    public double Parse(){
        double result = ParseExpression();
        
        // We are not getting out of the list bounds
        if (currentToken.Kind != TokenKind.EndOfFile)
            throw new InvalidOperationException("Syntax Error");

        return result;
    }


    private double ParseExpression(){
        double result = ParseTerm();

        while (currentToken.Kind == TokenKind.PlusOperator || currentToken.Kind == TokenKind.MinusOperator){
            Token operatorToken = currentToken;
            Next();

            double term = ParseTerm();
            if (operatorToken.Kind == TokenKind.PlusOperator)
                result += term;
            else if (operatorToken.Kind == TokenKind.MinusOperator)
                result -= term;
        }

        return result;
    }

    private double ParseTerm(){

        double result = ParseNumber();

        while (currentToken.Kind == TokenKind.MultOperator || currentToken.Kind == TokenKind.DivideOperator){
            Token operatorToken = currentToken;
            Next();

            double factor = ParseNumber();
            if (operatorToken.Kind == TokenKind.MultOperator)
                result *= factor;
            else if (operatorToken.Kind == TokenKind.DivideOperator)
                result /= factor;
        }

        return result;
    }

    private double ParseNumber(){
        // if we are looking at a Number Token:
        // We parse it, return it and move on
        if (currentToken.Kind == TokenKind.Number){
            double number = double.Parse(currentToken.Value);
            Next();
            return number;
        }
        // if there's a parenthesis we move on and our result will be the expression we've 
        // gotten so far
        else if (currentToken.Kind == TokenKind.LeftParenthesis){
            Next();
            double result = ParseExpression();

            // if we don't find a right parenthesis it will be a invalid expression
            if (currentToken.Kind != TokenKind.RightParenthesis)
                throw new InvalidOperationException("Syntax Error");

            Next();
            return result;
        }
        // any other case will be an invalid expression for now.
        else{
            throw new InvalidOperationException("Syntax Error");
        }
    }

}