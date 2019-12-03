using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketGoogle
{
    public class Indexer : IIndexer
    {
        private Dictionary<string, Dictionary<int, List<int>>> Ids =
            new Dictionary<string, Dictionary<int, List<int>>>();

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

        public void Add(int id, string documentText)
        {
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
            return;
        }
    }
}