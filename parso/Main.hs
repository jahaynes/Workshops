import Parser

import Data.Char           (isDigit, isSpace)

import Control.Applicative ((<|>), many, some)

charp :: (Char -> Bool) -> Parser Char
charp p = Parser f
    where
    f (x:xs) | p x       = Right (x, xs)
             | otherwise = Left "Char failed predicate"
    f []                 = Left "Out of input"

char :: Char -> Parser Char
char c = charp (==c)

expr :: ([Char] -> a) -> (a -> [(Char, a)] -> a) -> Parser a
expr tokeniser folder =

    folder <$> multiplication
           <*> many (plus <|> minus) 

    where
    plus  = (,) <$> char '+' <*> multiplication
    minus = (,) <$> char '-' <*> multiplication

    multiplication = folder <$> term
                            <*> many (times <|> divide)

        where
        times  = (,) <$> char '*' <*> term
        divide = (,) <$> char '/' <*> term
        term = number <|> bracketing
            where
            number = tokeniser <$> some (charp isDigit)
            bracketing = char '(' *> expr tokeniser folder <* char ')'

_collapse :: Integer -> [(Char, Integer)] -> Integer
_collapse x [] = x
_collapse x ((c,y):zs) = let a = (op c) x y in _collapse a zs
    where
    op '+' = (+)
    op '-' = (-)
    op '*' = (*)
    op '/' = div
    op   _ = error "bad op"

data Expr = Plus Expr Expr
          | Minus Expr Expr
          | Times Expr Expr
          | Div Expr Expr
          | Num Integer
instance Show Expr where
    show (Plus a b)  = "(" ++ show a ++ "+" ++ show b ++ ")"
    show (Minus a b) = "(" ++ show a ++ "-" ++ show b ++ ")"
    show (Times a b) = "(" ++ show a ++ "*" ++ show b ++ ")"
    show (Div a b)   = "(" ++ show a ++ "/" ++ show b ++ ")"
    show (Num i)     = show i

collapse2 :: Expr -> [(Char, Expr)] -> Expr
collapse2 x [] = x
collapse2 x ((c,y):zs) = let a = (op c) x y in collapse2 a zs
    where
    op '+' = Plus
    op '-' = Minus
    op '*' = Times
    op '/' = Div
    op   _ = error "bad op"

main :: IO ()
main = do
    let Parser run = expr (read) _collapse

    let input = " 3 + 8 / 4 / 2"

    case run  (filter (not . isSpace) input) of
        Right (x, "") -> print x
        Right _ -> error "Leftover"
        Left x -> error $ show x

{- Points of interest:
    
    * Taking the same 'parser' which evaluates as it goes, and replacing it with an AST builder

    * Left-recursion

    * Monads -> Linq -> Do-notation -> async/await

-}