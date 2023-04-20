using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using SMath.Manager;


namespace MaximaPlugin.PlotImage
{
    class Draw
    {
        /// <summary>
        /// Calls Maxima to generate the plot
        /// </summary>
        /// <param name="region"></param>
        /// <returns>File name</returns>
        public static string RegionDraw(MaximaPluginCanvas region)
        {
            //PREPARE INPUT
            Regex rxSys = new Regex(@"sys\(", RegexOptions.None);
            Regex rxUnit = new Regex(@"(['][\w\d]+)", RegexOptions.None);
            string textHolder = region.lastPlotRequest;
            // tag "polar" as noun
            textHolder = textHolder.Replace("polar(", ControlObjects.Replacement.Noun + "polar(");
            // wrap in system if required
            if (! textHolder.StartsWith("sys("))
                textHolder = "sys(" + textHolder + ",1,1)";
            // neutralize units
            textHolder = rxUnit.Replace(textHolder, "1");
            //REFRESH SETTINGSLIST
            region.plotStore.filename = Path.ChangeExtension(region.imageFilePath, null).Replace("\\", "/"); ;
            region.plotStore.MakeLists();
            //GET STRINGS OUT
            MaximaPlugin.ControlObjects.Translator.originalStrings = new List<string>();
            List<string> sl = MaximaPlugin.ControlObjects.Translator.GetStringsOutAndReplaceThem(new List<string>() { textHolder });
            //ADD SETTINGS TO PLOT
            MaximaPlugin.Converter.ElementStoreManager esm = new MaximaPlugin.Converter.ElementStoreManager();
            esm = MaximaPlugin.Converter.MatrixAndListFromSMathToMaxima.SMathListDataCollection(esm, sl[0]);
            //esm = MaximaPlugin.Converter.MatrixAndListFromSMathToMaxima.SMathListDataCollection(esm, textHolder);
            ListHandle(esm, region.plotStore.commandList, region.plotStore.prambleList);
            terminate(esm, region.plotStore.commandList, region.plotStore.prambleList);
            textHolder = MaximaPlugin.Converter.MatrixAndListFromSMathToMaxima.MakeTermString(esm, "", "");
            textHolder = textHolder.Substring(1, (textHolder.Length - 2));
            //PUT STRINGS IN
            sl = MaximaPlugin.ControlObjects.Translator.PutOriginalStringsIn(new List<string>() { textHolder });
            //SEND
            ControlObjects.TranslationModifiers.TimeOut = 5000;
            if (region.plotStore.plotType == PlotStore.PlotType.plot2D)
                textHolder = ControlObjects.Translator.Ask("draw2d(" + sl[0] + ") ");
            else
                textHolder = ControlObjects.Translator.Ask("draw3d(" + sl[0] + ") ");
            Regex rxError = new Regex(@"(syntax)|(error)", RegexOptions.None);
            if (rxError.IsMatch(textHolder, 0))
            {
                return textHolder;
            }
            else
                return null;
        }

        public static bool findPreL = false;
        public static void ListHandle(MaximaPlugin.Converter.ElementStoreManager esm, List<string> commands, List<string> preamble)
        {
            Regex rxPreamble = new Regex(@"preamble");
            for (int i = 0; i < esm.currentStore.items; i++)
            {
                if (findPreL) break;
                for (int j = 0; j < esm.currentStore.itemData[i].Count; j++)
                {
                    if (findPreL) break;
                    else if (esm.currentStore.itemData[i][j] == esm.layerMsg)
                    {
                        esm.nextElementStore();
                        ListHandle(esm, commands, preamble);
                        esm.prevElementStore();
                    }
                    else if (rxPreamble.Match(esm.currentStore.itemData[i][0], 0).Success)
                    {
                        findPreL = true;
                        if (esm.currentStore.itemData[i].Count > 1 && esm.currentStore.itemData[i][1] == esm.layerMsg)
                        {
                            bool tmp = false;
                            if (esm.currentStore.itemData[i][0][0] == '(')
                            {
                                esm.currentStore.itemData[i][0] = "user_preamble≡";
                                tmp = true;
                            }
                            esm.nextElementStore();
                            string temp = "";
                            for (int o = 0; o < preamble.Count; o++)
                            {
                                if (o == preamble.Count - 1)
                                    temp = temp + preamble[o];
                                else
                                    temp = temp + preamble[o] + GlobalProfile.ArgumentsSeparatorStandard + " ";
                            }
                            esm.currentStore.itemData.Insert(0, (new List<string>() { temp }));
                            esm.currentStore.items++;
                            esm.currentStore.rows = esm.currentStore.items;
                            esm.currentStore.cols = esm.currentStore.items;
                            esm.currentStore.refPointer = 0;
                            esm.prevElementStore();
                            if (tmp && esm.currentStore.itemData[i].Count > 1)
                            {
                                esm.currentStore.itemData[i].RemoveAt(2);
                            }
                        }
                        else
                        {
                            string temp = esm.currentStore.itemData[i][0].Substring(14);
                            if (temp[0] == '≡') temp = temp.Substring(1);
                            int open = 0, close = 0;
                            for (int z = 0; z < temp.Length; z++)
                            {
                                if (temp[z] == '(') { open++; }
                                else if (temp[z] == ')') { close++; }
                            }
                            if (open - close == -1) temp = temp.Substring(0, temp.Length - 1);
                            esm.currentStore.itemData.RemoveAt(i);
                            string temp2 = "[";
                            for (int o = 0; o < preamble.Count; o++)
                            {
                                temp2 = temp2 + preamble[o] + GlobalProfile.ArgumentsSeparatorStandard + " ";
                            }
                            temp2 = temp2 + temp + "]";
                            esm.currentStore.itemData.Insert(0, (new List<string>() { "user_preamble≡" + temp2 }));
                        }
                    }
                }
            }
            esm.currentStore.refPointer = 0;
        }
        public static void terminate(MaximaPlugin.Converter.ElementStoreManager esm, List<string> commands, List<string> preamble)
        {
            string temp = "[";

            if (!findPreL)
            {
                temp = "[";
                for (int o = 0; o < preamble.Count; o++)
                {
                    if (o == preamble.Count - 1)
                        temp = temp + preamble[o];
                    else
                        temp = temp + preamble[o] + GlobalProfile.ArgumentsSeparatorStandard + " ";
                }
                temp = temp + "]";
                esm.currentStore.itemData.Insert(0, (new List<string>() { "user_preamble≡" + temp }));

                esm.currentStore.items++;
                esm.currentStore.rows = esm.currentStore.items;
                esm.currentStore.cols = esm.currentStore.items;

            }



            temp = "";
            for (int o = 0; o < commands.Count; o++)
            {
                if (o == commands.Count - 1)
                    temp = temp + commands[o];
                else
                    temp = temp + commands[o] + GlobalProfile.ArgumentsSeparatorStandard + " ";
            }

            esm.currentStore.itemData.Insert(0, (new List<string>() { temp }));

            esm.currentStore.items++;
            esm.currentStore.rows = esm.currentStore.items;
            esm.currentStore.cols = esm.currentStore.items;
            esm.currentStore.refPointer = 0;
            findPreL = false;
            esm.gotoFirstElementStore();

        }

    }
}
