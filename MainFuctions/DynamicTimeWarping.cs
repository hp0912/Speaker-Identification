﻿using Recorder.MFCC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Recorder.DynamicTimeWarping
{
    class DynamicTimeWarpingOperations
    {

        public static void DTW(Sequence sequenceToBeCompared)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                string path = open.FileName;
                //Open the selected audio file
                AudioSignal signal = AudioOperations.OpenAudioFile(path);
                Sequence seq = AudioOperations.ExtractFeatures(signal);

                Console.WriteLine(DTW_Distance(sequenceToBeCompared, seq));
            }
        }

        public static double DTW_Distance(Sequence sequence1, Sequence sequence2)
        {
            int numberOfFrames_Sequence1 = sequence1.Frames.Count();
            int numberOfFrames_Sequence2 = sequence2.Frames.Count();
            double[,] DTW = new double[numberOfFrames_Sequence1 + 1, numberOfFrames_Sequence2 + 1];

            for (int i = 1; i <= numberOfFrames_Sequence1; i++)
            {
                DTW[i, 0] = 1e9;
            }
            for (int i = 1; i <= numberOfFrames_Sequence2; i++)
            {
                DTW[0, i] = 1e9;
            }
            DTW[0, 0] = 0;

            for (int i = 1; i <= numberOfFrames_Sequence1; i++)
            {
                for (int j = 1; j <= numberOfFrames_Sequence2; j++)
                {
                    double cost = distance(sequence1.Frames[i - 1], sequence2.Frames[j - 1]);
                    DTW[i, j] = cost + Math.Min(DTW[i - 1, j],
                                            Math.Min(DTW[i, j - 1],
                                            DTW[i - 1, j - 1]));
                }
            }
            return DTW[numberOfFrames_Sequence1, numberOfFrames_Sequence2];
        }

        private static double distance(MFCCFrame frame1, MFCCFrame frame2)
        {
            double difference_distance = 0;

            for (int i = 0; i < 13; i++)
            {
                difference_distance +=
                    (frame1.Features[i] - frame2.Features[i]) * (frame1.Features[i] - frame2.Features[i]);
            }

            return Math.Sqrt(difference_distance);
        }

        public static double LowerBound_Kim(Sequence sequence1, Sequence sequence2)
        {
            int sizeOfSequence1= sequence1.Frames.Count();
            int sizeOfSequence2= sequence2.Frames.Count();

            MFCCFrame firstElementInFirstSequence = sequence1.Frames[0];
            MFCCFrame firstElementInSecondSequence = sequence2.Frames[0];
            MFCCFrame lastElementInFirstSequence = sequence1.Frames[sizeOfSequence1 - 1];
            MFCCFrame lastElementInSecondSequence = sequence2.Frames[sizeOfSequence2 - 1];

            sequence1.Frames.OrderBy(f => f.Features);
            sequence2.Frames.OrderBy(f => f.Features);

            MFCCFrame minimumElementInFirstSequence = sequence1.Frames[0];
            MFCCFrame minimumElementInSecondSequence = sequence2.Frames[0];
            MFCCFrame maximumElementInFirstSequence = sequence1.Frames[sizeOfSequence1 - 1];
            MFCCFrame maximumElementInSecondSequence = sequence2.Frames[sizeOfSequence2 - 1];
            
            double distanceBetweenFirsts = distance(firstElementInFirstSequence, firstElementInSecondSequence);
            double distanceBetweenLasts = distance(lastElementInFirstSequence, lastElementInSecondSequence);
            double distanceBetweenMinimums = distance(minimumElementInFirstSequence, minimumElementInSecondSequence);
            double distanceBetweenMaximums = distance(maximumElementInFirstSequence, maximumElementInSecondSequence);
            double lowerBoundValue = Math.Min(
                                    Math.Min(distanceBetweenFirsts, distanceBetweenLasts),
                                    Math.Min(distanceBetweenMinimums, distanceBetweenMaximums));
            return lowerBoundValue;
        }
    }
}
