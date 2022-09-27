using System;

namespace BecomeJPEG
{
    [Serializable] //this could be a struct, but i just enjoy having a paramless ctor, which isnt a feature for structs in this version of C#.
    internal class QualityTemplate
    {
        internal string templateName;
        internal float frameDropChance;
        internal int compressionQuality;
        internal int frameLagTime;
        internal int frameLagRandom;

        //default values provided by constructor
        internal QualityTemplate()
        {
            frameDropChance = 0.85f;
            compressionQuality = 0;
            frameLagTime = 100;
            frameLagRandom = 400;
        }

        internal QualityTemplate(string name) : this()
        {
            templateName = name;
        }

        internal QualityTemplate(string name, float dropRate, int quality, int lagTime, int lagRandom)
        {
            templateName = name;
            frameDropChance = dropRate;
            compressionQuality = quality;
            frameLagTime = lagTime;
            frameLagRandom = lagRandom;
        }

        public override string ToString()
        {
            return $"{templateName}|{frameDropChance}|{compressionQuality}|{frameLagTime}|{frameLagRandom}";
        }

        /// <summary>
        /// Creates a QualityTemplate from a string in a specific format.
        /// name|dropchance|quality|lagtime|lagrandom
        /// </summary>
        /// <param name="s">the string</param>
        /// <returns>The Template, or null if input was invalid. Missing params will be initialized to a default value.</returns>
        public static QualityTemplate FromString(string s)
        {
            string[] args = s.Split('|');
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
                return null;
            QualityTemplate t = new QualityTemplate(args[0]);
            TryParseFloatFrom(ref t.frameDropChance, args, 1, 0f);
            TryParseIntFrom(ref t.compressionQuality, args, 2, 100);
            TryParseIntFrom(ref t.frameLagTime, args, 3, 0);
            TryParseIntFrom(ref t.frameLagRandom, args, 4, 0);
            return t;
        }

        private static void TryParseIntFrom(ref int i, string[] arr, int index, int defaultValue)
        {
            if (index < arr.Length)
                i = int.Parse(arr[index]);
            else
                i = defaultValue;
        }
        private static void TryParseFloatFrom(ref float i, string[] arr, int index, float defaultValue)
        {
            if (index < arr.Length)
                i = float.Parse(arr[index]);
            else
                i = defaultValue;
        }
    }
}
