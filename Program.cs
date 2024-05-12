//Written for Dragon Nest. https://store.steampowered.com/app/11610
using System;
using System.IO;

namespace Eyedentity_Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryReader br = new(File.OpenRead(args[0]));

            if (new string(System.Text.Encoding.GetEncoding("ISO-8859-1").GetChars(br.ReadBytes(0x100))).TrimEnd('\0', 'ý') != "EyedentityGames Packing File 0.1")
                throw new Exception("This is not an EyedentityGames Packing File.");

            br.ReadInt32();//Unknown
            int fileCount = br.ReadInt32();
            string path = Path.GetDirectoryName(args[0]);
            br.BaseStream.Position = br.ReadInt32();
            System.Collections.Generic.List<Subfile> subfiles = new();

            for (int i = 0; i < fileCount; i++)
            {
                subfiles.Add(new()
                {
                    name = new string(System.Text.Encoding.GetEncoding("ISO-8859-1").GetChars(br.ReadBytes(0x100))).TrimEnd('\0', 'ý'),
                    size1 = br.ReadInt32(),
                    unknown = br.ReadInt32(),
                    size2 = br.ReadInt32(),//I don't know why it's here twice. 
                    start = br.ReadInt32()
                });
                if (subfiles[^1].size1 != subfiles[^1].size2)
                    throw new Exception("Fuck!");
                br.BaseStream.Position += 44;
            }

            foreach (Subfile file in subfiles)
            {
                br.BaseStream.Position = file.start;
                Directory.CreateDirectory(path + "//" + Path.GetDirectoryName(file.name));
                BinaryWriter bw = new(File.Create(path + "//" + file.name));
                bw.Write(br.ReadBytes(file.size1));
                bw.Close();
            }
        }

        class Subfile
        {
            public string name;
            public int size1;
            public int unknown;
            public int size2;//I don't know why it's here twice.
            public int start;
        }
    }
}
