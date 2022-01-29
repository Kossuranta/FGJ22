using System;

[Flags]
public enum ColorEnum
{
    None = 0,
    Red = 1 << 1,
    Green = 1 << 2,
    Blue = 1 << 3,
}
public enum Levels
{
    Level_01,
    Level_02,
    Level_03,
    Level_04
}