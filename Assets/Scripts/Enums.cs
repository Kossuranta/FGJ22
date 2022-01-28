using System;

[Flags]
public enum ColorEnum
{
    None = 0,
    Red = 1 << 1,
    Green = 1 << 2,
    Blue = 1 << 3,
}