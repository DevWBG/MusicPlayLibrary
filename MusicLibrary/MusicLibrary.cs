using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO;

namespace MusicLibrary
{
    public abstract class Music
    {
        public abstract bool FileInput(string strFilePath);
        public abstract bool Play();
        public abstract bool Stop();
        public abstract bool Pause();
        public abstract bool Volume(int m_nVolume);
        public abstract bool Close();
    }

    internal class MP3File
    {
        public string m_strFilePath;
        public string m_strFileName;
        public string m_strFileComplete;
        public bool m_bHasID3Tag;
        public string m_strId3Title;
        public string m_strId3Artist;
        public string m_strId3Album;
        public string m_strId3Year;
        public string m_strId3Comment;
        public byte m_byId3TrackNumber;
        public byte m_byId3Genre;
        public string m_strFileType;

        public MP3File(string path, string name)
        {
            this.m_strFilePath = path;
            this.m_strFileName = name;
            this.m_strFileComplete = path + "\\" + name;
            this.m_bHasID3Tag = false;
            this.m_strId3Title = null;
            this.m_strId3Artist = null;
            this.m_strId3Album = null;
            this.m_strId3Year = null;
            this.m_strId3Comment = null;
            this.m_byId3TrackNumber = 0;
            this.m_byId3Genre = 0;
            this.m_strFileType = null;
        }
    }

    public class MP3 : Music
    {
        StringBuilder returnData = null;
        long error = 0;
        bool m_bPause = false;
        MP3File m_mp3File = null;
        string playCommand;
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        public override bool Play()
        {
            playCommand = "play MediaFile";
            error = mciSendString(playCommand, null, 0, IntPtr.Zero);
            if (error == 0)
            {
                return true;
            }
            else
            {
                Close();
                return false;
            }
        }

        private void Resume()
        {
            playCommand = "resume MediaFile";
            error = mciSendString(playCommand, null, 0, IntPtr.Zero);
        }

        public override bool Pause()
        {
            if (m_bPause)
            {
                Resume();
                m_bPause = false;
            }
            else if (IsPlaying())
            {
                playCommand = "pause MediaFile";
                error = mciSendString(playCommand, null, 0, IntPtr.Zero);
                m_bPause = true;
            }
            throw new NotImplementedException();
        }

        public override bool Close()
        {
            playCommand = "close MediaFile";
            if(mciSendString(playCommand, null, 0, IntPtr.Zero) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Stop()
        {
            playCommand = "stop MediaFile";
            error = mciSendString(playCommand, returnData, 128, IntPtr.Zero);
            if(error != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsPlaying()
        {
            playCommand = "status MediaFile mode";
            error = mciSendString(playCommand, returnData, 128, IntPtr.Zero);
            if (returnData.Length == 7 && returnData.ToString().Substring(0, 7) == "playing")
                return true;
            else
                return false;
        }

        public override bool FileInput(string strFilePath)
        {
            m_mp3File.m_strFilePath = strFilePath;
            Close();
            if (error != 0)
            {
                // Let MCI deside which file type the song is
                playCommand = "open \"" + m_mp3File.m_strFilePath + "\" alias MediaFile";
                error = mciSendString(playCommand, null, 0, IntPtr.Zero);
                if (error == 0)
                    return true;
                else
                    return false;
            }
            else
                return true;

            throw new NotImplementedException();
        }

        public override bool Volume(int m_nVolume)
        {
            playCommand = "setaudio MediaFile volume to " + m_nVolume.ToString();
            error = mciSendString(playCommand, null, 0, IntPtr.Zero);
            if(error != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
