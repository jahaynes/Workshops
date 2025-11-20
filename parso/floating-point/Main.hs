module Main where

import Control.Applicative ( Alternative (..) )
import Data.Char (isDigit)

newtype Parser a =
    Parser (String -> Either String (a, String)) 

instance Functor Parser where
    fmap f (Parser run) = Parser (\s ->
        case run s of
            Left e        -> Left e
            Right (x, s') -> Right (f x, s'))

instance Applicative Parser where
    pure x = Parser (\s -> Right (x, s))
    Parser pf <*> Parser px = Parser $ \s ->
        case pf s of
          Left e -> Left e
          Right (f, s') ->
            case px s' of
              Left e -> Left e
              Right (x, s'') -> Right (f x, s'')

instance Alternative Parser where

    empty = Parser (\_ -> Left "Out of choices")

    Parser pa <|> Parser pb = Parser $ \s ->
        case pa s of
            Right (x, s') -> Right (x, s')
            Left _ -> pb s

pFloatingPoint :: Parser String
pFloatingPoint  = (\n c p -> concat [n, [c] ,p]) <$> pNumber <*> pChar '.' <*> pPositive
              <|> pNumber
    
pNumber :: Parser String
pNumber = (\c p -> c:p) <$> pChar '-' <*> pPositive
      <|> pPositive

pPositive :: Parser String
pPositive = some (pCharPred isDigit)

pCharPred :: (Char -> Bool) -> Parser Char
pCharPred p = Parser $ \s ->
    case s of
        []     -> Left "No more input"
        (x:xs) -> if p x
                    then Right (x, xs)
                    else Left "Different char"

pChar :: Char -> Parser Char
pChar c = pCharPred (== c) 



main :: IO ()
main = do


    let Parser run = pFloatingPoint

    let input = "-33.43"


    print (run input)



