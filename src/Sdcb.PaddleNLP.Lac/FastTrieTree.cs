using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sdcb.PaddleNLP.Lac;

internal class FastTrieTree : IEnumerable<string>
{
    private readonly Dictionary<string, bool> tree = [];

    public void Add(string word)
    {
        tree[word] = true;

        for (int i = 1; i < word.Length; i++)
        {
            string fragment = word[..i];
            if (!tree.ContainsKey(fragment))
            {
                tree[fragment] = false;
            }
        }
    }

    public List<(int start, int end)> FindAll(string content)
    {
        List<(int start, int end)> result = [];
        int length = content.Length;

        for (int start = 0; start < length; start++)
        {
            for (int end = start + 1; end <= length; end++)
            {
                string subString = content[start..end];
                if (tree.TryGetValue(subString, out bool defined))
                {
                    if (defined && (result.Count == 0 || end > result[^1].end))
                    {
                        result.Add((start, end));
                    }
                }
                else
                {
                    break;
                }
            }
        }

        return result;
    }

    public IEnumerator<string> GetEnumerator()
    {
        return tree.Keys.Select(x => x).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
