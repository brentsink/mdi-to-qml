using System;
using System.IO;
using System.Linq;

namespace mdi_to_qml
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            // Check to make sure the file exists on the file system
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("File does not exist");
                return;
            }
            else if (!args[0].EndsWith(".css"))
            {
                Console.WriteLine("Not a valid .css file");
                return;
            }

            string qobjects = "#pragma once\n\n#include <QObject>\n#include <QString>\n\nclass MaterialIcons : public QObject\n{\n    Q_OBJECT\n\n";
            string values = "public:\n";
            string line;
            StreamReader file = new StreamReader(args[0]);

            while ((line = file.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith(".mdi-set {"))
                {
                    if (line.StartsWith(".mdi-"))
                    {
                        string property = line.Substring(1).Split('.').First();
                        property = property.Replace("mdi-", "").Replace(":before {", "");

                        string[] tokens = property.Split('-');
                        string name = "mdi";

                        foreach (var item in tokens)
                        {
                            name += ToTitleCase(item);
                        }

                        property = name;
                        line = file.ReadLine();

                        if (line != null && line.TrimStart().StartsWith("content:"))
                        {
                            string val = line.TrimStart().Replace("content: ", "").Replace(";", "").Replace("\\F", "\\uf");

                            qobjects += string.Format("    Q_PROPERTY(QString {0} READ {0} CONSTANT)\n", property);
                            values += string.Format("    QString {0}() const {{ return {1}; }}\n", property, val);
                        }
                    }
                }
            }

            string output = qobjects + "\n" + values + "};";
            File.WriteAllText(string.Format(@"{0}\MaterialIcons.h", Path.GetDirectoryName(args[0])), output);
        }

        public static string ToTitleCase(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
    }
}
