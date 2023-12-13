using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kyrsova
{
    internal class Compression
    {
        public Compression() { }
        public void DeCompression(string FileNameArchieve, string FileName)
        {
            byte[] data = File.ReadAllBytes(FileNameArchieve);
            byte[] arch = DeCompressionBytes(data);
            File.WriteAllBytes(FileName, arch);
        }

        private byte[] DeCompressionBytes(byte[] arch)
        {
            ParseHeader(arch, out int length, out int startIndex, out int[] freqs);
            Node root = CreateTree(freqs);
            byte[] data = DeCompress(arch, startIndex, length, root);
            return data;
        }

        private byte[] DeCompress(byte[] arch, int startIndex, int length, Node root)
        {
            int size = 0;
            Node curent = root;
            List<byte> data = new List<byte>();
            for (int i = startIndex; i < arch.Length; i++)
                for (int bit = 1; bit <= 128; bit <<= 1)
                {
                    bool zero = (arch[i] & bit) == 0;
                    if (zero)
                        curent = curent.bit0;
                    else
                        curent = curent.bit1;
                    if (curent.bit0 != null)
                        continue;
                    if (size++ < length)
                        data.Add(curent.symbol);
                    curent = root;
                }
            return data.ToArray();
        }

        private void ParseHeader(byte[] arch, out int length, out int startIndex, out int[] freqs)
        {
            length = arch[0] | (arch[1] << 8) | (arch[1] << 16) | (arch[1] << 24);
            freqs = new int[256];
            for (int i = 0; i < 256; i++)
            {
                freqs[i] = arch[4 + i];
            }
            startIndex = 4 + 256;
        }

        public void CompressionFile(string FileName, string FileNameArchieve)
        {
            byte[] data = File.ReadAllBytes(FileName);
            byte[] arch = CompressionBytes(data);
            File.WriteAllBytes(FileNameArchieve, arch);
        }
        private byte[] CompressionBytes(byte[] data)
        {
            int[] freqs = CreateFrequencyDictionary(data);
            byte[] head = CreateHeader(data.Length, freqs);
            Node root = CreateTree(freqs);
            string[] code = CreateCodes(root);
            byte[] bits = Compress(data, code);
            return head.Concat(bits).ToArray();
        }

        private int[] CreateFrequencyDictionary(byte[] data)
        {
            int[] freq = new int[256];
            foreach (byte b in data)
                freq[b]++;
            NormalizeFreq();
            return freq;

            void NormalizeFreq()
            {
                int max = freq.Max();
                if (max <= 255) return;
                for (int i = 0; i < 256; i++)
                {
                    if (freq[i] > 0)
                        freq[i] = 1 + freq[i] * 255 / (max + 1);
                }
            }
        }
        private byte[] CreateHeader(int length, int[] freqs)
        {
            List<byte> head = new List<byte>();
            head.Add((byte)(length & 255));
            head.Add((byte)((length >> 8) & 255));
            head.Add((byte)((length >> 16) & 255));
            head.Add((byte)((length >> 32) & 255));
            for (int i = 0; i < 256; i++)
                head.Add((byte)freqs[i]);
            return head.ToArray();
        }

        private Node CreateTree(int[] freqs)
        {
            var qr = new PriorityQuery<Node>();
            for (int i = 0; i < 256; i++)
            {
                if (freqs[i] > 0)
                {
                    qr.Enqueue(freqs[i], new Node((byte)i, freqs[i]));
                }
            }

            while (qr.Size() > 1)
            {
                Node bit0 = qr.Dequeue();
                Node bit1 = qr.Dequeue();
                int freq = bit0.freq + bit1.freq;
                Node next = new Node(bit0, bit1, freq);
                qr.Enqueue(freq, next);
            }
            return qr.Dequeue();
        }

        private string[] CreateCodes(Node root)
        {
            string[] codes = new string[256];
            Next(root, "");
            return codes;

            void Next(Node node, string code)
            {
                if (node.bit0 == null)
                    codes[node.symbol] = code;
                else
                {
                    Next(node.bit0, code + "0");
                    Next(node.bit1, code + "1");
                }
            }
        }

        private byte[] Compress(byte[] data, string[] codes)
        {
            List<byte> bits = new List<byte>();
            byte sum = 0;
            byte bit = 1;
            foreach (byte symbol in data)
                foreach (char c in codes[symbol])
                {
                    if (c == '1')
                        sum |= bit;
                    if (bit < 128)
                        bit <<= 1;
                    else
                    {
                        bits.Add(sum);
                        sum = 0;
                        bit = 1;
                    }
                }
            if (bit > 1)
                bits.Add(sum);
            return bits.ToArray();
        }
    }
}
