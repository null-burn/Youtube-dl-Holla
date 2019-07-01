using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace YoutubeDL_Holla.Helpers
{
    public class Output
    {
        public class AvailableMediaOutput
        {
            public string FormatCode { get; set; }
            public string Extension { get; set; }
            public string Resolution { get; set; }
            public string Note { get; set; }
            public string Description { get; set; }
            public int AudioBitrate { get; set; }
        }

        public List<AvailableMediaOutput> CreateAvailableMediaList(string[] consoleLines)
        {
            List<AvailableMediaOutput> availableMediaOutputList = new List<AvailableMediaOutput>();
            bool startReading = false;
            bool nextLineHeader = false;

            int positionFormatCode = 0;
            int positionExtension = 0;
            int positionResolution = 0;
            int positionNote = 0;

            string formatCodeHeader = "format code";
            string extensionHeader = "extension";
            string resolutionHeader = "resolution";
            string noteHeader = "note";

            foreach (string line in consoleLines)
            {
                if (startReading && line.Trim().Length > 0)
                {
                    if (line.Contains("exited."))
                    {
                        startReading = false;
                    }
                    else
                    {
                        AvailableMediaOutput availableMediaOutput = new AvailableMediaOutput();
                        availableMediaOutput.FormatCode = line.Substring(positionFormatCode, positionExtension - positionFormatCode).Trim();
                        availableMediaOutput.Extension = line.Substring(positionExtension, positionResolution - positionExtension).Trim();
                        availableMediaOutput.Resolution = line.Substring(positionResolution, positionNote - positionResolution).Trim();
                        availableMediaOutput.Note = line.Substring(positionNote, line.Length - positionNote).Trim();

                        if (availableMediaOutput.Resolution != "audio only" && availableMediaOutput.Note.Contains("@"))
                        {
                            availableMediaOutput.Description = availableMediaOutput.Extension + " - " + availableMediaOutput.Note + " (might contain audio)";
                        }
                        else
                        {
                            availableMediaOutput.Description = availableMediaOutput.Extension + " - " + availableMediaOutput.Note;
                        }

                        //try
                        //{
                        //    string bitrate = Regex.Match("", "((\\d+)k)").Value.Replace("k", "");
                        //    if(int.TryParse(bitrate, out int bt))
                        //    {
                        //        if (bt <= 128)
                        //        {
                        //            availableMediaOutput.AudioBitrate = 128;
                        //        }
                        //        else if (bt <= 192)
                        //        {
                        //            availableMediaOutput.AudioBitrate = 192;
                        //        }
                        //        else
                        //        {
                        //            availableMediaOutput.AudioBitrate = 320;
                        //        }
                        //    }
                        //}
                        //catch
                        //{

                        //}
                        
                        availableMediaOutputList.Add(availableMediaOutput);
                    }
                }
                if (nextLineHeader)
                {
                    positionFormatCode = line.IndexOf(formatCodeHeader);
                    positionExtension = line.IndexOf(extensionHeader);
                    positionResolution = line.IndexOf(resolutionHeader);
                    positionNote = line.IndexOf(noteHeader);

                    nextLineHeader = false;
                    startReading = true;
                }
                if (line.Contains("[info] Available formats"))
                {
                    nextLineHeader = true;
                }
            }

            return availableMediaOutputList;
        }
    }
}
