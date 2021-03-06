﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonDB
{
    public class IniFile
    {
        SortedList<string, string> Properties = new SortedList<string, string>();
        string filename;

        public IniFile(string file)
        {
            Reload(file);
        }
        
        public T GetValue<T>(string Key)
        {
            string value = Get(Key);
            Type type = typeof(T);
            if (value != null)
            {
                return (T)Convert.ChangeType(value, type);
            }
            return default;
        }

        string Get(string field)
        {
            if (Properties.TryGetValue(field, out string x))
                return x;

            return null;
        }

        public void SetValue(string field, object value)
        {
            Properties[field] = value.ToString();
        }

        public void Save()
        {
            Save(filename);
        }

        void Save(string filename)
        {
            this.filename = filename;

            if (!System.IO.File.Exists(filename))
                System.IO.File.Create(filename);

            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);

            foreach (string prop in Properties.Keys.ToArray())
                if (!string.IsNullOrWhiteSpace(Properties[prop]))
                    file.WriteLine(prop + "=" + Properties[prop]);

            file.Close();
        }

        public void Reload()
        {
            Reload(filename);
        }

        void Reload(string filename)
        {
            this.filename = filename;
            Properties = new SortedList<string, string>();

            if (System.IO.File.Exists(filename))
                Parse(filename);
            else
                System.IO.File.Create(filename);
        }

        private void Parse(string file)
        {
            try
            {
                string[] data = System.IO.File.ReadAllLines(file);
                foreach (string line in data)
                {
                    if ((!string.IsNullOrEmpty(line)) &&
                        (!line.StartsWith(";")) &&
                        (!line.StartsWith("#")) &&
                        (!line.StartsWith("'")) &&
                        (line.Contains('=')))
                    {
                        int index = line.IndexOf('=');
                        string key = line.Substring(0, index).Trim();
                        string value = line.Substring(index + 1).Trim();

                        if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                            (value.StartsWith("'") && value.EndsWith("'")))
                        {
                            value = value.Substring(1, value.Length - 2);
                        }

                        try
                        {
                            //ignore dublicates
                            Properties.Add(key, value);
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

    }
}
