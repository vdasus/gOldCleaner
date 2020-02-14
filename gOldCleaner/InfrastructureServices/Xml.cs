using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace gOldCleaner.InfrastructureServices
{
    public static class Xml
    {
        public static string SerializeToXmlStringIndented<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (XmlTextWriter writer = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented })
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new XmlException("An error occurred", ex);
            }
        }

        public static string SerializeToPlainXmlString<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter, settings))
                {
                    xmlserializer.Serialize(writer, value, emptyNamespaces);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new XmlException("An error occurred", ex);
            }
        }

        public static string SerializeToPlainXmlStringEmptyStripped<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter, settings))
                {
                    xmlserializer.Serialize(writer, value, emptyNamespaces);
                    return Regex.Replace(stringWriter.ToString(), @"<[a-zA-Z].[^(><.)]+/>\s+", RemoveText);
                }
            }
            catch (Exception ex)
            {
                throw new XmlException("An error occurred", ex);
            }
        }

        public static string SerializeToXmlString<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (XmlTextWriter writer = new XmlTextWriter(stringWriter) { Formatting = Formatting.None })
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new XmlException("An error occurred", ex);
            }
        }

        public static string SerializeToXmlString(this string objectData)
        {
            return SerializeToXmlString<string>(objectData);
        }

        public static T XmlDeserializeFromString<T>(this string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static object XmlDeserializeFromString(this string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;
            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }
            return result;
        }

        static string RemoveText(Match m) { return ""; }
    }
}
