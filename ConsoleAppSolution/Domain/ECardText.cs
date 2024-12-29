using System.ComponentModel;
using System.Reflection;

namespace Domain
{
    public enum ECardText
    {
        [Description("0")]
        Zero,

        [Description("1")]
        One,

        [Description("2")]
        Two,

        [Description("3")]
        Three,

        [Description("4")]
        Four,

        [Description("5")]
        Five,

        [Description("6")]
        Six,

        [Description("7")]
        Seven,

        [Description("8")]
        Eight,

        [Description("9")]
        Nine,

        [Description("Skip")]
        Skip,

        [Description("Reverse")]
        Reverse,

        [Description("+2")]
        PlusTwo,

        [Description("Choose Color")]
        ChooseColor,
        
        [Description("+4 Choose Color")]
        ChooseColorPlusFour
    }
    
    public static class EnumExtensions
    {
        public static string ToDescriptionString(this Enum value)
        {
            String s = value.ToString();
            FieldInfo? field = value.GetType().GetField(value.ToString());

            if (field == null) return value.ToString();

            DescriptionAttribute? attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}