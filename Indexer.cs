using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private static List<Tuple<string, int>> GetWords(string text)
        {
            var index = 0;
            var words = new List<Tuple<string, int>>();
            while (index < text.Length)
            {
                var tuple = GetWord(index, text);
                var word = tuple.Item1;
                var position = 0;
                if (word == string.Empty)
                    continue;
                words.Add(Tuple.Create(word, index));
                position++;
                index = tuple.Item2;
            }

            return words;
        }

        public void Add(int id, string documentText)
        {
            texts[id] = documentText;
            var index = 0;
            while (index < documentText.Length)
            {
                var tuple = GetWord(index, documentText);
                var word = tuple.Item1;
                var position = 0;
                if (word == string.Empty)
                    continue;
                if (!Ids.ContainsKey(word))
                {
                    Ids[word] = new Dictionary<int, List<int>>();
                }

                if (!Ids[word].ContainsKey(id))
                {
                    Ids[word][id] = new List<int>();
                }

                Ids[word][id].Add(position);
                position++;
                index = tuple.Item2;
            }
        }

        public List<int> GetIds(string word)
        {
            return Ids[word].Keys.ToList();
        }

        public List<int> GetPositions(int id, string word)
        {
            return Ids[word][id];
        }

        public void Remove(int id)
        {
            
        }
    }
}