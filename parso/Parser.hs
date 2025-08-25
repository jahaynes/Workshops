module Parser where

import Control.Applicative (Alternative (..))

newtype Parser a =
    Parser (String -> Either String (a, String))

instance Functor Parser where

    fmap f (Parser run) = Parser $ \s ->
        case run s of
            Left l        -> Left l
            Right (x, s') -> Right (f x, s')

instance Applicative Parser where

    pure x = Parser (\s -> Right (x, s))

    Parser pf <*> Parser px = Parser $ \s ->
        case pf s of
            Left l -> Left l
            Right (f, s') ->
                case px s' of
                    Left l -> Left l
                    Right (x, s'') -> Right (f x, s'')

instance Monad Parser where

    return = pure

    Parser run >>= mf = Parser $ \s ->
        case run s of
            Left l -> Left l
            Right (x, s') -> let Parser run' = mf x in run' s'

instance Alternative Parser where

    empty = Parser (\_ -> Left "No more alternatives")

    Parser p1 <|> Parser p2 = Parser $ \s ->
        case p1 s of
            Left{} -> p2 s
            r      -> r

instance MonadFail Parser where