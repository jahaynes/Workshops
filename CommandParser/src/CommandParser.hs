module CommandParser where

import Parser
import Strings

import Control.Applicative ((<|>), some)
import Data.Char           (isSpace)

commandLine = ws *> some (simpleToken <* ws <|> quotedToken <* ws) 

    where
    simpleToken = some simpleTokenChar
    
    simpleTokenChar = specialCharPair <|> ordinaryChar

    quotedToken = char '\"' *> some quotedTokenChar <* char '\"'

    quotedTokenChar = specialCharPair <|> ordinaryChar <|> char ' '

    specialCharPair = char '\\' *> (char '\\' <|> char '\"')

    ordinaryChar = pChar f
        where
        f '\"' = False
        f ' '  = False
        f    _ = True
