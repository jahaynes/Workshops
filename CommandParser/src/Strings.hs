module Strings where

import Parser

import Control.Applicative
import Data.Char (isSpace)

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