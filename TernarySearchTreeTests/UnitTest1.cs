using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TernarySearchTree;

namespace TernarySearchTreeTests
{
	[TestClass]
	public class UnitTest1
	{
		//[TestMethod]
		//public void make_sure_creating_two_word_tree_result_in_one_branch_when_first_added_is_substring_of_second_added()
		//{
		//    string firstWord = "ab";
		//    string secondWord = "abc";
		//    var charStream = "".ToObservable();
		//    var tree = new TreeSearchTree<char>(new List<IEnumerable<char>>(new String[] { firstWord, secondWord }), charStream);
		//    TreeSearchTree<char>.Node node_a = new TreeSearchTree<char>.Node('a');
		//    TreeSearchTree<char>.Node node_b = new TreeSearchTree<char>.Node('b');
		//    TreeSearchTree<char>.Node node_c = new TreeSearchTree<char>.Node('c');

		//    node_a.AppendNodeInBranch(TODO, node_b);
		//    node_b.AppendNodeInBranch(TODO, node_c);

		//    Assert.IsTrue(tree.Equals(node_a));
		//}

		[TestMethod]
		public void tree_with_one_word_should_match_when_fed_same_word()
		{
			string expected = "Hello";
			IObservable<char> charStream = expected.ToObservable();
			var tree = new TernarySearchTree<Char>(new List<IEnumerable<char>>(new[] {"Hello"}), charStream);
			tree.Subscribe(match => Assert.AreEqual(expected, match));
		}

		[TestMethod]
		public void tree_with_several_words_should_match_any_given_word_in_the_tree()
		{
			string expected = "cute";
			IObservable<char> charStream = expected.ToObservable();
			var tree = new TernarySearchTree<char>(new List<IEnumerable<char>>(new[] {"as", "at", "cup", "cute", "he", "i", "us"}),
			                                    charStream);
			tree.Subscribe(match => Assert.AreEqual(expected, match));
		}
	}
}