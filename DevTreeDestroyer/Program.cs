using System;
using System.Collections.Generic;
using System.IO;

namespace DevTreeDestroyer
{
    class Program
    {
        private static bool stripDevCosts = true;
        private static bool stripDepCosts = true;
        private static bool modifyOnlineItems = true;
        private static CstDev[] cstDevs;
        private static FlwDev[] flwDevs;
        private static List<Eqp> eqps = new List<Eqp>(0);
        private static List<Tuple<string, int>> ids = new List<Tuple<string, int>>(0);

        private static int id = 1200; //Used for giving items unique ids.

        static void Main(string[] args)
        {
            if (Array.Exists(args, x => x == "-ndev"))
                stripDevCosts = false;
            if (Array.Exists(args, x => x == "-ndep"))
                stripDepCosts = false;
            if (Array.Exists(args, x => x == "-nonl"))
                modifyOnlineItems = false;

            Console.WriteLine($"stripDevCosts = {stripDevCosts}");
            Console.WriteLine($"stripDepCosts = {stripDepCosts}");
            Console.WriteLine($"modifyOnlineItems = {modifyOnlineItems}");

            ReadConstSetting("EquipDevelopConstSetting.lua");
            ReadFlowSetting("EquipDevelopFlowSetting.lua");

            CreateEqp();
            StripCosts();

            if (modifyOnlineItems)
                ModifyOnlineItems();

            WriteConstSetting("EquipDevelopConstSetting2.lua");
            WriteFlowSetting("EquipDevelopFlowSetting2.lua");
        } //Main

        private static void CreateEqp()
        {
            int count = cstDevs.Length;

            for(int i = 0; i < count; i++)
            {
                Eqp eqp = new Eqp();
                eqp.cstDev = cstDevs[i];
                eqp.flwDev = flwDevs[i];

                eqps.Add(eqp);
            } //for
        } //CreateEqp

        private static void ModifyOnlineItems()
        {
            Console.Write("Offlining online weapons...");

            for (int i = 0; i < eqps.Count; i++)
            {
                Eqp eqp = eqps[i];

                string[] excludeList = { "TppMbDev.EQP_DEV_TYPE_Suit", "TppMbDev.EQP_DEV_TYPE_Equip", "TppMbDev.EQP_DEV_TYPE_ArtificialArm", "TppMbDev.EQP_DEV_TYPE_SecurityGadgets" };

                //Anything without a unique p01 will break. This includes suits, arms and items other than cboxes. Some buddy equipment will also break.
                //Security devices also don't work if they have their IDs changed.
                if (eqp.flwDev.p72 == "1"
                    && !Array.Exists(excludeList, x => x == eqp.cstDev.p02))
                {
                    ids.Add(new Tuple<string, int>(eqp.cstDev.p00, i));

                    eqp.cstDev.p00 = id.ToString();
                    id++;
                    
                    eqp.flwDev.p72 = "0";
                } //if

                //Fix parent index for any items that had their parent's id changed.
                if (eqp.cstDev.p03 != "0")
                {
                    int parentIndex = ids.Exists(x => x.Item1 == eqp.cstDev.p03) ? ids.Find(x => x.Item1 == eqp.cstDev.p03).Item2 : -1;

                    if (parentIndex != -1)
                    {
                        eqp.cstDev.p03 = eqps[parentIndex].cstDev.p00;
                    } //else
                } //if
            } //for

            Console.WriteLine("Done!");
        } //ModifyOnlineItems

        private static void StripCosts()
        {
            int count = eqps.Count;

            Console.Write("Modifying reqs...");

            for (int i = 0; i < count; i++)
            {
                CstDev cstDev = eqps[i].cstDev;
                FlwDev flwDev = eqps[i].flwDev;

                //CstDev Mods
                cstDev.p04 = "0";
                cstDev.p05 = "65535";

                //FlwDevMods
                if (stripDevCosts)
                {
                    flwDev.p53 = "0";
                    flwDev.p55 = flwDev.p55 == "0" ? "0" : "1";
                    flwDev.p56 = "0";
                    flwDev.p57 = "0";
                    flwDev.p58 = "\"\"";
                    flwDev.p59 = "0";
                    flwDev.p60 = "\"\"";
                    flwDev.p61 = "0";

                    if(modifyOnlineItems || flwDev.p72 == "0")
                        flwDev.p62 = "1";

                    flwDev.p63 = "0";
                    flwDev.p64 = "0";
                    flwDev.p71 = "0";
                    flwDev.p73 = "0";
                } //if

                if (stripDepCosts)
                {
                    flwDev.p54 = "0";
                    flwDev.p65 = "\"\"";
                    flwDev.p66 = "0";
                    flwDev.p67 = "\"\"";
                    flwDev.p68 = "0";
                } //if

                if(flwDev.p69 == "1")
                    flwDev.p69 = "0";
            } //for

            Console.WriteLine("Done!");
        } //StripCosts

