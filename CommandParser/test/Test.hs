module Main (main) where

import CommandParser
import Generators
import Parser

import           Control.Monad
import           Hedgehog
import           Hedgehog.Gen (sample)

main :: IO ()
main = writeFile "./samples" . unlines =<< replicateM 1000 (sample genInput)
-- main = void (checkParallel $$(discover))

prop_mytest :: Property
prop_mytest = withTests 10000 . property $ do
    input <- forAll genInput
    case runParser commandLine input of
        Left l  -> error $ "Could not parse: {" ++ input ++ "}" ++ "\n" ++ l
        Right{} -> pure ()
