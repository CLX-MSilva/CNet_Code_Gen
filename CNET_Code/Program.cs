//display header
using System.Xml.Linq;

Console.WriteLine("Extract names from XML");
Console.WriteLine("<thing name=\"MyName\"> to MyName");
Console.WriteLine("-------------------------------------------------------------------------------");

Start:

//get folder path
Console.WriteLine("Enter file path:");
string path = Console.ReadLine();

//test for invalid path
if (!File.Exists(path))
{
    //display error message
    Console.WriteLine("--Invalid file path--");
    goto Start;
}

//get PVP XML file
StreamReader sr = new(path);
StreamWriter sw = new(Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + " Names.txt");
string line;
List<string> names = new();

while(!sr.EndOfStream)
{
    //get next line of file
    line = sr.ReadLine();

    //test if name has lines
    if(line.Contains("<Module Name=\""))
    {
        //remove everthing that isn't part of the name
        line = line.Substring(line.IndexOf("<Module Name=\""));
        line = line.Replace("<Module Name=\"", "");
        line = line.Remove(line.IndexOf("\""));

        //only keep module names
        if (line.Contains('_') && char.IsDigit(line.Last()))
        {
            sw.WriteLine(line);
            names.Add(line);
        }
    }
}

sr.Close();
sw.WriteLine("");

//test for no names
if (names.Count < 1)
    goto end;

line = "";
List<string> groups = new();

for (int n = 1; n < names.Count; n++)
{
    //test for new group
    if (line == "")
        line = names[n - 1] + ",";

    if (names[n].Remove(names[n].IndexOf('_')) == names[n - 1].Remove(names[n - 1].IndexOf('_')))
        line += names[n] + ",";
    else
    {
        //remove last comma
        if (line.Last() == ',')
            line = line.Remove(line.LastIndexOf(','));

        sw.WriteLine(line);
        groups.Add(line);

        line = "";
    }
}

sw.WriteLine("");

foreach (string s in groups)
{
    string[] items = s.Split(',');
    string rung1 = "", rung2 = "", rung3 = "";

    int n = 1;

    foreach(string i in items)
    {
        rung1 += $"GSV(Module,{i},Mode,Cnet_Card[{n}])";
        rung2 += $"OTL(Cnet_Card[{n}].2)";
        rung3 += $"SSV(Module,{i},Mode,Cnet_Card[{n++}])";
    }

    sw.WriteLine($"{rung1};{Environment.NewLine}{rung2};{Environment.NewLine}{rung3};{Environment.NewLine}NOP();");
}

end:
sw.Close();

//display finished message
Console.WriteLine("Done\n");
goto Start;
