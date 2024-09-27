using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public class WordTree
    {
        protected Tree wordTree = new Tree();


        public WordTree(string[] words) { 
            foreach (string word in Words)
            {
                wordTree.addWord(word);
            }


        }


        public class Tree
        {
            Dictionary<char,Tree> branches = new Dictionary<char, Tree>();
            Tree RootTree = null;
            char letter;

            //  Adds a word to the word tree
            //  Returns the leaf containing the last letter of the added word
            //
            public Tree addWord(string word)
            {
                Tree currTree = this;
                foreach (char letter in word)
                {
                    if (currTree.branches.ContainsKey(letter))
                    {
                        currTree = currTree.branches[letter];
                        currTree.letter = letter;
                    }
                    else
                    {
                        currTree.branches.Add(letter, new Tree);
                        currTree.branches[letter].RootTree = currTree;
                        currTree.branches[letter].RootTree = currTree;
                        currTree.letter = letter;
                    }
                }
                return currTree;
            }
        }

    }
}
