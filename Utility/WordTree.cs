using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public class WordTree
    {
        public Tree wordTree = new Tree();
        public List<Tree> Vocabulary = new List<Tree>();

        public WordTree()
        {

        }

        public WordTree(string[] words) { 
            foreach (string word in words)
            {
                Tree Leaf = wordTree.addWord(word);
                if (Leaf != null) Vocabulary.Add(Leaf);
            }

            
        }

        public WordTree reverseWordTree()
        {
            WordTree returnWordTree;
            string[] reversedWords = new string[Vocabulary.Count()];

            int wordNumber = 0;
            foreach (Tree wordLeaf in Vocabulary)
            {
                Tree currBranch = wordLeaf;
                string word = "";

                while (currBranch.branches != null )
                {
                    if (currBranch.letter == '\0') break;
                    word += currBranch.letter;
                    currBranch = currBranch.rootTree;

                }
                reversedWords[wordNumber] = word;
                wordNumber++;
            }
            returnWordTree = new WordTree(reversedWords);

            return returnWordTree;
        }

        public class Tree
        {
            public Dictionary<char,Tree> branches = new Dictionary<char, Tree>();
            public Tree rootTree = null;
            public char letter;
            public bool isWord = false;
            //  Adds a word to the word tree
            //  Returns the leaf containing the last letter of the added word
            //  Returns null if the word already existed
            public Tree addWord(string word)
            {
                Tree currTree = this;
                foreach (char letter in word)
                {
                    if (currTree.branches.ContainsKey(letter))
                    {
                        currTree = currTree.branches[letter];
                    }
                    else
                    {
                        currTree.branches.Add(letter, new Tree());
                        currTree.branches[letter].rootTree = currTree;
                        currTree.branches[letter].rootTree = currTree;
                        currTree.letter = letter;
                    }
                }
                if (currTree.isWord == true) return null;
                currTree.isWord = true;
                return currTree;
            }
        }

    }
}
