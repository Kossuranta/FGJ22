using System;

[Flags]
public enum ColorEnum
{
    None = 0,
    Red = 1 << 1,
    Green = 1 << 2,
    Purple = 1 << 3,
    Orange = 1 << 4,
}
public enum Levels
{
    Hub = 0,
    Level_01 = 1,
    Level_02 = 2,
    Level_03 = 3,
    Level_04 = 4,
}