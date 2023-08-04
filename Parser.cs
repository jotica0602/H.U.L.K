// After we made our list of Tokens, we need to find the sense of the expression, if it exists.
public class Parser{
    // This is the List
    private List<Token> tokens;
    // Current Index
    private int currentTokenIndex;
    // Current Token
    private Token currentToken;
    public Parser(List<Token> tokens){
        this.tokens=tokens;
        currentTokenIndex=0;
        currentToken=tokens[currentTokenIndex];
    }

    // Now we can move through our Tokens List
    public void Next(){
        currentTokenIndex++;
        if(currentTokenIndex<tokens.Count())
            currentToken=tokens[currentTokenIndex];
        else
            currentToken=new Token(TokenKind.EndOfFile,"");
    }

    
    private double ParseToken(){
        // if the token we are looking at is a number, we parse it, return it and move to the next
        if(currentToken.Kind==TokenKind.Number){
            double number = double.Parse(currentToken.Value);
            Next();
            Console.WriteLine($"Parsed Number {number}");
            return number;
        }
        else if (currentToken.Kind==TokenKind.LeftParenthesis){
            Next();
            double number = ParseSum();
            if(currentToken.Kind!=TokenKind.RightParenthesis)
                throw new InvalidOperationException ("Syntax Error");
            Next();
            return number;
        }
        else 
            throw new InvalidOperationException ("Syntax Error");
        
    }

    private double ParseSum(){
        double expressionResult = ParseMult(); 

        while(currentToken.Kind==TokenKind.PlusOperator || currentToken.Kind==TokenKind.MinusOperator){
            Token operatorToken = currentToken;
            Next();
            double nextToken = ParseMult();
            if(operatorToken.Kind==TokenKind.PlusOperator)
                expressionResult+=nextToken;
            else if (operatorToken.Kind==TokenKind.MinusOperator)
                expressionResult-=nextToken;

        }
        return expressionResult;
    }
    private double ParseMult(){
        double expressionResult = ParsePow();
        while(currentToken.Kind==TokenKind.MultOperator || currentToken.Kind==TokenKind.DivideOperator){
            Token operatorToken = currentToken;
            Next();
            double nextToken = ParsePow();
           if(operatorToken.Kind==TokenKind.MultOperator)
                expressionResult*=nextToken;
            else if(operatorToken.Kind==TokenKind.DivideOperator)
                expressionResult/=nextToken;
        }
        return expressionResult;
    }

    private double ParsePow(){
        double expressionResult = ParseToken();
        while(currentToken.Kind==TokenKind.PowerOperator){
            Token operatorToken = currentToken;
            Next();
            // double nextToken = ParseToken();
            if(operatorToken.Kind!=TokenKind.LeftParenthesis)
                expressionResult = Math.Pow(expressionResult,ParseSum());
            else
                throw new InvalidOperationException("Syntax Error");
        }
        return expressionResult;

    }

    public double Parse(){
        double expressionResult = ParseSum();
        if(currentToken.Kind!=TokenKind.EndOfFile)
            throw new InvalidOperationException("Syntax Error");
        return expressionResult;
    }
    
}