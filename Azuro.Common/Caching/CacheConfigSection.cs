using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;

namespace Azuro.Caching
{
    public enum Strategy
    {
        Custom,
        Http,
        Memory
    }

    [XmlRoot( "Azuro.Caching" )]
    public class CacheConfigSection
    {
        [XmlAttribute( "strategy" )]
        public Strategy Strategy;
        [XmlAttribute( "type" )]
        public string Type;
        [XmlAttribute( "assembly" )]
        public string Assembly;
    }

    public class CacheConfigSectionHandler : IConfigurationSectionHandler
    {
        public object Create( object parent, object configContext, XmlNode section )
        {
            CacheConfigSection cfg = new CacheConfigSection();
            if( section == null ) return cfg;
            XmlSerializer xmlSerializer = new XmlSerializer( typeof( CacheConfigSection ) );
            cfg = (CacheConfigSection)xmlSerializer.Deserialize( new XmlTextReader( new StringReader( section.OuterXml ) ) );
            return cfg;
        }
    }
}