        private static void ReadConstSetting(string path)
        {
            try
            {
                const int SKIP_ENTRIES = 2;
                string file = File.ReadAllText(path);
                string[] entries = file.Split('{');
                int length = entries.Length;
                cstDevs = new CstDev[length - SKIP_ENTRIES];

                Console.Write("Reading ConstSetting...");

                for (int i = SKIP_ENTRIES; i < length; i++)
                {
                    string s = entries[i];
                    CstDev cstDev = new CstDev();

                    if (s.Contains("p00"))
                    {
                        int startIndex = s.IndexOf("p00") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p00 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p01"))
                    {
                        int startIndex = s.IndexOf("p01") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p01 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p02"))
                    {
                        int startIndex = s.IndexOf("p02") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p02 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p03"))
                    {
                        int startIndex = s.IndexOf("p03") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p03 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p04"))
                    {
                        int startIndex = s.IndexOf("p04") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p04 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p05"))
                    {
                        int startIndex = s.IndexOf("p05") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p05 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p06"))
                    {
                        int startIndex = s.IndexOf("p06") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p06 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p07"))
                    {
                        int startIndex = s.IndexOf("p07") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p07 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p08"))
                    {
                        int startIndex = s.IndexOf("p08") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p08 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p09"))
                    {
                        int startIndex = s.IndexOf("p09") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p09 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p10"))
                    {
                        int startIndex = s.IndexOf("p10") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p10 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p11"))
                    {
                        int startIndex = s.IndexOf("p11") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p11 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p12"))
                    {
                        int startIndex = s.IndexOf("p12") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p12 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p13"))
                    {
                        int startIndex = s.IndexOf("p13") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p13 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p14"))
                    {
                        int startIndex = s.IndexOf("p14") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p14 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p15"))
                    {
                        int startIndex = s.IndexOf("p15") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p15 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p16"))
                    {
                        int startIndex = s.IndexOf("p16") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p16 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p17"))
                    {
                        int startIndex = s.IndexOf("p17") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p17 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p30"))
                    {
                        int startIndex = s.IndexOf("p30") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p30 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p31"))
                    {
                        int startIndex = s.IndexOf("p31") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p31 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p32"))
                    {
                        int startIndex = s.IndexOf("p32") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p32 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p33"))
                    {
                        int startIndex = s.IndexOf("p33") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p33 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p34"))
                    {
                        int startIndex = s.IndexOf("p34") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p34 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p35"))
                    {
                        int startIndex = s.IndexOf("p35") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        cstDev.p35 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p36"))
                    {
                        int startIndex = s.IndexOf("p36") + 4;
                        int endIndex = s.IndexOf('}', startIndex);
                        cstDev.p36 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    cstDevs[i - SKIP_ENTRIES] = cstDev;
                } //for

                Console.WriteLine("Done!");
            } //try
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            } //catch
        } //ReadConstSetting

        private static void WriteConstSetting(string file)
        {
            int count = eqps.Count;
            FileStream fileStream = new FileStream(file, FileMode.Create);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.AutoFlush = true;

            Console.Write("Writing ConstSetting...");

            try
            {
                writer.Write("this={}");

                for (int i = 0; i < count; i++)
                {
                    CstDev cstDev = eqps[i].cstDev;

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
            } //try
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
        } //WriteConstSetting

        private static void ReadFlowSetting(string path)
        {
            try
            {
                const int SKIP_ENTRIES = 2;
                string file = File.ReadAllText(path);
                string[] entries = file.Split('{');
                int length = entries.Length;
                flwDevs = new FlwDev[length - SKIP_ENTRIES];

                Console.Write("Reading FlowSetting...");

                for (int i = SKIP_ENTRIES; i < length; i++)
                {
                    string s = entries[i];
                    FlwDev flwDev = new FlwDev();

                    if (s.Contains("p50"))
                    {
                        int startIndex = s.IndexOf("p50") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p50 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p51"))
                    {
                        int startIndex = s.IndexOf("p51") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p51 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p52"))
                    {
                        int startIndex = s.IndexOf("p52") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p52 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p53"))
                    {
                        int startIndex = s.IndexOf("p53") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p53 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p54"))
                    {
                        int startIndex = s.IndexOf("p54") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p54 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p55"))
                    {
                        int startIndex = s.IndexOf("p55") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p55 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p56"))
                    {
                        int startIndex = s.IndexOf("p56") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p56 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p57"))
                    {
                        int startIndex = s.IndexOf("p57") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p57 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p58"))
                    {
                        int startIndex = s.IndexOf("p58") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p58 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p59"))
                    {
                        int startIndex = s.IndexOf("p59") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p59 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p60"))
                    {
                        int startIndex = s.IndexOf("p60") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p60 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p61"))
                    {
                        int startIndex = s.IndexOf("p61") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p61 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p62"))
                    {
                        int startIndex = s.IndexOf("p62") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p62 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p63"))
                    {
                        int startIndex = s.IndexOf("p63") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p63 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p64"))
                    {
                        int startIndex = s.IndexOf("p64") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p64 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p65"))
                    {
                        int startIndex = s.IndexOf("p65") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p65 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p66"))
                    {
                        int startIndex = s.IndexOf("p66") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p66 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p67"))
                    {
                        int startIndex = s.IndexOf("p67") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p67 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p68"))
                    {
                        int startIndex = s.IndexOf("p68") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p68 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p69"))
                    {
                        int startIndex = s.IndexOf("p69") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p69 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p70"))
                    {
                        int startIndex = s.IndexOf("p70") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p70 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p71"))
                    {
                        int startIndex = s.IndexOf("p71") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p71 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p72"))
                    {
                        int startIndex = s.IndexOf("p72") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p72 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p73"))
                    {
                        int startIndex = s.IndexOf("p73") + 4;
                        int endIndex = s.IndexOf(',', startIndex);
                        flwDev.p73 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    if (s.Contains("p74"))
                    {
                        int startIndex = s.IndexOf("p74") + 4;
                        int endIndex = s.IndexOf('}', startIndex);
                        flwDev.p74 = s.Substring(startIndex, endIndex - startIndex);
                    } //if

                    flwDevs[i - SKIP_ENTRIES] = flwDev;
                } //for

                Console.WriteLine("Done!");
            }//try
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            } //catch
        } //ReadFlowSetting

        private static void WriteFlowSetting(string file)
        {
            int count = eqps.Count;
            FileStream fileStream = new FileStream(file, FileMode.Create);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.AutoFlush = true;

            Console.Write("Writing FlowSetting...");

            try
            {
                writer.Write("this={}");

                for (int i = 0; i < count; i++)
                {
                    FlwDev flwDev = eqps[i].flwDev;
                    writer.Write("TppMotherBaseManagement.RegFlwDev{");
                    writer.Write($"p50={flwDev.p50},");
                    writer.Write($"p51={flwDev.p51},");
                    writer.Write($"p52={flwDev.p52},");
                    writer.Write($"p53={flwDev.p53},");
                    writer.Write($"p54={flwDev.p54},");
                    writer.Write($"p55={flwDev.p55},");
                    writer.Write($"p56={flwDev.p56},");
                    writer.Write($"p57={flwDev.p57},");
                    writer.Write($"p58={flwDev.p58},");
                    writer.Write($"p59={flwDev.p59},");
                    writer.Write($"p60={flwDev.p60},");
                    writer.Write($"p61={flwDev.p61},");
                    writer.Write($"p62={flwDev.p62},");
                    writer.Write($"p63={flwDev.p63},");
                    writer.Write($"p64={flwDev.p64},");
                    writer.Write($"p65={flwDev.p65},");
                    writer.Write($"p66={flwDev.p66},");
                    writer.Write($"p67={flwDev.p67},");
                    writer.Write($"p68={flwDev.p68},");
                    writer.Write($"p69={flwDev.p69},");
                    writer.Write($"p70={flwDev.p70},");
                    writer.Write($"p71={flwDev.p71},");
                    writer.Write($"p72={flwDev.p72},");
                    writer.Write($"p73={flwDev.p73},");
                    writer.Write($"p74={flwDev.p74}}}");
                } //for

                writer.Write("return this");

                Console.WriteLine("Done!");
            } //try
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
        } //WriteFlowSetting

        //DEBUG
        private static void DisplayEqpInfo(int index)
        {
            Eqp eqp = eqps[index];

            Console.WriteLine($"Index {index}\n================================");
            Console.WriteLine($"p00 = {eqp.cstDev.p00}");
            Console.WriteLine($"p01 = {eqp.cstDev.p01}");
            Console.WriteLine($"p02 = {eqp.cstDev.p02}");
            Console.WriteLine($"p03 = {eqp.cstDev.p03}");
            Console.WriteLine($"p04 = {eqp.cstDev.p04}");
            Console.WriteLine($"p05 = {eqp.cstDev.p05}");
            Console.WriteLine($"p06 = {eqp.cstDev.p06}");
            Console.WriteLine($"p07 = {eqp.cstDev.p07}");
            Console.WriteLine($"p08 = {eqp.cstDev.p08}");
            Console.WriteLine($"p09 = {eqp.cstDev.p09}");
            Console.WriteLine($"p10 = {eqp.cstDev.p10}");
            Console.WriteLine($"p11 = {eqp.cstDev.p11}");
            Console.WriteLine($"p12 = {eqp.cstDev.p12}");
            Console.WriteLine($"p13 = {eqp.cstDev.p13}");
            Console.WriteLine($"p14 = {eqp.cstDev.p14}");
            Console.WriteLine($"p15 = {eqp.cstDev.p15}");
            Console.WriteLine($"p16 = {eqp.cstDev.p16}");
            Console.WriteLine($"p17 = {eqp.cstDev.p17}");
            Console.WriteLine($"p30 = {eqp.cstDev.p30}");
            Console.WriteLine($"p31 = {eqp.cstDev.p31}");
            Console.WriteLine($"p32 = {eqp.cstDev.p32}");
            Console.WriteLine($"p33 = {eqp.cstDev.p33}");
            Console.WriteLine($"p34 = {eqp.cstDev.p34}");
            Console.WriteLine($"p35 = {eqp.cstDev.p35}");
            Console.WriteLine($"p36 = {eqp.cstDev.p36}");

            Console.WriteLine($"p50 = {eqp.flwDev.p50}");
            Console.WriteLine($"p51 = {eqp.flwDev.p51}");
            Console.WriteLine($"p52 = {eqp.flwDev.p52}");
            Console.WriteLine($"p53 = {eqp.flwDev.p53}");
            Console.WriteLine($"p54 = {eqp.flwDev.p54}");
            Console.WriteLine($"p55 = {eqp.flwDev.p55}");
            Console.WriteLine($"p56 = {eqp.flwDev.p56}");
            Console.WriteLine($"p57 = {eqp.flwDev.p57}");
            Console.WriteLine($"p58 = {eqp.flwDev.p58}");
            Console.WriteLine($"p59 = {eqp.flwDev.p59}");
            Console.WriteLine($"p60 = {eqp.flwDev.p60}");
            Console.WriteLine($"p61 = {eqp.flwDev.p61}");
            Console.WriteLine($"p62 = {eqp.flwDev.p62}");
            Console.WriteLine($"p63 = {eqp.flwDev.p63}");
            Console.WriteLine($"p64 = {eqp.flwDev.p64}");
            Console.WriteLine($"p65 = {eqp.flwDev.p65}");
            Console.WriteLine($"p66 = {eqp.flwDev.p66}");
            Console.WriteLine($"p67 = {eqp.flwDev.p67}");
            Console.WriteLine($"p68 = {eqp.flwDev.p68}");
            Console.WriteLine($"p69 = {eqp.flwDev.p69}");
            Console.WriteLine($"p70 = {eqp.flwDev.p70}");
            Console.WriteLine($"p71 = {eqp.flwDev.p71}");
            Console.WriteLine($"p72 = {eqp.flwDev.p72}");
            Console.WriteLine($"p73 = {eqp.flwDev.p73}");
            Console.WriteLine($"p74 = {eqp.flwDev.p74}");
        } //DisplayEqpInfo
    } //class
} //namespace
