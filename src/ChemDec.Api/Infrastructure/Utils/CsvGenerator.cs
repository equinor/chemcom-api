using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace ChemDec.Api.Infrastructure.Utils
{
    public class CsvGenerator
    {
        private const string DELIMITER = ";";

        public string ToString<T>(IList<T> list, bool createHeader = true)
        {
            StringBuilder builder = new StringBuilder();

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            if (createHeader)
            {
                builder.AppendLine(this.CreateHeader(properties));
            }

            foreach (var item in list)
            {
                builder.AppendLine(this.CreateLine(item, properties));
            }

            return builder.ToString();
        }

        public string Write<T>(IList<T> list, string fileName, bool createHeader = true)
        {
            string csv = this.ToString(list, createHeader);

            this.ToFile(fileName, csv);

            return csv;
        }

        private string CreateHeader(PropertyInfo[] properties)
        {
            List<string> propertyValues = new List<string>();

            foreach (var property in properties)
            {
                string value = property.Name;

                var attribute = property.GetCustomAttribute(typeof(DisplayAttribute));
                if (attribute != null)
                {
                    value = (attribute as DisplayAttribute).Name;
                }

                this.CreateString(propertyValues, value);
            }

            return this.CreateLine(propertyValues);
        }

        private string CreateLine<T>(T item, PropertyInfo[] properties)
        {
            List<string> propertyValues = new List<string>();

            foreach (var property in properties)
            {
                object value = property.GetValue(item, null);

                if (property.PropertyType == typeof(string))
                {
                    this.CreateString(propertyValues, value);
                }
                else if (property.PropertyType == typeof(string[]))
                {
                    this.CreateStringArray(propertyValues, value);
                }
                else if (property.PropertyType == typeof(List<string>))
                {
                    this.CreateStringList(propertyValues, value);
                }
                else
                {
                    this.CreateObject(propertyValues, value);
                }
            }

            return this.CreateLine(propertyValues);
        }

        private string CreateLine(IList<string> list)
        {
            return string.Join(DELIMITER, list);
        }

        private void CreateObject(List<string> propertyValues, object value)
        {
            if (value != null)
            {
                string stringValue;

                if (value is double d)
                {
                    //stringValue = d.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                    stringValue = d.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    stringValue = value.ToString();
                }

                propertyValues.Add(stringValue);
            }
            else
            {
                propertyValues.Add(string.Empty);
            }
        }

        private void CreateStringList(List<string> propertyValues, object value)
        {
            string formatString = "\"{0}\"";
            if (value != null)
            {
                value = this.CreateLine((List<string>)value);
                propertyValues.Add(string.Format(formatString, this.EscapeString(value)));
            }
            else
            {
                propertyValues.Add(string.Empty);
            }
        }

        private void CreateStringArray(List<string> propertyValues, object value)
        {
            string formatString = "\"{0}\"";
            if (value != null)
            {
                value = this.CreateLine(((string[])value).ToList());
                propertyValues.Add(string.Format(formatString, this.EscapeString(value)));
            }
            else
            {
                propertyValues.Add(string.Empty);
            }
        }

        private void CreateString(List<string> propertyValues, object value)
        {
            string formatString = "\"{0}\"";
            if (value != null)
            {
                propertyValues.Add(string.Format(formatString, this.EscapeString(value)));
            }
            else
            {
                propertyValues.Add(string.Empty);
            }
        }

        private string EscapeString(object value)
        {
            return value.ToString().Replace("\"", "\"\"");
        }

        public bool ToFile(string fileName, string csv)
        {
            bool fileCreated = false;

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                File.WriteAllText(fileName, csv);
                fileCreated = true;
            }

            return fileCreated;
        }
    }
}
