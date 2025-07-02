using System.IO;
using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    public string outputFilePath = "recorded.wav";
    private AudioClip recordedClip;
    private bool isRecording = false;

    public void StartRecording()
    {
        recordedClip = Microphone.Start(null, false, 5, 44100);
        isRecording = true;

    }

    public void StopRecording()
    {
        if (!isRecording) return;
        Microphone.End(null);
        SavWav.Save(outputFilePath, recordedClip);
        isRecording = false;
    }

    public string GetAudioPath()
    {
        return Path.Combine(Application.persistentDataPath, outputFilePath);
    }
}
