using System.Collections.Generic;
using System.Linq;

namespace Rogue.Ingame.Network
{
    public class FrameBuffer<T> where T : IDataFrame
    {
        private readonly List<T> buffer = new List<T>();

        public void Add(T data)
        {
            buffer.Add(data);
        }

        public void Update(int lastFrame)
        {
            while (buffer.Count > 1 && buffer[1].ServerFrame <= lastFrame)
                buffer.RemoveAt(0);
        }

        public IEnumerable<T> FindExactlyAll(int frame)
        {
            return buffer.FindAll(x => x.ServerFrame == frame);
        }

        public bool FindExactly(int frame, out T ret)
        {
            ret = buffer.FirstOrDefault(x => x.ServerFrame == frame);
            return ret.ServerFrame == frame;
        }

        public bool FindRecent(int frame, out T ret)
        {
            if (buffer.Count == 0)
            {
                ret = default;
                return false;
            }

            var idx = 0;
            while (idx + 1 < buffer.Count)
            {
                if (frame >= buffer[idx + 1].ServerFrame)
                    idx++;
                else
                    break;
            }

            ret = buffer[idx];
            return true;
        }
    }
}