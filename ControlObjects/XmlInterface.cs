using System.Collections.Generic;
using System.Xml;

namespace MaximaPlugin
{
    /// <summary>
    /// object for storage of conversion strings
    /// </summary>
    class ExpressionStore
    {
        public string functionName= "";
        public string regex = "";
        public string replace = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fn">String</param>
        /// <param name="rx">value for regex</param>
        /// <param name="re">value for replace</param>
        public ExpressionStore(string fn, string rx, string re)
        {
            functionName = fn;
            regex = rx;
            replace = re;
        }
    }
}


namespace MaximaPlugin.ControlObjects
{
    static class XmlInterface
    {
        public static int counterS = 0;

        /// <summary>
        /// Write the configuration to an xml file
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="wSettings">Plugin configuration info</param>
        /// <param name="commands">Default maxima init commands</param>
        /// <param name="customCommands">Custom maxima init commands</param>
        /// <param name="exprSMathToMaxima">Custom SMath to Maxima translations</param>
        /// <param name="exprMaximaToSMath">Custom Maxima to SMath translations</param>
        public static void writeXml(string path, List<string> wSettings, List<string> commands, List<string> customCommands, List<ExpressionStore> exprSMathToMaxima, List<ExpressionStore> exprMaximaToSMath)
        {
            counterS++;
            XmlWriter writer = null;
            try
            {
                // Create an XmlWriterSettings object with the correct options. 
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("\t");
                settings.OmitXmlDeclaration = true;
                // Create the XmlWriter object and write some content.
                writer = XmlWriter.Create(path, settings);
                writer.WriteStartElement("MaximaPlugin");
                //Settings start
                writer.WriteStartElement("Settings");
                writer.WriteStartElement("PathToMaxima.bat");
                writer.WriteString(wSettings[0]);
                writer.WriteEndElement();
                writer.WriteStartElement("ID");
                writer.WriteString(wSettings[1]);
                writer.WriteEndElement();
                writer.WriteStartElement("Version");
                writer.WriteAttributeString("Major", wSettings[2]);
                writer.WriteAttributeString("Minor", wSettings[3]);
                writer.WriteAttributeString("Build", wSettings[4]);
                writer.WriteAttributeString("Revision", wSettings[5]);
                writer.WriteString(wSettings[6]);
                writer.WriteEndElement();
                writer.WriteEndElement();
                //settings end
                //Load start
                writer.WriteStartElement("MaximaLoadCommands");
                foreach (string writeString in commands)
                {
                    writer.WriteElementString("Command", writeString);
                }
                writer.WriteEndElement();
                //Load end
                writer.WriteComment("Here you can add your own load() commands.");
                //Load start
                writer.WriteStartElement("CustomLoadCommands");
                foreach (string writeString in customCommands)
                {
                    writer.WriteElementString("Command", writeString);
                }
                writer.WriteEndElement();
                //Load end
                writer.WriteComment("Here you can add your functions to convert. Use {0} for argument separator and {1} for decimal symbol");
                //Converting start
                writer.WriteStartElement("Convert");
                writer.WriteStartElement("ConvertFromSMathToMaxima");
                foreach (ExpressionStore exp in exprSMathToMaxima)
                {
                    writer.WriteStartElement("Expression");
                    writer.WriteAttributeString("regex", exp.regex);
                    writer.WriteAttributeString("replace", exp.replace);
                    writer.WriteString(exp.functionName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteStartElement("ConvertFromMaximaToSMath");
                foreach (ExpressionStore exp in exprMaximaToSMath)
                {
                    writer.WriteStartElement("Expression");
                    writer.WriteAttributeString("regex", exp.regex);
                    writer.WriteAttributeString("replace", exp.replace);
                    writer.WriteString(exp.functionName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                //Converting end
                writer.WriteEndElement();
                writer.Flush();
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Read config file
        /// TODO MK 2017 08 01: See if that can be simplified. 
        /// </summary>
        /// <param name="filepath">path to config file</param>
        /// <param name="wSettings">Plugin configuration info</param>
        /// <param name="commands">Default maxima init commands</param>
        /// <param name="customCommands">Custom maxima init commands</param>
        /// <param name="exprSMathToMaxima">Custom SMath to Maxima translations</param>
        /// <param name="exprMaximaToSMath">Custom Maxima to SMath translations</param>
        /// <param name="option"></param>
        public static void readXmlALL(string filepath, List<string> settings, List<string> commands, List<string> customCommands, List<ExpressionStore> exprSMathToMaxima, List<ExpressionStore> exprMaximaToSMath)
        {
            counterS++;

            XmlReader reader = XmlReader.Create(filepath);
            while (reader.Read())
            {
                // get settings
                if (reader.NodeType == XmlNodeType.Element
                && reader.Name == "Settings")
                {
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Read();

                        if (reader.Name == "PathToMaxima.bat")
                        {
                            while (reader.NodeType != XmlNodeType.EndElement)
                            {
                                if (reader.NodeType == XmlNodeType.Text)
                                {
                                    settings.Add(reader.Value);
                                }
                                reader.Read();
                            }
                            reader.Read();
                        }
                        if (reader.Name == "ID")
                        {
                            while (reader.NodeType != XmlNodeType.EndElement)
                            {
                                if (reader.NodeType == XmlNodeType.Text)
                                {
                                    settings.Add(reader.Value);
                                }
                                reader.Read();
                            }
                            reader.Read();
                        }
                        if (reader.Name == "Version")
                        {
                            settings.Add(reader.GetAttribute(0));
                            settings.Add(reader.GetAttribute(1));
                            settings.Add(reader.GetAttribute(2));
                            settings.Add(reader.GetAttribute(3));

                            while (reader.NodeType != XmlNodeType.EndElement)
                            {
                                reader.Read();
                            }
                            reader.Read();
                        }
                    }
                    reader.Read();

                }
                // get default init commands
                if (reader.NodeType == XmlNodeType.Element
                && reader.Name == "MaximaLoadCommands")
                {
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Read();
                        if (reader.Name == "Command")
                        {
                            while (reader.NodeType != XmlNodeType.EndElement)
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Text)
                                {
                                    commands.Add(reader.Value);
                                }
                            }
                            reader.Read();
                        }
                    }
                }
                // get custom init command
                if (reader.NodeType == XmlNodeType.Element
                && reader.Name == "CustomLoadCommands")
                {
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Read();
                        if (reader.Name == "Command")
                        {
                            while (reader.NodeType != XmlNodeType.EndElement)
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Text)
                                {
                                    customCommands.Add(reader.Value);
                                }
                            }
                            reader.Read();
                        }
                    }
                }
                // get convering stuff
                if (reader.NodeType == XmlNodeType.Element
                && reader.Name == "Convert")
                {
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Read();
                        if (reader.Name == "ConvertFromSMathToMaxima")
                        {
                            while (reader.NodeType != XmlNodeType.EndElement)
                            {
                                if (reader.Name == "Expression")
                                {
                                    exprSMathToMaxima.Add(new ExpressionStore("looknext", reader.GetAttribute(0), reader.GetAttribute(1)));
                                    while (reader.NodeType != XmlNodeType.EndElement)
                                    {
                                        reader.Read();
                                        if (reader.NodeType == XmlNodeType.Text)
                                        {
                                            exprSMathToMaxima[exprSMathToMaxima.Count - 1].functionName = reader.Value;
                                        }
                                    }
                                }
                                reader.Read();
                            }
                            reader.Read();
                        }
                        if (reader.Name == "ConvertFromMaximaToSMath")
                        {
                            while (reader.NodeType != XmlNodeType.EndElement)
                            {
                                if (reader.Name == "Expression")
                                {
                                    exprMaximaToSMath.Add(new ExpressionStore("looknext", reader.GetAttribute(0), reader.GetAttribute(1)));
                                    while (reader.NodeType != XmlNodeType.EndElement)
                                    {
                                        reader.Read();
                                        if (reader.NodeType == XmlNodeType.Text)
                                        {
                                            exprMaximaToSMath[exprMaximaToSMath.Count - 1].functionName = reader.Value;
                                        }
                                    }
                                }
                                reader.Read();
                            }
                            reader.Read();
                        }
                    }
                }
            }
            reader.Close();
        }
    }
}

