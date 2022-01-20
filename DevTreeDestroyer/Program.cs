using System;
using System.Collections.Generic;
using System.IO;

namespace DevTreeDestroyer
{
    class Program
    {
        private static bool stripDevCosts = true;
        private static bool modifyOnlineItems = true;
        private static CstDev[] cstDevs;
        private static FlwDev[] flwDevs;
        private static List<Eqp> eqps = new List<Eqp>(0);
        private static List<Tuple<string, int>> ids = new List<Tuple<string, int>>(0);

        private static int id = 1200; //Used for giving items unique ids.

        static void Main(string[] args)
        {
            if (Array.Exists(args, x => x == "-ns"))
                stripDevCosts = false;
            if (Array.Exists(args, x => x == "-no"))
                modifyOnlineItems = false;

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
            for (int i = 0; i < eqps.Count; i++)
            {
                Eqp eqp = eqps[i];

                //Anything without a unique p01 will break. This includes suits, arms and items other than cboxes. Some buddy equipment will also break.
                //Security devices also don't work if they have their IDs changed.
                if ((eqp.flwDev.p69 == "1" || eqp.flwDev.p72 == "1") &&
                    !(eqp.cstDev.p02 == "TppMbDev.EQP_DEV_TYPE_Suit" || (eqp.cstDev.p02 == "TppMbDev.EQP_DEV_TYPE_Equip" && !eqp.cstDev.p01.Contains("TppEquip.EQP_IT_CBox")) || eqp.cstDev.p02 == "TppMbDev.EQP_DEV_TYPE_ArtificialArm" || eqp.cstDev.p02 == "TppMbDev.EQP_DEV_TYPE_Quiet" || eqp.cstDev.p02 == "TppMbDev.EQP_DEV_TYPE_SecurityGadgets") &&
                    !eqp.cstDev.p09.Contains("TppMbDev.EQP_DEV_GROUP_BUDDY") &&
                    !((String.Compare(eqp.cstDev.p00, "16001") >= 0) && (String.Compare(eqp.cstDev.p00, "16008") <= 0)) && //Ignore fulton stuff.
                    eqp.cstDev.p00 != "12003" //WR Cbox breaks too for some reason.
                   )
                {
                    ids.Add(new Tuple<string, int>(eqp.cstDev.p00, i));

                    eqp.cstDev.p00 = id.ToString();
                    id++;

                    eqp.flwDev.p69 = "0";
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
        } //ModifyOnlineItems

        private static void StripCosts()
        {
            int count = eqps.Count;

            /*
             * 1080: Water Pistol Grade 3
             * 1090: ADAM-SKA SP.
             * 1091: WUS 333 CB SP.
             * 2030: MACHT-P5 WEISS
             * 4060: RASP SB-SG GOLD
             * 9010 - 9013: DLC Shields
             * 
             * 12003: C.Box (WR)
             * 12013: C.Box (SMK)
             * 12043: Stealth Camo
             * 16003: Fulton Device Grade 3
             * 16007: Fulton +Child
             * 16008: Fulton +Wormhole
             * 19024: Naked Fatigues (Gold)
             * 19060: Parasite Suit
             * 19073: Raiden
             * 28003: Task-Arm SM
             * 37002: Infinity Bandana
             */
            string[] specialIds = { "12003", "12013", "12043", "16003", "16007", "16008", "19024", "19060", "19073", "28003", "37002" };
            string[] specialIds2 = { "1080", "1090", "1091", "2030", "4060", "9010", "9011", "9012", "9013" };

            Console.Write("Modifying entries...");

            for (int i = 0; i < count; i++)
            {
                CstDev cstDev = eqps[i].cstDev;
                FlwDev flwDev = eqps[i].flwDev;

                //CstDev Mods
                if (cstDev.p04 != "0")
                    cstDev.p04 = "0";

                //EXTRA is normally for DLC items. Removing it will break them. EXTRA_4010 is for the parasite powers. Removing it from them is fine.
                if (cstDev.p05 != "65535" && !(cstDev.p05.Contains("EXTRA") && !cstDev.p05.Contains("EXTRA_4010")))
                    cstDev.p05 = "0";

                if (cstDev.p03 != "0" && cstDev.p05 == "0")
                    cstDev.p05 = "65535";

                if (Array.Exists(specialIds, x => x == cstDev.p00))
                    cstDev.p05 = "0";

                if (Array.Exists(specialIds2, x => x == cstDev.p00))
                    cstDev.p05 = "65535";

                //FOB Camos
                //if ((String.Compare(cstDev.p00, "19090") >= 0) && String.Compare(cstDev.p00, "19186") <= 0)
                //    cstDev.p05 = "0";

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
                    flwDev.p71 = "0";
                    flwDev.p73 = "0";
                }

                flwDev.p54 = "0";
                flwDev.p65 = "\"\"";
                flwDev.p66 = "0";
                flwDev.p67 = "\"\"";
                flwDev.p68 = "0";
            } //for

            Console.WriteLine("Done!");
        } //StripCosts

        private static void DuplicateItems()
        {
            string[] dlc = { "TppMotherBaseManagementConst.EXTRA_4024", "TppMotherBaseManagementConst.EXTRA_4025", "TppMotherBaseManagementConst.EXTRA_4000",
                    "TppMotherBaseManagementConst.EXTRA_4001", "TppMotherBaseManagementConst.EXTRA_4003", "TppMotherBaseManagementConst.EXTRA_4004",
                    "TppMotherBaseManagementConst.EXTRA_4005", "TppMotherBaseManagementConst.EXTRA_4006", "TppMotherBaseManagementConst.EXTRA_4007",
                    "TppMotherBaseManagementConst.EXTRA_4008"};

            int count = eqps.Count;

            for (int i = 0; i < count; i++)
            {
                Eqp eqp = eqps[i];
                
                if(Array.Exists(dlc, x => x == eqp.cstDev.p05))
                {
                    Eqp dupe = new Eqp(eqp);

                    ids.Add(new Tuple<string, int>(dupe.cstDev.p00, eqps.Count));

                    dupe.cstDev.p00 = id.ToString();
                    id++;

                    dupe.flwDev.p50 = eqps.Count.ToString();

                    eqps.Add(dupe);
                } //if
            } //for
        } //DuplicateItems

        private static void ReadConstSetting(string path)
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
            const int SKIP_ENTRIES = 2;
            string file = File.ReadAllText(path);
            string[] entries = file.Split('{');
            int length = entries.Length;
            flwDevs = new FlwDev[length - SKIP_ENTRIES];

            for (int i = SKIP_ENTRIES; i < length; i++)
            {
                string s = entries[i];
                FlwDev flwDev = new FlwDev();

                Console.WriteLine($"Index {i - SKIP_ENTRIES}\n================================");

                if (s.Contains("p50"))
                {
                    int startIndex = s.IndexOf("p50") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p50 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p50 = {flwDev.p50}");
                } //if

                if (s.Contains("p51"))
                {
                    int startIndex = s.IndexOf("p51") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p51 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p51 = {flwDev.p51}");
                } //if

                if (s.Contains("p52"))
                {
                    int startIndex = s.IndexOf("p52") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p52 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p52 = {flwDev.p52}");
                } //if

                if (s.Contains("p53"))
                {
                    int startIndex = s.IndexOf("p53") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p53 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p53 = {flwDev.p53}");
                } //if

                if (s.Contains("p54"))
                {
                    int startIndex = s.IndexOf("p54") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p54 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p54 = {flwDev.p54}");
                } //if

                if (s.Contains("p55"))
                {
                    int startIndex = s.IndexOf("p55") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p55 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p55 = {flwDev.p55}");
                } //if

                if (s.Contains("p56"))
                {
                    int startIndex = s.IndexOf("p56") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p56 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p56 = {flwDev.p56}");
                } //if

                if (s.Contains("p57"))
                {
                    int startIndex = s.IndexOf("p57") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p57 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p57 = {flwDev.p57}");
                } //if

                if (s.Contains("p58"))
                {
                    int startIndex = s.IndexOf("p58") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p58 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p58 = {flwDev.p58}");
                } //if

                if (s.Contains("p59"))
                {
                    int startIndex = s.IndexOf("p59") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p59 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p59 = {flwDev.p59}");
                } //if

                if (s.Contains("p60"))
                {
                    int startIndex = s.IndexOf("p60") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p60 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p60 = {flwDev.p60}");
                } //if

                if (s.Contains("p61"))
                {
                    int startIndex = s.IndexOf("p61") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p61 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p61 = {flwDev.p61}");
                } //if

                if (s.Contains("p62"))
                {
                    int startIndex = s.IndexOf("p62") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p62 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p62 = {flwDev.p62}");
                } //if

                if (s.Contains("p63"))
                {
                    int startIndex = s.IndexOf("p63") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p63 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p63 = {flwDev.p63}");
                } //if

                if (s.Contains("p64"))
                {
                    int startIndex = s.IndexOf("p64") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p64 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p64 = {flwDev.p64}");
                } //if

                if (s.Contains("p65"))
                {
                    int startIndex = s.IndexOf("p65") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p65 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p65 = {flwDev.p65}");
                } //if

                if (s.Contains("p66"))
                {
                    int startIndex = s.IndexOf("p66") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p66 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p66 = {flwDev.p66}");
                } //if

                if (s.Contains("p67"))
                {
                    int startIndex = s.IndexOf("p67") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p67 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p67 = {flwDev.p67}");
                } //if

                if (s.Contains("p68"))
                {
                    int startIndex = s.IndexOf("p68") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p68 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p68 = {flwDev.p68}");
                } //if

                if (s.Contains("p69"))
                {
                    int startIndex = s.IndexOf("p69") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p69 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p69 = {flwDev.p69}");
                } //if

                if (s.Contains("p70"))
                {
                    int startIndex = s.IndexOf("p70") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p70 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p70 = {flwDev.p70}");
                } //if

                if (s.Contains("p71"))
                {
                    int startIndex = s.IndexOf("p71") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p71 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p71 = {flwDev.p71}");
                } //if

                if (s.Contains("p72"))
                {
                    int startIndex = s.IndexOf("p72") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p72 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p72 = {flwDev.p72}");
                } //if

                if (s.Contains("p73"))
                {
                    int startIndex = s.IndexOf("p73") + 4;
                    int endIndex = s.IndexOf(',', startIndex);
                    flwDev.p73 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p73 = {flwDev.p73}");
                } //if

                if (s.Contains("p74"))
                {
                    int startIndex = s.IndexOf("p74") + 4;
                    int endIndex = s.IndexOf('}', startIndex);
                    flwDev.p74 = s.Substring(startIndex, endIndex - startIndex);

                    Console.WriteLine($"p74 = {flwDev.p74}\n");
                } //if

                flwDevs[i - SKIP_ENTRIES] = flwDev;
            } //for
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

            //Console.WriteLine($"Index {index}\n================================");
            Console.WriteLine($"p00 = {eqp.cstDev.p00}");
            //Console.WriteLine($"p01 = {eqp.cstDev.p01}");
            //Console.WriteLine($"p02 = {eqp.cstDev.p02}");
            //Console.WriteLine($"p03 = {eqp.cstDev.p03}");
            //Console.WriteLine($"p04 = {eqp.cstDev.p04}");
            //Console.WriteLine($"p05 = {eqp.cstDev.p05}");
            //Console.WriteLine($"p06 = {eqp.cstDev.p06}");
            //Console.WriteLine($"p07 = {eqp.cstDev.p07}");
            //Console.WriteLine($"p08 = {eqp.cstDev.p08}");
            //Console.WriteLine($"p09 = {eqp.cstDev.p09}");
            //Console.WriteLine($"p10 = {eqp.cstDev.p10}");
            //Console.WriteLine($"p11 = {eqp.cstDev.p11}");
            //Console.WriteLine($"p12 = {eqp.cstDev.p12}");
            //Console.WriteLine($"p13 = {eqp.cstDev.p13}");
            //Console.WriteLine($"p14 = {eqp.cstDev.p14}");
            //Console.WriteLine($"p15 = {eqp.cstDev.p15}");
            //Console.WriteLine($"p16 = {eqp.cstDev.p16}");
            //Console.WriteLine($"p17 = {eqp.cstDev.p17}");
            //Console.WriteLine($"p30 = {eqp.cstDev.p30}");
            //Console.WriteLine($"p31 = {eqp.cstDev.p31}");
            //Console.WriteLine($"p32 = {eqp.cstDev.p32}");
            //Console.WriteLine($"p33 = {eqp.cstDev.p33}");
            //Console.WriteLine($"p34 = {eqp.cstDev.p34}");
            //Console.WriteLine($"p35 = {eqp.cstDev.p35}");
            //Console.WriteLine($"p36 = {eqp.cstDev.p36}");

            Console.WriteLine($"p69 = {eqp.flwDev.p69}");
            Console.WriteLine($"p72 = {eqp.flwDev.p72}");
        } //DisplayEqpInfo
    } //class
} //namespace
