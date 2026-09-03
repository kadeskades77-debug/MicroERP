namespace MicroERP.Application.Common.Files;

public static class FileSignatures
{
    public static readonly Dictionary<string, byte[][]>
        Signatures = new(StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] =
            [
                new byte[] { 0x25, 0x50, 0x44, 0x46 }
            ],

            [".png"] =
            [
                new byte[]
                {
                    0x89, 0x50, 0x4E, 0x47,
                    0x0D, 0x0A, 0x1A, 0x0A
                }
            ],

            [".jpg"] =
            [
                new byte[] { 0xFF, 0xD8, 0xFF }
            ],

            [".jpeg"] =
            [
                new byte[] { 0xFF, 0xD8, 0xFF }
            ],

            [".doc"] =
            [
                new byte[]
                {
                    0xD0, 0xCF, 0x11, 0xE0,
                    0xA1, 0xB1, 0x1A, 0xE1
                }
            ],

            // DOCX is a ZIP container.
            [".docx"] =
            [
                new byte[]
                {
                    0x50, 0x4B, 0x03, 0x04
                },

                new byte[]
                {
                    0x50, 0x4B, 0x05, 0x06
                },

                new byte[]
                {
                    0x50, 0x4B, 0x07, 0x08
                }
            ]
        };
}