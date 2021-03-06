﻿using System.Collections.Generic;

namespace ZeroCompression
{
    public class Encoder
    {
        private int _sequenceLength = 128;

        public Encoder(int sequenceLength)
        {
            _sequenceLength = sequenceLength;
        }

        public List<byte> Encode(List<byte> inputList)
        {
            int counter = 1;

            List<byte> outputList = new List<byte>();

            for(int i = 1; i < inputList.Count; i++)
            {
                if (counter == _sequenceLength)
                {
                    compress(ref outputList, inputList[i - 1], ref counter);
                    continue;
                }
                if (i == inputList.Count - 1) // last item
                {
                    if (inputList[i] == inputList[i - 1]) // last item is part of a sequence
                    {
                        counter++;
                        compress(ref outputList, inputList[i - 1], ref counter);
                    }
                    else // last item is not part of a sequence
                    {
                        if (counter > 1) // a sequence just ended before the last item 
                            compress(ref outputList, inputList[i - 1], ref counter); // add previous sequence
                        else // item before last was not part of a sequence
                            outputList.Add(inputList[i-1]); // add previous item 
                        outputList.Add(inputList[i]); // add current item
                    }
                }
                else // any item except the last
                {
                    if (inputList[i] == inputList[i - 1])
                        counter++;
                    else
                    {
                        if (counter <= 1) // not the end of a consequtive sequence of 1's or 0's
                            outputList.Add(inputList[i - 1]);
                        else
                            compress(ref outputList, inputList[i - 1], ref counter);
                    }
                }
            }
            return outputList;
        }

        private void compress(ref List<byte> list, byte sequenceType, ref int sequenceLength)
        {
            if (sequenceType == 0x01) // compress 1's
                list.Add((byte)(sequenceLength + 128));
            else // compress 0's
                list.Add((byte)(sequenceLength));

            sequenceLength = 1; // reset sequence counter
        }
    }
}
