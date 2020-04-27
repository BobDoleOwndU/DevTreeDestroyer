using System;
using System.IO;

namespace EquipDevelopConstSettingTool
{
    class Program
    {
        private static CstDev[] cstDevs;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string path = args[0];

                if (Path.GetExtension(path) == ".lua")
                {
                    ReadFile(args[0]);
                    ModifyEntries();
                    WriteFile(path);
                } //if
                else
                    Console.WriteLine("Invalid file");
            } //if
        } //Main

        private static void ReadFile(string path)
        {
            try
            {
                const int SKIP_ENTRIES = 2;
                string file = File.ReadAllText(path);
                string[] entries = file.Split('{');
                int length = entries.Length;
                cstDevs = new CstDev[length - SKIP_ENTRIES];

                for (int i = SKIP_ENTRIES; i < length; i++)
                {
                    string s = entries[i];
                    CstDev cstDev = new CstDev();

                    Console.WriteLine($"Index {i - SKIP_ENTRIES}\n================================");

                    if (s.Contains("p00"))
                    {
                        int startIndex = s.IndexOf("p00") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p00 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p00 = {cstDev.p00}");
                    } //if

                    if (s.Contains("p01"))
                    {
                        int startIndex = s.IndexOf("p01") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p01 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p01 = {cstDev.p01}");
                    } //if

                    if (s.Contains("p02"))
                    {
                        int startIndex = s.IndexOf("p02") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p02 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p02 = {cstDev.p02}");
                    } //if

                    if (s.Contains("p03"))
                    {
                        int startIndex = s.IndexOf("p03") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p03 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p03 = {cstDev.p03}");
                    } //if

                    if (s.Contains("p04"))
                    {
                        int startIndex = s.IndexOf("p04") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p04 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p04 = {cstDev.p04}");
                    } //if

                    if (s.Contains("p05"))
                    {
                        int startIndex = s.IndexOf("p05") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p05 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p05 = {cstDev.p05}");
                    } //if

                    if (s.Contains("p06"))
                    {
                        int startIndex = s.IndexOf("p06") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p06 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p06 = {cstDev.p06}");
                    } //if

                    if (s.Contains("p07"))
                    {
                        int startIndex = s.IndexOf("p07") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p07 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p07 = {cstDev.p07}");
                    } //if

                    if (s.Contains("p08"))
                    {
                        int startIndex = s.IndexOf("p08") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p08 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p08 = {cstDev.p08}");
                    } //if

                    if (s.Contains("p09"))
                    {
                        int startIndex = s.IndexOf("p09") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p09 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p09 = {cstDev.p09}");
                    } //if

                    if (s.Contains("p10"))
                    {
                        int startIndex = s.IndexOf("p10") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p10 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p10 = {cstDev.p10}");
                    } //if

                    if (s.Contains("p11"))
                    {
                        int startIndex = s.IndexOf("p11") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p11 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p11 = {cstDev.p11}");
                    } //if

                    if (s.Contains("p12"))
                    {
                        int startIndex = s.IndexOf("p12") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p12 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p12 = {cstDev.p12}");
                    } //if

                    if (s.Contains("p13"))
                    {
                        int startIndex = s.IndexOf("p13") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p13 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p13 = {cstDev.p13}");
                    } //if

                    if (s.Contains("p14"))
                    {
                        int startIndex = s.IndexOf("p14") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p14 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p14 = {cstDev.p14}");
                    } //if

                    if (s.Contains("p15"))
                    {
                        int startIndex = s.IndexOf("p15") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p15 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p15 = {cstDev.p15}");
                    } //if

                    if (s.Contains("p16"))
                    {
                        int startIndex = s.IndexOf("p16") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p16 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p16 = {cstDev.p16}");
                    } //if

                    if (s.Contains("p17"))
                    {
                        int startIndex = s.IndexOf("p17") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p17 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p17 = {cstDev.p17}");
                    } //if

                    if (s.Contains("p30"))
                    {
                        int startIndex = s.IndexOf("p30") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p30 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p30 = {cstDev.p30}");
                    } //if

                    if (s.Contains("p31"))
                    {
                        int startIndex = s.IndexOf("p31") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p31 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p31 = {cstDev.p31}");
                    } //if

                    if (s.Contains("p32"))
                    {
                        int startIndex = s.IndexOf("p32") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p32 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p32 = {cstDev.p32}");
                    } //if

                    if (s.Contains("p33"))
                    {
                        int startIndex = s.IndexOf("p33") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p33 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p33 = {cstDev.p33}");
                    } //if

                    if (s.Contains("p34"))
                    {
                        int startIndex = s.IndexOf("p34") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p34 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p34 = {cstDev.p34}");
                    } //if

                    if (s.Contains("p35"))
                    {
                        int startIndex = s.IndexOf("p35") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p35 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p35 = {cstDev.p35}");
                    } //if

                    if (s.Contains("p36"))
                    {
                        int startIndex = s.IndexOf("p36") + 4;
                        int endIndex = s.IndexOf('}', startIndex);
                        cstDev.p36 = s.Substring(startIndex, endIndex - startIndex);

                        Console.WriteLine($"p36 = {cstDev.p36}\n");
                    } //if

                    cstDevs[i - SKIP_ENTRIES] = cstDev;
                } //for
            } //try
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            } //catch
        } //ReadFile

        private static void ModifyEntries()
        {
            int length = cstDevs.Length;

            /*
             * 12003: C.Box (WR)
             * 12013: C.Box (SMK)
             * 12043: Stealth Camo
             * 16003: Fulton Device Grade 3
             * 16007: Fulton +Child
             * 16008: Fulton +Wormhole
             * 19024: Naked Fatigues (Gold)
             * 19060: Parasite Suit
             * 19073: Raiden
             * 37002: Infinity Bandana
             */
            string[] specialIds = { "12003", "12013", "12043", "16003", "16007", "16008", "19024", "19060", "19073", "37002" };

            Console.Write("Modifying entries...");

            for (int i = 0; i < length; i++)
            {
                CstDev cstDev = cstDevs[i];

                if (cstDev.p04 != "0")
                    cstDev.p04 = "0";

                //EXTRA is normally for DLC items. Removing it will break them. EXTRA_4010 is for the parasite powers. Removing it from them is fine.
                if (cstDev.p05 != "65535" && !(cstDev.p05.Contains("EXTRA") && !cstDev.p05.Contains("EXTRA_4010")))
                    cstDev.p05 = "0";

                if (cstDev.p03 != "0" && cstDev.p05 == "0")
                    cstDev.p05 = "65535";

                if (Array.Exists(specialIds, x=> x == cstDev.p00))
                    cstDev.p05 = "0";

                //FOB Camos
                if ((String.Compare(cstDev.p00, "19090") >= 0) && String.Compare(cstDev.p00, "19186") <= 0)
                    cstDev.p05 = "0";
            } //for

            Console.WriteLine("Done!");
        } //ModifyEntries

        private static void WriteFile(string file)
        {
            int length = cstDevs.Length;
            FileStream fileStream = new FileStream(file, FileMode.Create);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.AutoFlush = true;

            Console.Write("Writing file...");

            try
            {
                writer.Write("this={}");

                for (int i = 0; i < length; i++)
                {
                    CstDev cstDev = cstDevs[i];

                    writer.Write("TppMotherBaseManagement.RegCstDev{");
                    writer.Write($"p00={cstDev.p00},");
                    writer.Write($"p01={cstDev.p01},");
                    writer.Write($"p02={cstDev.p02},");
                    writer.Write($"p03={cstDev.p03},");
                    writer.Write($"p04={cstDev.p04},");
                    writer.Write($"p05={cstDev.p05},");
                    writer.Write($"p06={cstDev.p06},");
                    writer.Write($"p07={cstDev.p07},");
                    writer.Write($"p08={cstDev.p08},");
                    writer.Write($"p09={cstDev.p09},");

                    if (!string.IsNullOrEmpty(cstDev.p10))
                    {
                        writer.Write($"p10={cstDev.p10},");

                        if (!string.IsNullOrEmpty(cstDev.p11))
                        {
                            writer.Write($"p11={cstDev.p11},");

                            if (!string.IsNullOrEmpty(cstDev.p12))
                            {
                                writer.Write($"p12={cstDev.p12},");

                                if (!string.IsNullOrEmpty(cstDev.p13))
                                {
                                    writer.Write($"p13={cstDev.p13},");

                                    if (!string.IsNullOrEmpty(cstDev.p14))
                                    {
                                        writer.Write($"p14={cstDev.p14},");

                                        if (!string.IsNullOrEmpty(cstDev.p15))
                                        {
                                            writer.Write($"p15={cstDev.p15},");

                                            if (!string.IsNullOrEmpty(cstDev.p16))
                                            {
                                                writer.Write($"p16={cstDev.p16},");

                                                if (!string.IsNullOrEmpty(cstDev.p17))
                                                {
                                                    writer.Write($"p17={cstDev.p17},");
                                                } //if
                                            } //if
                                        } //if
                                    } //if
                                } //if
                            } //if
                        } //if
                    } //if

                    writer.Write($"p30={cstDev.p30},");
                    writer.Write($"p31={cstDev.p31},");
                    writer.Write($"p32={cstDev.p32},");
                    writer.Write($"p33={cstDev.p33},");
                    writer.Write($"p34={cstDev.p34},");
                    writer.Write($"p35={cstDev.p35},");
                    writer.Write($"p36={cstDev.p36}}}");
                } //for

                writer.Write("return this");

                Console.WriteLine("Done!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            } //catch
            finally
            {
                writer.Close();
                fileStream.Close();
            } //finally
        } //WriteFile
    } //class
} //namespace
