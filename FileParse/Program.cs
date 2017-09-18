using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileParse
{
    public class Field
    {
        public string FieldName;
        public string  Value;
    }

    public class Record
    {
        public bool Found = false; 
        public string SortField = ""; 
        public List<Field> Fields = new List<Field>();
    }

    class Program
    {
        public static List<Record> Report = new List<Record>();
        public static string MyParamName;
        public static string FileName ="";
        public static string Sort = "";
        public static string SearchTerm = "";

        
        static void Main(string[] args)
        {
            //iterate through command args
            for (int i = 0; i < args.Length; i++)
            {

               
                if (args[i][0] == '–' || args[i][0] == '-')
                { MyParamName = args[i].Substring(1); } //this argument is a parameter
                else
                    switch (MyParamName) 
                    {
                        case "file":
                            FileName += ' ' + args[i].Trim('"').Trim('“').Trim('”');
                            break;
                        case "sort":
                            Sort += ' ' + args[i].Trim('"').Trim('“').Trim('”');
                            break;
                        case "search":
                            SearchTerm += ' ' + args[i].Trim('"').Trim('“').Trim('”');
                            break;
                        default: //discard unrecognized parameter
                            System.Console.WriteLine("?: {0}", MyParamName);
                            break;
                    }       

            }
            FileName = FileName.TrimStart(' ');
            SearchTerm = SearchTerm.TrimStart(' ');
            Sort = Sort.TrimStart(' ');

            // make sure we have the required parameter
            if (FileName != "")
            { ParseFile(); }
            else
        	{
            Console.WriteLine("Filename not supplied");

	
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static void ParseFile()
        {


            if (System.IO.File.Exists(FileName))
            {
                Record MyRecord = new Record();

                int counter = 0;
                string line;

                // Read the file line by line.
                System.IO.StreamReader file =
                   new System.IO.StreamReader(FileName);
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains("=END OF RESULT=")) { //found record terminator
                        if (MyRecord.Found) Report.Add(MyRecord); //add unless the record is to be filtered
                        
                        MyRecord = new Record();
                        //new record
                    } else {
                        string[] part;
                        part = line.Split(':');

                        if (part.Length > 1)
                        {
                            Field MyField = new Field();
                            MyField.FieldName = part[0];
                            MyField.Value = part[1].TrimStart(' ');

                            MyRecord.Fields.Add(MyField);
                            //if there is no 'search' parameter we bypassn the filtering otherwise look for the search term
                            if (SearchTerm == "" || MyField.Value.Contains(SearchTerm)) MyRecord.Found = true;
                            
                            //if there is a sort parameter and this is the field that matches then store the value in the 'SortField' property
                            if (Sort != "" && MyField.FieldName == Sort) MyRecord.SortField = MyField.Value;

                        }
                    }
                    counter++;
                }

                file.Close();
                if (Sort != "") Report.Sort((x, y) => x.SortField.CompareTo(y.SortField)); //sort the list
                
                foreach (var RecX in Report)
                {
                    Console.WriteLine("---------------------");
                    foreach (var FldX in RecX.Fields) { 
                        string strX = string.Format("{0,-20}", FldX.FieldName); //format output
                        Console.WriteLine("{0}:{1}", strX, FldX.Value);

                    }
                }
            }
            else
            {
                Console.WriteLine("FILE NOT FOUND");

            }

                
        }
    }
}
