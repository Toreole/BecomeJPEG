using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BecomeJPEG
{
    internal static class Util
    {
        /// <summary>
        /// Attempts to parse an integer from a specified entry in a string array.
        /// </summary>
        /// <param name="i">the integer var that receives the result</param>
        /// <param name="arr">the string array</param>
        /// <param name="index">the index of the string you want to parse</param>
        /// <param name="defaultValue">the default value if index is invalid</param>
        internal static void ParseIntFromStrArr(ref int i, string[] arr, int index, int defaultValue)
        {
            if (index < arr.Length)
                if (!int.TryParse(arr[index], out i)) //if parse fails, return the default value.
                    i = defaultValue;
        }

        /// <summary>
        /// Attempts to parse a float from a specified entry in a string array.
        /// </summary>
        /// <param name="i">the float var that receives the result</param>
        /// <param name="arr">the string array</param>
        /// <param name="index">the index of the string you want to parse</param>
        /// <param name="defaultValue">the default value if index is invalid</param>
        internal static void ParseFloatFromStrArr(ref float i, string[] arr, int index, float defaultValue)
        {
            if (index < arr.Length)
                i = float.Parse(arr[index]);
            else
                i = defaultValue;
        }

        /// <summary>
        /// Gets all supported resolutions from a source.<br/>
        /// Taken from https://stackoverflow.com/questions/20414099/videocamera-get-supported-resolutions 
        /// </summary>
        internal static List<Resolution> GetAllAvailableResolutions(DsDevice vidDev)
        {
            try
            {
                //HRESULT, bitCount?
                int hr, bitCount = 0;

                IBaseFilter sourceFilter = null;

                var filterGraph2 = new FilterGraph() as IFilterGraph2;
                hr = filterGraph2.AddSourceFilterForMoniker(vidDev.Mon, null, vidDev.Name, out sourceFilter);
                var pRaw2 = DsFindPin.ByCategory(sourceFilter, PinCategory.Capture, 0);
                var availableResoltuions = new List<Resolution>();

                var v = new VideoInfoHeader();
                IEnumMediaTypes mediaTypeE;
                hr = pRaw2.EnumMediaTypes(out mediaTypeE);

                AMMediaType[] mediaTypes = new AMMediaType[1];
                IntPtr fetched = IntPtr.Zero;
                hr = mediaTypeE.Next(1, mediaTypes, fetched);

                while(fetched != null && mediaTypes[0] != null)
                {
                    Marshal.PtrToStructure(mediaTypes[0].formatPtr, v);
                    if(v.BmiHeader.Size != 0 && v.BmiHeader.BitCount != 0)
                    {
                        if(v.BmiHeader.BitCount > bitCount)
                        {
                            availableResoltuions.Clear();
                            bitCount = v.BmiHeader.BitCount;
                        }
                        availableResoltuions.Add(new Resolution(v.BmiHeader.Width, v.BmiHeader.Height));
                    }
                    hr = mediaTypeE.Next(1, mediaTypes, fetched);
                }
                return availableResoltuions;
            }
            catch(Exception ex)
            {
                Logger.LogLine(ex.Message);
                return new List<Resolution>();
            }
        }
    }
}
