namespace FileRipperCore.Domain
{
    public enum FileTypes
    {
        FixedWidth,
        Delimited,
        Xml
    }
    
    public class FieldDefinition
    {
        public string FieldName { get; protected set; }
        public FileTypes? FileType { get; protected set; }
        public int? StartPosition { get; protected set; }
        public int? FieldLength { get; protected set; }
        public string XmlNodeName { get; protected set; }
        public int? PositionInRow { get; protected set; }
        
        public static FieldDefinition BuildDelimitedField(string fieldName, int positionInRow)
        {
            return new FieldDefinition()
            {
                FieldName = fieldName,
                PositionInRow = positionInRow,
                FileType = FileTypes.Delimited
            };
        }

        public static FieldDefinition BuildXmlField(string fieldName, string xmlNodeName = null)
        {
            return new FieldDefinition()
            {
                FieldName = fieldName,
                XmlNodeName = xmlNodeName ?? fieldName,
                FileType = FileTypes.Xml
            };
        }

        public static FieldDefinition BuildFixedWidthField(string fieldName, int startPosition, int fieldLength)
        {
            return new FieldDefinition()
            {
                FieldName = fieldName,
                StartPosition = startPosition,
                FieldLength = fieldLength,
                FileType = FileTypes.FixedWidth
            };
        }
    }
}