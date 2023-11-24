using System.ComponentModel;

namespace Domain
{
    public enum EUnoCardValue
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

        [Description("+4")]
        PlusFour,

        [Description("Choose Color")]
        ChooseColor,
        
        [Description("+4 Choose Color")]
        ChooseColorPlusFour
    }
}