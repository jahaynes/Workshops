module Generators where

import           Control.Monad (foldM)
import           Data.Char (isSpace)
import           Hedgehog
import qualified Hedgehog.Gen as Gen
import qualified Hedgehog.Range as Range

genInput :: Gen String
genInput = do
    cmd  <- genTerm
    args <- Gen.list (Range.linear 0 7) genArg
    joinSpace (cmd:args)

genArg :: Gen String
genArg = Gen.choice [genTerm, quotedTerm]

    where
    quotedTerm :: Gen String
    quotedTerm = Gen.list (Range.linear 1 4) genTerm
             >>= joinSpace
             >>= quotify

joinSpace :: [String] -> Gen String
joinSpace = foldM f "" . reverse
    where
    f acc x = do
        gap <- Gen.list (Range.linear 1 2) (pure ' ')
        pure $ concat [x, gap, acc]

quotify :: String -> Gen String
quotify x = concat <$> sequence [ws, pure "\"", ws, pure x, ws, pure "\"", ws]
    where
    ws = Gen.list (Range.linear 0 2) (pure ' ')

genTerm :: Gen String
genTerm = concat <$> Gen.list (Range.linear 1 30) genChar

genChar :: Gen String
genChar = Gen.frequency [ (16, (:[]) <$> Gen.alphaNum)
                        , ( 4,           pure " ")
                        , ( 4,           pure "\\ ")
                        , ( 4,           pure "\\\"")
                        , ( 4,           pure " ")
                        ]
