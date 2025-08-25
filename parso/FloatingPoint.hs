module Main where

import Control.Applicative
import Parser

{-

BNF Grammar for a decimal (floating point) number:

    <decimal>  :: <negative> | <positive>
    <negative> :: - <positive>
    <positive> :: <integer> ( . <integer> )?
    <integer>  :: <digit>+
    <digit>    :: [0-9]

-}

decimal, negative, positive, integer, digit :: Parser ()

decimal  = negative <|> positive
negative = char '-' *> positive
positive = integer *> opt (char '.' *> integer)
integer  = oneOrMore digit
digit    = choose (map char ['0'..'9'])

runParser :: Parser a -> String -> Either String ()
runParser (Parser run) s =
    case run s of
        Right (_, "") -> Right ()
        Right (_,  _) -> Left "Leftover input"
        Left{}        -> Left "Parse failed"

main :: IO ()
main = do

    -- Positive test cases
    print (runParser decimal "1"      == Right ())
    print (runParser decimal "123"    == Right ())
    print (runParser decimal "-1"     == Right ())
    print (runParser decimal "1.39"   == Right ())
    print (runParser decimal "-1.39"  == Right ())

    -- Negative test cases
    print (runParser decimal ""       == Left "Parse failed")
    print (runParser decimal "abc"    == Left "Parse failed")
    print (runParser decimal "- 1"    == Left "Parse failed")
    print (runParser decimal "1bcd"   == Left "Leftover input")   
    print (runParser decimal "--1"    == Left "Parse failed")



{-

    https://regexr.com/

    ^-?[0-9]+(:?\.[0-9]*)?$

    1
    123
    -1
    1.39
    -1.39

    abc
    - 1
    1bcd
    --1

-}

choose :: [Parser a] -> Parser ()
choose ps = asum ps *> pure ()

oneOrMore :: Parser a -> Parser ()
oneOrMore p = some p *> pure ()

opt :: Parser a -> Parser ()
opt p = optional p *> pure ()

charp :: (Char -> Bool) -> Parser Char
charp p = Parser f
    where
    f (x:xs) | p x       = Right (x, xs)
             | otherwise = Left "Char failed predicate"
    f []                 = Left "Out of input"

char :: Char -> Parser Char
char c = charp (==c)