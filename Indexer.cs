using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PocketGoogle
{
    public class Indexer : IIndexer
    {
        private Dictionary<string, Dictionary<int, List<int>>> Ids =
            new Dictionary<string, Dictionary<int, List<int>>>();
        
        private Dictionary<int, string> texts = new Dictionary<int, string>();

        private static Tuple<string, int> GetWord(int startIndex, string text)
        {
            var word = new StringBuilder();
            var splitters = new HashSet<char>(new[] {' ', '.', ',', '!', '?', ':', '-', '\r', '\n'});
            while (!splitters.Contains(text[startIndex]) && startIndex < text.Length)
            {
                word.Append(text[startIndex]);
                startIndex++;
            }

            return Tuple.Create(word.ToString(), startIndex);
        }

        private static List<Tuple<string, int>> GetTuples(string text)
        {
            var index = 0;
            var words = new List<Tuple<string, int>>();
            while (index < text.Length)
            {
                var tuple = GetWord(index, text);
                var word = tuple.Item1;
                if (word == string.Empty)
                {
                    index++;
                    continue;
                }
                words.Add(Tuple.Create(word, index));
                index = tuple.Item2 + 1;
            }

            return words;
        }

        public void Add(int id, string documentText)
        {
            texts[id] = documentText;
            var words = GetTuples(documentText);
            foreach (var tuple in words)
            {
                var word = tuple.Item1;
                var position = tuple.Item2;
                if (!Ids.ContainsKey(word))
                {
                    Ids[word] = new Dictionary<int, List<int>>();
                }

                if (!Ids[word].ContainsKey(id))
                {
                    Ids[word][id] = new List<int>();
                }
                
                Ids[word][id].Add(position);
            }
        }

        public List<int> GetIds(string word)
        {
            return !Ids.TryGetValue(word, out var result) ? new List<int>() : result.Keys.ToList();
        }

        public List<int> GetPositions(int id, string word)
        {
            Dictionary<int, List<int>> dictionary;
            List<int> positions;
            if(!Ids.TryGetValue(word, out dictionary))
            {
                return new List<int>();
            }

            return dictionary.TryGetValue(id, out positions) ? positions : new List<int>();
        }

        public void Remove(int id)
        {
            var text = texts[id];
            var tuples = GetTuples(text);
            foreach (var word in tuples.Select(tuple => tuple.Item1))
            {
                Ids[word].Remove(id);
            }
        }
    }
    
    [TestFixture]
    public static class Tests
    {
        [Test]
        public static void Test()
        {
            var indexer = new Indexer();
            var text = "A rabbit sleeps. A hole stands.";
            indexer.Add(0, text);
            text = "A. A aldus.";
            indexer.Add(1, text);
            var positions = indexer.GetPositions(0, "A");
            var expectedPositions = new[] {0, 17};
            Assert.AreEqual(positions, expectedPositions);
            var ids = indexer.GetIds("A");
            var expectedIds = new[] {0, 1};
            Assert.AreEqual(ids, expectedIds);
        }
    }
}