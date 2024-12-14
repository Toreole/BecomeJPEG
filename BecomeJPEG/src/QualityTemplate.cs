using System;
using static BecomeJPEG.Util;

namespace BecomeJPEG
{
    [Serializable] //this could be a struct, but i just enjoy having a paramless ctor, which isnt a feature for structs in this version of C#.
    internal class QualityTemplate
    {
        internal string templateName;
        internal int frameDropChance;
        internal int compressionQuality;
        internal int frameLagTime;
        internal int frameLagRandom;
        internal int repeatChance;
        internal int repeatFrameCount;
        internal int repeatCooldown;
        internal int repeatChain;

        //default values provided by constructor
        internal QualityTemplate()
        {
            frameDropChance = 55;
            compressionQuality = 0;
            frameLagTime = 100;
            frameLagRandom = 400;
            repeatChance = 0;
            repeatFrameCount = 12;
            repeatCooldown = 100;
            repeatChain = 1;
        }

        internal QualityTemplate(string name) : this()
        {
            templateName = name;
        }

        internal QualityTemplate(
            string name, 
            int dropRate, 
            int quality, 
            int lagTime, 
            int lagRandom, 
            int repeatChance, 
            int repeatFrameCount, 
            int repeatCooldown, 
            int repeatChain)
        {
            templateName = name;
            frameDropChance = dropRate;
            compressionQuality = quality;
            frameLagTime = lagTime;
            frameLagRandom = lagRandom;
            this.repeatChance = repeatChance;
            this.repeatFrameCount = repeatFrameCount;
            this.repeatCooldown = repeatCooldown;
            this.repeatChain = repeatChain;
        }

        public override string ToString()
        {
            return $"{templateName}|{frameDropChance}|{compressionQuality}|{frameLagTime}|{frameLagRandom}|" +
                $"{repeatChance}|{repeatFrameCount}|{repeatCooldown}|{repeatChain}";
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
            ParseIntFromStrArr(ref t.frameDropChance, args, 1, 0);
            ParseIntFromStrArr(ref t.compressionQuality, args, 2, 100);
            ParseIntFromStrArr(ref t.frameLagTime, args, 3, 0);
            ParseIntFromStrArr(ref t.frameLagRandom, args, 4, 0);
			ParseIntFromStrArr(ref t.repeatChance, args, 5, 0);
			ParseIntFromStrArr(ref t.repeatFrameCount, args, 6, 12);
			ParseIntFromStrArr(ref t.repeatCooldown, args, 7, 12);
			ParseIntFromStrArr(ref t.repeatChain, args, 8, 12);
			return t;
        }
    }

    /// <summary>
    /// A shortlived readonly struct that holds the values of a QualityTemplate.
    /// Useful for events or small Get methods that should not even get the chance to modify the original QualityTemplate.
    /// </summary>
    internal readonly struct SReadonlyQualityTemplate
    {
        internal readonly int quality;
        internal readonly int lagTime;
        internal readonly int lagRandom;
        internal readonly int dropChance;
		internal readonly int repeatChance;
		internal readonly int repeatFrameCount;
        internal readonly int repeatCooldown;
        internal readonly int repeatChain;

		internal readonly bool isValid;
        internal readonly string name;

        internal SReadonlyQualityTemplate(QualityTemplate qt)
        {
            quality = qt.compressionQuality;
            lagTime = qt.frameLagTime;
            lagRandom = qt.frameLagRandom;
            dropChance = qt.frameDropChance;
            isValid = true;
            name = qt.templateName;
            repeatChance = qt.repeatChance;
            repeatFrameCount = qt.repeatFrameCount;
			repeatCooldown = qt.repeatCooldown;
            repeatChain = qt.repeatChain;
		}
    }
}
