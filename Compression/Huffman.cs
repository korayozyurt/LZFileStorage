using System.Text;
using LZAlgorithms;

namespace Compression;

public class Huffman
{
    public class HuffmanNode
    {
        public char Symbol { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }

        public bool IsLeaf => Left == null && Right == null;
    }


    public Dictionary<char, string> BuildHuffmanTable(string input)
    {
        Dictionary<char, int> frequencyTable = new Dictionary<char, int>();

        foreach (char c in input)
        {
            if (frequencyTable.ContainsKey(c))
                frequencyTable[c]++;
            else
                frequencyTable[c] = 1;
        }

        PriorityQueue<HuffmanNode, int> priorityQueue = new PriorityQueue<HuffmanNode, int>();
        foreach (var item in frequencyTable)
        {
            priorityQueue.Enqueue(new HuffmanNode { Symbol = item.Key, Frequency = item.Value }, item.Value);
        }

        while (priorityQueue.Count > 1)
        {
            HuffmanNode left = priorityQueue.Dequeue();
            HuffmanNode right = priorityQueue.Dequeue();
            HuffmanNode parent = new HuffmanNode
            {
                Frequency = left.Frequency + right.Frequency,
                Left = left,
                Right = right
            };
            priorityQueue.Enqueue(parent, parent.Frequency);
        }

        HuffmanNode root = priorityQueue.Dequeue();
        Dictionary<char, string> huffmanTable = new Dictionary<char, string>();
        TraverseHuffmanTree(root, string.Empty, huffmanTable);

        return huffmanTable;
    }

    private void TraverseHuffmanTree(HuffmanNode node, string code, Dictionary<char, string> huffmanTable)
    {
        if (node.IsLeaf)
        {
            huffmanTable[node.Symbol] = code;
        }
        else
        {
            TraverseHuffmanTree(node.Left, code + "0", huffmanTable);
            TraverseHuffmanTree(node.Right, code + "1", huffmanTable);
        }
    }

    public string HuffmanEncode(string input, Dictionary<char, string> huffmanTable)
    {
        StringBuilder encodedData = new StringBuilder();

        foreach (char c in input)
        {
            encodedData.Append(huffmanTable[c]);
        }

        return encodedData.ToString();
    }

    public string HuffmanDecode(string encodedData, Dictionary<char, string> huffmanTable)
    {
        Dictionary<string, char> reverseHuffmanTable = new Dictionary<string, char>();
        foreach (var kvp in huffmanTable)
        {
            reverseHuffmanTable[kvp.Value] = kvp.Key;
        }

        StringBuilder decodedData = new StringBuilder();
        string currentCode = string.Empty;

        foreach (char bit in encodedData)
        {
            currentCode += bit;

            if (reverseHuffmanTable.ContainsKey(currentCode))
            {
                decodedData.Append(reverseHuffmanTable[currentCode]);
                currentCode = string.Empty;
            }
        }

        return decodedData.ToString();
    }

}