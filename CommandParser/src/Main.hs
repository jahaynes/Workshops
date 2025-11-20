module Main (main) where

import CommandParser
import Parser

import Control.Monad
import Data.Either (partitionEithers)

main :: IO ()
main = do

    inputs <- lines <$> readFile "./samples"

    outputs <- forM inputs $ \i ->
        case runParser commandLine i of
            Left e  -> pure $ Left (i, e)
            Right r -> pure $ Right (i, r)

    let (bads, goods) = partitionEithers outputs

    forM_ goods $ \(i, rs) -> do
        putStrLn $ "{" ++ i ++ "}"
        forM_ rs $ \r ->
            putStrLn $ "[" ++ r ++ "]"
        putStrLn ""
        
    forM_ bads $ \(i, e) -> do
        putStrLn "Bad"
        putStrLn $ "{" ++ i ++ "}"
        print e
        putStrLn ""