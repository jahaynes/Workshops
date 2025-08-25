module Main where

import Control.Applicative
import Data.Char (isSpace)

import Parser


    -- TODO extra whitespace

{-
    <CommandLine>   ::=     <QuotedCommand> | <Command>

    <QuotedCommand> ::=     " <Command> "

    <Command>       ::=     (QuotedToken | Token) +

    <QuotedToken>   ::=     " <Char>+ "

    <Token>         ::=     <Char>+

    <Char>          ::=     \ <AnyChar>
                      |     <ValidChar>

    <AnyChar>       ::=     Any char

    <ValidChar>     ::=     Any char except double-quote or backslash or whitespace
-}

data Command = 
    Command { cmd  :: !String
            , args :: ![String]
            } deriving (Eq, Show)

parseTopLevel :: Parser Command
parseTopLevel = do
    (cmd:args) <- quotedCommand
    pure Command { cmd = cmd, args = args }

quotedCommand :: Parser [String]
quotedCommand = ws *> (   char '\"' *> command <* char '\"'
                      <|> command )

command :: Parser [String]
command = some (quotedToken <* ws <|> token <* ws)

quotedToken :: Parser String
quotedToken = char '\"' *> token <* char '\"'

token :: Parser String
token = some tChar

tChar :: Parser Char
tChar = char '\\' *> anyChar
    <|> validChar

anyChar :: Parser Char
anyChar = pChar (\_ -> True)

validChar :: Parser Char
validChar = pChar f
    where
    f '\"' = False
    f '\\' = False
    f c    = not (isSpace c)

ws :: Parser ()
ws = many (pChar isSpace) *> pure ()

pChar :: (Char -> Bool) -> Parser Char
pChar p = Parser f
    where
    f (x:xs) | p x       = Right (x, xs)
             | otherwise = Left "Char failed predicate"
    f []                 = Left "Out of input"

char :: Char -> Parser Char
char c = pChar (==c)

runParser :: Parser a -> String -> Either String a
runParser (Parser run) s =
    case run s of
        Right (x, "") -> Right x
        Right (_,  _) -> Left "Leftover input"
        Left{}        -> Left "Parse failed"

main :: IO ()
main = do

    let positives = [ ("foo", Right (Command "foo" []))
                    , ("foo bar", Right (Command "foo" ["bar"]))
                    , ("foo bar baz", Right (Command "foo" ["bar", "baz"]))
                    ]

    let negatives = [ ("", Left "Parse failed")
                    --, ("\"", Left "Parse failed")
                    --, ("\"\"", Left "Parse failed")
                    ]

    mapM_ testCase positives
    mapM_ testCase negatives

testCase :: (String, Either String Command) -> IO ()
testCase (input, output) = do

    let result = runParser parseTopLevel input

    if result == output
        then putStrLn $ "\nSuccess: " ++ input
        else putStrLn $ "\nFAILURE: " ++ input ++ ".  Expected:\n\t" ++ show output ++ "\nGot:\n\t" ++ show result

